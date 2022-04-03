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

        [Inject]
        public void Constractor(IAudioTimeSource audioSource, IReadonlyBeatmapData beatmapData, ColorScheme scheme)
        {
            this._source = audioSource;
            this._beatmapData = beatmapData;
            this._colorScheme = scheme;
        }

        private IAudioTimeSource _source;
        private IReadonlyBeatmapData _beatmapData;
        private ColorScheme _colorScheme;
        private DateTime _lastSendTime;
        private bool enable = false;

        private void Awake()
        {
            ColorManagerColorForTypePatch.LeftColor = this._colorScheme.saberAColor;
            ColorManagerColorForTypePatch.RightColor = this._colorScheme.saberBColor;

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

            this._lastSendTime = DateTime.Now;

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
                        noteData.ChangeNoteCutDirection(BeatmapUtil.SwitchNoteCutDirection(noteData.cutDirection));
                    }

                    if (conf.randomizeArrows)
                    {
                        noteData.ChangeNoteCutDirection(BeatmapUtil.RandomizeNoteCutDirection(noteData));
                    }

                    if (conf.restrictedrandomizeArrows)
                    {
                        noteData.ChangeNoteCutDirection(BeatmapUtil.RestrictedRandomizeNoteCutDirection(noteData));
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

        }

        private void Update()
        {
            if (!this.enable)
            {
                return;
            }
            if (this._source.songTime == 0)
            {
                this._lastSendTime = DateTime.Now;
                return;
            }

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

            if (conf.rainbowColor)
            {
                ColorManagerColorForTypePatch.LeftColor = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
                ColorManagerColorForTypePatch.RightColor = Color.HSVToRGB(UnityEngine.Random.Range(0f, 1f), 1f, 1f);
            }
        }

        /// <summary>
        /// Called every frame after every other enabled script's Update().
        /// </summary>
        private void LateUpdate()
        {

        }

        /// <summary>
        /// Called when the script becomes enabled and active
        /// </summary>
        private void OnEnable()
        {

        }

        /// <summary>
        /// Called when the script becomes disabled or when it is being destroyed.
        /// </summary>
        private void OnDisable()
        {

        }

        /// <summary>
        /// Called when the script is being destroyed.
        /// </summary>
        private void OnDestroy()
        {
            Plugin.Log?.Debug($"{name}: OnDestroy()");

        }
    }
}
