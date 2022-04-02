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
        public void Constractor(IReadonlyBeatmapData beatmapData)
        {
            this._beatmapData = beatmapData;
        }

        private IReadonlyBeatmapData _beatmapData;
        private PropertyInfo noteColorTypeProperty;
        private PropertyInfo sliderColorTypeProperty;
        private PropertyInfo gameplayTypeProperty;

        private void Awake()
        {
            if (this._beatmapData == null)
            {
                return;
            }
            bool confCheck = (
                conf.noRed || conf.noBlue || conf.oneColorRed || conf.oneColorBlue || conf.noArrow || conf.noNotesBomb ||
                conf.reverseArrows || conf.randomizeArrows || conf.restrictedrandomizeArrows ||
                conf.arcMode || conf.allBurstSliderHead || conf.changeChainNotes
            );
            
            if (!confCheck)
            {
                return;
            }
            

            this.noteColorTypeProperty = typeof(NoteData).GetProperty("colorType");
            this.sliderColorTypeProperty = typeof(SliderData).GetProperty("colorType");
            this.gameplayTypeProperty = typeof(NoteData).GetProperty("gameplayType");

            var beatmapObjectDataItems = this._beatmapData.allBeatmapDataItems.Where(x => x is BeatmapObjectData).Select(x => x as BeatmapObjectData).ToArray();

            foreach (BeatmapObjectData beatmapObjectData in beatmapObjectDataItems)
            {
                var noteData = beatmapObjectData as NoteData;
                var sliderData = beatmapObjectData as SliderData;
                if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                {
                    if (conf.noArrow)
                    {
                        noteData.SetNoteToAnyCutDirection();
                    }

                    if (conf.allBurstSliderHead && (noteData.gameplayType == NoteData.GameplayType.Normal))
                    {
                        noteData.ChangeToBurstSliderHead();
                    }

                    if (conf.noRed && (noteData.colorType == ColorType.ColorA))
                    {
                        noteData.ChangeNoteCutDirection(NoteCutDirection.None);
                    }
                    else if (conf.noBlue && (noteData.colorType == ColorType.ColorB))
                    {
                        noteData.ChangeNoteCutDirection(NoteCutDirection.None);
                    }

                    if (conf.oneColorBlue && (noteData.colorType == ColorType.ColorA))
                    {
                        this.SwitchNoteColorType(noteData);
                    }
                    else if (conf.oneColorRed && (noteData.colorType == ColorType.ColorB))
                    {
                        this.SwitchNoteColorType(noteData);
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
                }

                if (sliderData != null && sliderData.headCutDirection != NoteCutDirection.None)
                {
                    if (conf.oneColorBlue && sliderData.colorType == ColorType.ColorA)
                    {
                        this.SwitchSliderColorType(sliderData);
                    }
                    else if (conf.oneColorRed && sliderData.colorType == ColorType.ColorB)
                    {
                        this.SwitchSliderColorType(sliderData);
                    }
                }
            }
        }

        private void SetNoteColorType(NoteData noteData, ColorType colorType)
        {
            this.noteColorTypeProperty.GetSetMethod(true).Invoke(noteData, new object[]
            {
                colorType
            });
        }

        private void SetSliderColorType(SliderData sliderData, ColorType colorType)
        {
            this.sliderColorTypeProperty.GetSetMethod(true).Invoke(sliderData, new object[]
            {
                colorType
            });
        }

        private void SwitchNoteColorType(NoteData noteData)
        {
            ColorType colorType = (ColorType)this.noteColorTypeProperty.GetValue(noteData);
            if (colorType == ColorType.ColorA)
            {
                this.SetNoteColorType(noteData, ColorType.ColorB);
                return;
            }
            if (colorType == ColorType.ColorB)
            {
                this.SetNoteColorType(noteData, ColorType.ColorA);
            }
        }

        private void SwitchSliderColorType(SliderData sliderData)
        {
            ColorType colorType = (ColorType)this.sliderColorTypeProperty.GetValue(sliderData);
            if (colorType == ColorType.ColorA)
            {
                this.SetSliderColorType(sliderData, ColorType.ColorB);
                return;
            }
            if (colorType == ColorType.ColorB)
            {
                this.SetSliderColorType(sliderData, ColorType.ColorA);
            }
        }

        private void Start()
        {

        }

        private void Update()
        {

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
