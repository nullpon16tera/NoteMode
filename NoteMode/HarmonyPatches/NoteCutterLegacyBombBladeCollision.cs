using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using NoteMode.Configuration;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    /// <summary>
    /// ノーツ／ボムオブジェクトの Sphere 等そのものではなく、<see cref="NoteCutter.Cut"/> が張る
    /// 「ブレードがカットを検出する空間」の置き方がバージョンで変わった問題向けの近似。
    /// 公式側ではボムだけ検出範囲がプレイヤー前方寄りになったり、体前〜約1mが安全扱いになったりする変更がある。
    /// ここではブレード点を水平後方へ動かした box の center/orientation でボムの Cut を呼ぶ。
    /// 「オフセット用 Overlap が球と交差するか」に頼ると、後ろへずらすほど交差しなくなりバニラフォールバックだけになりがちなので、
    /// バニラ Overlap でボムを拾えた場合も okOffset なら常にオフセット box のパラメータで Cut する。
    /// </summary>
    [HarmonyPatch(typeof(NoteCutter), nameof(NoteCutter.Cut))]
    [HarmonyPriority(int.MaxValue)]
    internal static class NoteCutterLegacyBombBladeCollision
    {
        /// <summary>ボム用 Overlap のブレード点を水平後方へずらす距離（m）。固定値。</summary>
        private const float LegacyBombBladeSweepBackMeters = 2f;

        private static readonly FieldInfo CollidersField = AccessTools.Field(typeof(NoteCutter), "_colliders");
        private static readonly FieldInfo SortParamsField = AccessTools.Field(typeof(NoteCutter), "_cuttableBySaberSortParams");
        private static readonly FieldInfo ComparerField = AccessTools.Field(typeof(NoteCutter), "_comparer");

        /// <summary>プレイ中は <see cref="Camera.main"/> が未設定のことが多く、オフセットが常に 0 になるのを防ぐためキャッシュする。</summary>
        private static Camera _cachedGameplayCamera;

        private static bool _bombTypesResolved;

        /// <summary>1.40 前後で名前空間・型名が変わっても拾うため、Main から解決する。</summary>
        private static Type _sphereCuttableBySaberType;

        private static Type _bombNoteControllerType;

        private struct HitParams
        {
            public Vector3 Center;
            public Quaternion Orientation;
            public Vector3 CutVec;
            public Vector3 TopForDistance;
        }

        private static bool Prefix(NoteCutter __instance, Saber saber)
        {
            if (PluginConfig.Instance == null)
            {
                return true;
            }

            if (!PluginConfig.Instance.legacyBombBladeHitbox)
            {
                return true;
            }

            if (CollidersField == null || SortParamsField == null || ComparerField == null)
            {
                return true;
            }

            var collidersBuffer = CollidersField.GetValue(__instance) as Collider[];
            var sortParams = SortParamsField.GetValue(__instance) as Array;
            var comparer = ComparerField.GetValue(__instance) as IComparer;
            if (collidersBuffer == null || sortParams == null || comparer == null)
            {
                return true;
            }

            Vector3 saberTop = saber.saberBladeTopPosForLogic;
            Vector3 saberBottom = saber.saberBladeBottomPosForLogic;
            BladeMovementDataElement prevAddedData = saber.movementDataForLogic.prevAddedData;
            Vector3 prevTop = prevAddedData.topPos;
            Vector3 prevBottom = prevAddedData.bottomPos;
            Vector3 prevMid = (prevBottom + prevTop) * 0.5f;

            Vector3 saberTopO = saberTop;
            Vector3 saberBottomO = saberBottom;
            Vector3 prevTopO = prevTop;
            Vector3 prevBottomO = prevBottom;
            ApplyLegacySweepPlacementOffset(saber, ref saberBottomO, ref saberTopO);
            ApplyLegacySweepPlacementOffset(saber, ref prevBottomO, ref prevTopO);
            Vector3 prevMidO = (prevBottomO + prevTopO) * 0.5f;

            bool okVanilla = ThreePointsToBox(saberTop, saberBottom, prevMid, out Vector3 centerV, out Vector3 halfV, out Quaternion orientV);
            bool okOffset = ThreePointsToBox(saberTopO, saberBottomO, prevMidO, out Vector3 centerO, out Vector3 halfO, out Quaternion orientO);

            if (!okVanilla && !okOffset)
            {
                return false;
            }

            int nVanilla = 0;
            if (okVanilla)
            {
                nVanilla = Physics.OverlapBoxNonAlloc(centerV, halfV, collidersBuffer, orientV, LayerMasks.noteLayerMask);
            }

            int nOffset = 0;
            var offsetBuffer = new Collider[collidersBuffer.Length];
            if (okOffset)
            {
                nOffset = Physics.OverlapBoxNonAlloc(centerO, halfO, offsetBuffer, orientO, LayerMasks.noteLayerMask);
            }

            Vector3 cutVecV = saberTop - prevTop;
            Vector3 cutVecO = saberTopO - prevTopO;

            var hitParamsByCuttable = new Dictionary<CuttableBySaber, HitParams>(16);
            var orderedCuttables = new List<CuttableBySaber>(16);

            void Register(CuttableBySaber c, in HitParams hp)
            {
                if (c == null || hitParamsByCuttable.ContainsKey(c))
                {
                    return;
                }

                hitParamsByCuttable[c] = hp;
                orderedCuttables.Add(c);
            }

            // ノーツ等: バニラ Overlap のみ（ボムは除外）
            for (int i = 0; i < nVanilla; i++)
            {
                Collider col = collidersBuffer[i];
                if (col == null)
                {
                    continue;
                }

                CuttableBySaber c = col.gameObject.GetComponent<CuttableBySaber>();
                if (c == null || IsBombCuttable(c))
                {
                    continue;
                }

                Register(c, new HitParams
                {
                    Center = centerV,
                    Orientation = orientV,
                    CutVec = cutVecV,
                    TopForDistance = prevTop,
                });
            }

            // ボム: バニラ Overlap で拾えたら「ヒットした箱」ではなく、後ろにずらしたブレードで作った箱の Cut パラメータを使う。
            // （オフセット用 Overlap が球と交差しないとフォールバックでバニラと同じになり、体感が一切変わらない）
            for (int i = 0; i < nVanilla; i++)
            {
                Collider col = collidersBuffer[i];
                if (col == null)
                {
                    continue;
                }

                CuttableBySaber c = col.gameObject.GetComponent<CuttableBySaber>();
                if (c == null || !IsBombCuttable(c))
                {
                    continue;
                }

                HitParams bombHp;
                if (okOffset)
                {
                    bombHp = new HitParams
                    {
                        Center = centerO,
                        Orientation = orientO,
                        CutVec = cutVecO,
                        TopForDistance = prevTopO,
                    };
                }
                else
                {
                    bombHp = new HitParams
                    {
                        Center = centerV,
                        Orientation = orientV,
                        CutVec = cutVecV,
                        TopForDistance = prevTop,
                    };
                }

                Register(c, bombHp);
            }

            // バニラに無くオフセット Overlap だけが拾ったボム（稀）
            for (int i = 0; i < nOffset; i++)
            {
                Collider col = offsetBuffer[i];
                if (col == null)
                {
                    continue;
                }

                CuttableBySaber c = col.gameObject.GetComponent<CuttableBySaber>();
                if (c == null || !IsBombCuttable(c) || hitParamsByCuttable.ContainsKey(c))
                {
                    continue;
                }

                Register(c, new HitParams
                {
                    Center = centerO,
                    Orientation = orientO,
                    CutVec = cutVecO,
                    TopForDistance = prevTopO,
                });
            }

            int num = orderedCuttables.Count;
            if (num == 0)
            {
                return false;
            }

            for (int i = 0; i < num; i++)
            {
                CuttableBySaber component = orderedCuttables[i];
                Vector3 position = component.transform.position;
                HitParams hp = hitParamsByCuttable[component];
                var te = Traverse.Create(sortParams.GetValue(i));
                te.Field<CuttableBySaber>("cuttableBySaber").Value = component;
                te.Field<float>("distance").Value = (hp.TopForDistance - position).sqrMagnitude - component.radius * component.radius;
                te.Field<Vector3>("pos").Value = position;
            }

            if (num == 1)
            {
                CuttableBySaber c = orderedCuttables[0];
                HitParams hp = hitParamsByCuttable[c];
                c.Cut(saber, hp.Center, hp.Orientation, hp.CutVec);
                return false;
            }

            if (num == 2)
            {
                object a0 = sortParams.GetValue(0);
                object a1 = sortParams.GetValue(1);

                void DoCut(object elem)
                {
                    CuttableBySaber c = Traverse.Create(elem).Field<CuttableBySaber>("cuttableBySaber").Value;
                    HitParams hp = hitParamsByCuttable[c];
                    c.Cut(saber, hp.Center, hp.Orientation, hp.CutVec);
                }

                if (comparer.Compare(a0, a1) > 0)
                {
                    DoCut(a0);
                    DoCut(a1);
                }
                else
                {
                    DoCut(a1);
                    DoCut(a0);
                }

                return false;
            }

            Array.Sort(sortParams, 0, num, comparer);
            for (int j = 0; j < num; j++)
            {
                CuttableBySaber c = Traverse.Create(sortParams.GetValue(j)).Field<CuttableBySaber>("cuttableBySaber").Value;
                HitParams hp = hitParamsByCuttable[c];
                c.Cut(saber, hp.Center, hp.Orientation, hp.CutVec);
            }

            return false;
        }

        private static void EnsureBombRelatedTypes()
        {
            if (_bombTypesResolved)
            {
                return;
            }

            _bombTypesResolved = true;
            _sphereCuttableBySaberType = AccessTools.TypeByName("SphereCuttableBySaber");
            _bombNoteControllerType = AccessTools.TypeByName("BombNoteController");

            foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!string.Equals(asm.GetName().Name, "Main", StringComparison.Ordinal))
                {
                    continue;
                }

                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException e)
                {
                    Type[] loaded = e.Types;
                    var list = new List<Type>(loaded != null ? loaded.Length : 0);
                    if (loaded != null)
                    {
                        for (int i = 0; i < loaded.Length; i++)
                        {
                            if (loaded[i] != null)
                            {
                                list.Add(loaded[i]);
                            }
                        }
                    }

                    types = list.ToArray();
                }

                for (int i = 0; i < types.Length; i++)
                {
                    Type t = types[i];
                    if (t == null)
                    {
                        continue;
                    }

                    if (_sphereCuttableBySaberType == null && t.Name == "SphereCuttableBySaber" && typeof(CuttableBySaber).IsAssignableFrom(t))
                    {
                        _sphereCuttableBySaberType = t;
                    }

                    if (_bombNoteControllerType == null && t.Name == "BombNoteController")
                    {
                        _bombNoteControllerType = t;
                    }
                }

                break;
            }

            if (_sphereCuttableBySaberType == null || _bombNoteControllerType == null)
            {
                foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                {
                    try
                    {
                        if (_sphereCuttableBySaberType == null)
                        {
                            Type t = asm.GetType("SphereCuttableBySaber");
                            if (t != null && typeof(CuttableBySaber).IsAssignableFrom(t))
                            {
                                _sphereCuttableBySaberType = t;
                            }
                        }

                        if (_bombNoteControllerType == null)
                        {
                            Type t = asm.GetType("BombNoteController");
                            if (t != null)
                            {
                                _bombNoteControllerType = t;
                            }
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    if (_sphereCuttableBySaberType != null && _bombNoteControllerType != null)
                    {
                        break;
                    }
                }
            }
        }

        private static bool IsBombCuttable(CuttableBySaber c)
        {
            if (c == null)
            {
                return false;
            }

            EnsureBombRelatedTypes();

            Type t = c.GetType();
            if (_sphereCuttableBySaberType != null && _sphereCuttableBySaberType.IsAssignableFrom(t))
            {
                return true;
            }

            if (_bombNoteControllerType != null)
            {
                if (c.gameObject.GetComponent(_bombNoteControllerType) != null)
                {
                    return true;
                }

                if (c.transform.GetComponentInParent(_bombNoteControllerType) != null)
                {
                    return true;
                }
            }

            return t.Name == "SphereCuttableBySaber";
        }

        private static void ApplyLegacySweepPlacementOffset(Saber saber, ref Vector3 bladeBottom, ref Vector3 bladeTop)
        {
            Vector3 delta = HorizontalBackwardFromPlayer(saber);
            bladeBottom += delta;
            bladeTop += delta;
        }

        /// <summary>HMD の水平前方の逆。Camera.main が無いときはタグ付きカメラ・全カメラ列挙、最後にセイバー親の forward で代用。</summary>
        private static Vector3 HorizontalBackwardFromPlayer(Saber saber)
        {
            float meters = LegacyBombBladeSweepBackMeters;
            if (meters <= 0f)
            {
                return Vector3.zero;
            }

            Vector3 horizontalForward = Vector3.zero;
            Camera cam = ResolveGameplayCamera();
            if (cam != null)
            {
                Vector3 f = cam.transform.forward;
                f.y = 0f;
                if (f.sqrMagnitude > 1e-8f)
                {
                    horizontalForward = f.normalized;
                }
            }
            else if (saber != null && saber.transform != null)
            {
                // プレイカメラが取れないとき: コントローラ系の親の forward（体の向きの近似）
                Transform t = saber.transform;
                for (int d = 0; d < 6 && t != null; d++)
                {
                    Vector3 f = t.forward;
                    f.y = 0f;
                    if (f.sqrMagnitude > 1e-8f)
                    {
                        horizontalForward = f.normalized;
                        break;
                    }

                    t = t.parent;
                }
            }

            if (horizontalForward.sqrMagnitude < 1e-8f)
            {
                return Vector3.zero;
            }

            return -horizontalForward * meters;
        }

        private static Camera ResolveGameplayCamera()
        {
            if (_cachedGameplayCamera != null && _cachedGameplayCamera && _cachedGameplayCamera.enabled && _cachedGameplayCamera.gameObject.activeInHierarchy)
            {
                return _cachedGameplayCamera;
            }

            _cachedGameplayCamera = null;

            Camera main = Camera.main;
            if (main != null)
            {
                _cachedGameplayCamera = main;
                return _cachedGameplayCamera;
            }

            GameObject tagged = GameObject.FindGameObjectWithTag("MainCamera");
            if (tagged != null)
            {
                Camera c = tagged.GetComponent<Camera>();
                if (c != null)
                {
                    _cachedGameplayCamera = c;
                    return _cachedGameplayCamera;
                }
            }

            int count = Camera.allCamerasCount;
            if (count > 0)
            {
                var buf = new Camera[count];
                int n = Camera.GetAllCameras(buf);
                for (int i = 0; i < n; i++)
                {
                    Camera c = buf[i];
                    if (c == null || !c.enabled || !c.gameObject.scene.IsValid())
                    {
                        continue;
                    }

                    if (c.stereoEnabled)
                    {
                        _cachedGameplayCamera = c;
                        return _cachedGameplayCamera;
                    }
                }

                for (int i = 0; i < n; i++)
                {
                    Camera c = buf[i];
                    if (c != null && c.enabled && c.gameObject.scene.IsValid())
                    {
                        _cachedGameplayCamera = c;
                        return _cachedGameplayCamera;
                    }
                }
            }

            return null;
        }

        private static bool ThreePointsToBox(Vector3 p0, Vector3 p1, Vector3 p2, out Vector3 center, out Vector3 halfSize, out Quaternion orientation)
        {
            Vector3 normalized = Vector3.Cross(p1 - p2, p0 - p2).normalized;
            if (normalized.sqrMagnitude > 1E-05f)
            {
                Vector3 normalized2 = (p0 - p1).normalized;
                Vector3 vector = Vector3.Cross(normalized2, normalized);
                orientation = default;
                orientation.SetLookRotation(normalized2, normalized);
                float num = Mathf.Abs(new Plane(vector, p0).GetDistanceToPoint(p2));
                float num2 = Vector3.Magnitude(p0 - p1);
                Vector3 vector2 = (p0 + p1) * 0.5f;
                center = vector2 - vector * num * 0.5f;
                halfSize = new Vector3(num * 0.5f, 0f, num2 * 0.5f);
                return true;
            }

            center = Vector3.zero;
            halfSize = Vector3.zero;
            orientation = Quaternion.identity;
            return false;
        }
    }
}
