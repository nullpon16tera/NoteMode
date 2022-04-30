using NoteMode.Configuration;
using NoteMode.Utilities;
using NoteMode.HarmonyPatches;
using IPA.Loader;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Zenject;

namespace NoteMode.Controllers
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class BeatmapDataTransformController : MonoBehaviour
    {
        private PluginConfig conf = PluginConfig.Instance;

        
        private bool enable = false;
        public Color[] Colors => this._rainbow;
        public int LeftColorIndex { get; private set; }
        public int RightColorIndex { get; private set; }

        private void Awake()
        {
            ColorManagerColorForTypePatch.LeftColor = this._colorScheme.saberAColor;
            ColorManagerColorForTypePatch.RightColor = this._colorScheme.saberBColor;

            if (conf.oneColorRed)
            {
                ColorManagerColorForTypePatch.LeftColor = _colorScheme.saberAColor;
                ColorManagerColorForTypePatch.RightColor = _colorScheme.saberAColor;
            }
            if (conf.oneColorBlue)
            {
                ColorManagerColorForTypePatch.LeftColor = _colorScheme.saberBColor;
                ColorManagerColorForTypePatch.RightColor = _colorScheme.saberBColor;
            }

            if (this._beatmapData == null)
            {
                return;
            }
            this.enable = (
                conf.noRed || conf.noBlue || conf.oneColorRed || conf.oneColorBlue || conf.noArrow || conf.noNotesBomb ||
                conf.reverseArrows || conf.randomizeArrows || conf.restrictedrandomizeArrows ||
                conf.arcMode || conf.allBurstSliderHead || conf.changeChainNotes || conf.rainbowColor
            );
            
            if (!this.enable)
            {
                return;
            }

            var beatmapObjectDataItems = this._beatmapData.allBeatmapDataItems.Where(x => x is BeatmapObjectData).Select(x => x as BeatmapObjectData).ToArray();

            foreach (BeatmapObjectData beatmapObjectData in beatmapObjectDataItems)
            {
                var noteData = beatmapObjectData as NoteData;
                if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                {
                    if (conf.noArrow)
                    {
                        noteData.SetNoteToAnyCutDirection();
                    }

                    if (conf.noRed && (noteData.colorType == ColorType.ColorA))
                    {
                        noteData.ChangeNoteCutDirection(NoteCutDirection.None);
                    }
                    else if (conf.noBlue && (noteData.colorType == ColorType.ColorB))
                    {
                        noteData.ChangeNoteCutDirection(NoteCutDirection.None);
                    }

                    if (conf.reverseArrows)
                    {
                        noteData.ChangeNoteCutDirection(CutDirectionUtil.SwitchNoteCutDirection(noteData.cutDirection));
                    }

                    if (conf.randomizeArrows)
                    {
                        noteData.ChangeNoteCutDirection(CutDirectionUtil.RandomizeNoteCutDirection(noteData));
                    }

                    if (conf.restrictedrandomizeArrows)
                    {
                        noteData.ChangeNoteCutDirection(CutDirectionUtil.RestrictedRandomizeNoteCutDirection(noteData));
                    }

                    if (conf.allBurstSliderHead && (noteData.gameplayType == NoteData.GameplayType.Normal))
                    {
                        noteData.ChangeToBurstSliderHead();
                    }
                }
            }
        }

        private void Start()
        {
            ColorManagerColorForTypePatch.Enable = conf.rainbowColor ? !this._util.IsNoodle && !this._util.IsChroma : (conf.oneColorRed || conf.oneColorBlue);

            this._rainbow = new Color[s_colorCount];
            var tmp = 1f / s_colorCount;
            for (var i = 0; i < s_colorCount; i++)
            {
                var hue = tmp * i;
                this._rainbow[i] = Color.HSVToRGB(hue, 1f, 1f);
            }
        }

        private void Update()
        {
            if (!this.enable)
            {
                return;
            }


            if (conf.rainbowColor)
            {
                ColorManagerColorForTypePatch.LeftColor = Colors[this.LeftColorIndex];
                ColorManagerColorForTypePatch.RightColor = Colors[this.RightColorIndex];
            }
        }

        public void FixedUpdate()
        {
            this.LeftColorIndex = Time.frameCount % s_colorCount;
            this.RightColorIndex = (Time.frameCount + (s_colorCount / 2)) % s_colorCount;
        }

        private IReadonlyBeatmapData _beatmapData;
        private ColorScheme _colorScheme;
        private BeatmapUtil _util;
        private Color[] _rainbow;
        public const int s_colorCount = 256;

        [Inject]
        public void Constractor(IReadonlyBeatmapData beatmapData, ColorScheme scheme, BeatmapUtil util)
        {
            this._util = util;
            this._beatmapData = beatmapData;
            this._colorScheme = scheme;
        }
    }
}
