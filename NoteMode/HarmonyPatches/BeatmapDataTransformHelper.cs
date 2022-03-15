using HarmonyLib;
using IPA.Utilities;
using NoteMode.Configuration;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataTransformHelper), "CreateTransformedBeatmapData")]
    internal static class BeatmapDataTransformHelperCreateTransformedBeatmapData
    {
        private static PropertyInfo s_colorTypeProperty;
        public static PropertyInfo ColorTypeProperty => s_colorTypeProperty ?? (s_colorTypeProperty = typeof(NoteData).GetProperty("colorType"));

        private static PropertyInfo s_gameplayType;
        public static PropertyInfo GameplayTypeProperty => s_gameplayType ?? (s_gameplayType = typeof(NoteData).GetProperty("gameplayType"));

        private static PropertyInfo slider_colorTypeProperty;
        public static PropertyInfo SliderColorTypeProperty => slider_colorTypeProperty ?? (slider_colorTypeProperty = typeof(SliderData).GetProperty("colorType"));

        private static void SetNoteColorType(NoteData noteData, ColorType colorType)
        {
            BeatmapDataTransformHelperCreateTransformedBeatmapData.ColorTypeProperty.GetSetMethod(true).Invoke(noteData, new object[]
            {
                colorType
            });
        }

        private static void SetGameplayType(NoteData noteData, NoteData.GameplayType gameplayType)
        {
            BeatmapDataTransformHelperCreateTransformedBeatmapData.GameplayTypeProperty.GetSetMethod(true).Invoke(noteData, new object[]
            {
                gameplayType
            });
        }

        private static void SetSliderColorType(SliderData sliderData, ColorType colorType)
        {
            BeatmapDataTransformHelperCreateTransformedBeatmapData.SliderColorTypeProperty.GetSetMethod(true).Invoke(sliderData, new object[]
            {
                colorType
            });
        }

        private static void SwitchNoteColorType(NoteData noteData)
        {
            ColorType colorType = (ColorType)BeatmapDataTransformHelperCreateTransformedBeatmapData.ColorTypeProperty.GetValue(noteData);
            if (colorType == ColorType.ColorA)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetNoteColorType(noteData, ColorType.ColorB);
                return;
            }
            if (colorType == ColorType.ColorB)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetNoteColorType(noteData, ColorType.ColorA);
            }
        }

        private static void SwitchGameplayType(NoteData noteData)
        {
            NoteData.GameplayType gameplayType = (NoteData.GameplayType)BeatmapDataTransformHelperCreateTransformedBeatmapData.GameplayTypeProperty.GetValue("gameplayType");
                Plugin.Log.Debug($"NoteData: {noteData.gameplayType}");
            if (noteData.gameplayType == NoteData.GameplayType.Normal)
            {
                Plugin.Log.Debug($"NoteData: Normal");
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetGameplayType(noteData, NoteData.GameplayType.BurstSliderHead);
            }
        }

        private static void SwitchSliderColorType(SliderData sliderData)
        {
            ColorType colorType = (ColorType)BeatmapDataTransformHelperCreateTransformedBeatmapData.SliderColorTypeProperty.GetValue(sliderData);
            if (colorType == ColorType.ColorA)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetSliderColorType(sliderData, ColorType.ColorB);
                return;
            }
            if (colorType == ColorType.ColorB)
            {
                BeatmapDataTransformHelperCreateTransformedBeatmapData.SetSliderColorType(sliderData, ColorType.ColorA);
            }
        }

        private static void SwitchNoteCutDirection(NoteData noteData)
        {
            switch (noteData.cutDirection)
            {
                case NoteCutDirection.Left:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.Right);
                    break;
                case NoteCutDirection.Right:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.Left);

                    break;
                case NoteCutDirection.Up:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.Down);
                    break;
                case NoteCutDirection.Down:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.Up);
                    //noteData.SetNonPublicProperty("noteLineLayer", 1);//動作する
                    //noteData.SetNonPublicProperty("lineIndex", 1);//動作しない
                    break;
                case NoteCutDirection.UpLeft:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    break;
                case NoteCutDirection.UpRight:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                    break;
                case NoteCutDirection.DownLeft:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                    break;
                case NoteCutDirection.DownRight:
                    noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                    break;
                default:
                    break;
            }
        }
        private static void RandomizeNoteCutDirection(NoteData noteData)
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond+(int)(noteData.time*1000)+(int)noteData.noteLineLayer+noteData.lineIndex);
            int rand = UnityEngine.Random.Range(0, 3);

            switch (noteData.cutDirection)
            {
                case NoteCutDirection.Left:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                    break;
                case NoteCutDirection.Right:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    break;
                case NoteCutDirection.Up:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                    break;
                case NoteCutDirection.Down:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    break;
                case NoteCutDirection.UpLeft:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Up);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.Left);
                    break;
                case NoteCutDirection.UpRight:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Up);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.Right);
                    break;
                case NoteCutDirection.DownLeft:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Down);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.Left);
                    break;
                case NoteCutDirection.DownRight:
                    if (rand == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Down);
                    if (rand == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.Right);
                    break;
                default:
                    break;
            }
        }

        private static void RestrictedRandomizeNoteCutDirection(NoteData noteData)
        {
            UnityEngine.Random.InitState(DateTime.Now.Millisecond + (int)(noteData.time * 1000));
            int rand3 = UnityEngine.Random.Range(0, 3);
            int rand2 = UnityEngine.Random.Range(0, 2);
            switch (noteData.cutDirection)
            {
                case NoteCutDirection.Left:
                    if ((int)noteData.noteLineLayer == 0)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                    }
                    else if ((int)noteData.noteLineLayer == 2)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                    }
                    else
                    {
                        //if (rand3 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                        //if (rand3 == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                    }
                    break;
                case NoteCutDirection.Right:
                    if ((int)noteData.noteLineLayer == 0)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    }
                    else if ((int)noteData.noteLineLayer == 2)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                    }
                    else
                    {
                        //if (rand3 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                        //if (rand3 == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    }
                    break;
                case NoteCutDirection.Up:
                    if (noteData.lineIndex == 0)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                    }
                    else if (noteData.lineIndex == 3)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                    }
                    else
                    {
                        if (rand3 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.UpLeft);
                        if (rand3 == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.UpRight);
                    }
                    break;
                case NoteCutDirection.Down:
                    if (noteData.lineIndex == 0)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                    }
                    else if (noteData.lineIndex == 3)
                    {
                        if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    }
                    else
                    {
                        if (rand3 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.DownLeft);
                        if (rand3 == 2) noteData.ChangeNoteCutDirection(NoteCutDirection.DownRight);
                    }
                    break;
                case NoteCutDirection.UpLeft:
                    if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Up);
                    break;
                case NoteCutDirection.UpRight:
                    if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Up);
                    break;
                case NoteCutDirection.DownLeft:
                    if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Down);
                    break;
                case NoteCutDirection.DownRight:
                    if (rand2 == 1) noteData.ChangeNoteCutDirection(NoteCutDirection.Down);
                    break;
                default:
                    break;
            }
            if ((int)noteData.noteLineLayer == 2)
            {
                //noteData.ChangeNoteCutDirection(NoteCutDirection.Any);
            }
            if (noteData.lineIndex == 3)
            {
                //noteData.ChangeNoteCutDirection(NoteCutDirection.Any);
            }
        }

        private static SliderData sliderData2 = null;
        private static SliderData sliderData3 = null;
        [HarmonyPriority(600)]
        private static void Prefix(ref IReadonlyBeatmapData beatmapData, IPreviewBeatmapLevel beatmapLevel, GameplayModifiers gameplayModifiers, bool leftHanded, EnvironmentEffectsFilterPreset environmentEffectsFilterPreset, EnvironmentIntensityReductionOptions environmentIntensityReductionOptions, MainSettingsModelSO mainSettingsModel)
        {
            /*if (!PluginConfig.Instance.noArrow && !PluginConfig.Instance.oneColorBlue && !PluginConfig.Instance.oneColorRed && !PluginConfig.Instance.reverseArrows && !PluginConfig.Instance.randomizeArrows && !PluginConfig.Instance.restrictedrandomizeArrows)
            {
                return;
            }*/

            BeatmapData copy = beatmapData.GetCopy();
            var beatmapObjectDataItems = copy.allBeatmapDataItems.Where(x => x is BeatmapObjectData).Select(x => x as BeatmapObjectData).ToArray();
            
            foreach (BeatmapObjectData beatmapObjectData in beatmapObjectDataItems)
            {
                NoteData noteData = beatmapObjectData as NoteData;
                SliderData sliderData = beatmapObjectData as SliderData;
                if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                {
                    /*if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        if (BeatmapDataTransformHelperCreateTransformedBeatmapData.sliderData2 == null)
                        {
                            SliderData sliderData2 = new SliderData(SliderData.Type.Normal, ColorType.ColorA, false, 0, 0, NoteLineLayer.Base, NoteLineLayer.Base, 0.5f, NoteCutDirection.Down, 0f, false, beatmapObjectData.time, 0, NoteLineLayer.Upper, NoteLineLayer.Upper, 1.5f, NoteCutDirection.Up, 0f, SliderMidAnchorMode.Straight, 0, 1f);
                            copy.AddBeatmapObjectData(sliderData2);
                        }
                        if (BeatmapDataTransformHelperCreateTransformedBeatmapData.sliderData3 == null)
                        {
                            SliderData sliderData3 = new SliderData(SliderData.Type.Normal, ColorType.ColorB, false, 0, 4, NoteLineLayer.Base, NoteLineLayer.Base, 0.5f, NoteCutDirection.Down, 0f, false, beatmapObjectData.time, 4, NoteLineLayer.Upper, NoteLineLayer.Upper, 1.5f, NoteCutDirection.Up, 0f, SliderMidAnchorMode.Straight, 0, 1f);
                            copy.AddBeatmapObjectData(sliderData3);
                        }

                    }*/

                    var noteDataItems = beatmapObjectDataItems.Where(x => x is NoteData).Select(x => x as NoteData).ToArray();
                    int matchCount = 0;
                    NoteData noteData2 = null;
                    foreach (NoteData noteData1 in noteDataItems)
                    {
                        if (noteData.time == noteData1.time)
                        {
                            matchCount++;
                        }
                        if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time)
                        {
                            noteData2 = noteData1;
                        }
                    }
                    
                    if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        if (noteData2 != null)
                        {
                            if (noteData.colorType == noteData2.colorType)
                            {
                                SliderMidAnchorMode sliderMidAnchorMode = SliderMidAnchorMode.Straight;
                                if (noteData.noteLineLayer == NoteLineLayer.Base && noteData2.noteLineLayer == NoteLineLayer.Base)
                                {
                                    sliderMidAnchorMode = SliderMidAnchorMode.Clockwise;
                                }
                                else if (noteData.noteLineLayer == NoteLineLayer.Upper && noteData2.noteLineLayer == NoteLineLayer.Upper)
                                {
                                    sliderMidAnchorMode = SliderMidAnchorMode.Clockwise;
                                }
                                else if (noteData.noteLineLayer == NoteLineLayer.Top && noteData2.noteLineLayer == NoteLineLayer.Top)
                                {
                                    sliderMidAnchorMode = SliderMidAnchorMode.Clockwise;
                                }
                                SliderData sliderData1 = SliderData.CreateSliderData(
                                    noteData.colorType,
                                    noteData.time,
                                    noteData.lineIndex,
                                    noteData.noteLineLayer,
                                    noteData.beforeJumpNoteLineLayer,
                                    0.5f,
                                    noteData.cutDirection,
                                    noteData.time + noteData.timeToNextColorNote,
                                    noteData2.lineIndex,
                                    noteData2.noteLineLayer,
                                    noteData2.beforeJumpNoteLineLayer,
                                    1f,
                                    noteData2.cutDirection,
                                    sliderMidAnchorMode
                                );
                                copy.AddBeatmapObjectData(sliderData1);
                            }
                        }

                        NoteLineLayer tailBeforeJumpLineLayer = NoteLineLayer.Upper;
                        int tailLineIndex = -1;
                        int tailLineCount = 5;
                        bool isTailBeforeJumpLineLayer = false;
                        if (noteData.noteLineLayer == NoteLineLayer.Upper)
                        {
                            if (noteData.cutDirection == NoteCutDirection.Down || noteData.cutDirection == NoteCutDirection.DownRight || noteData.cutDirection == NoteCutDirection.DownLeft)
                            {
                                tailBeforeJumpLineLayer = NoteLineLayer.Base;
                                isTailBeforeJumpLineLayer = true;
                            }
                            else if (noteData.cutDirection == NoteCutDirection.Up || noteData.cutDirection == NoteCutDirection.UpRight || noteData.cutDirection == NoteCutDirection.UpLeft)
                            {
                                tailBeforeJumpLineLayer = NoteLineLayer.Top;
                                isTailBeforeJumpLineLayer = true;
                            }
                            else
                            {
                                isTailBeforeJumpLineLayer = false;
                            }
                            if (noteData.cutDirection == NoteCutDirection.Down)
                            {
                                tailLineCount = 3;
                                if (noteData.lineIndex == 0)
                                {
                                    tailLineIndex = 0;
                                }
                                else if (noteData.lineIndex == 3)
                                {
                                    tailLineIndex = 3;
                                }
                            }
                            else if (noteData.cutDirection == NoteCutDirection.DownRight)
                            {
                                if (noteData.lineIndex == 0)
                                {
                                    tailLineCount = 8;
                                    tailLineIndex = 2;
                                }
                            }
                            else if (noteData.cutDirection == NoteCutDirection.DownLeft)
                            {
                                if (noteData.lineIndex == 3)
                                {
                                    tailLineCount = 8;
                                    tailLineIndex = 1;
                                }
                            }
                            if (noteData.cutDirection == NoteCutDirection.Up)
                            {
                                tailLineCount = 3;
                                if (noteData.lineIndex == 0)
                                {
                                    tailLineIndex = 0;
                                }
                                else if (noteData.lineIndex == 3)
                                {
                                    tailLineIndex = 3;
                                }
                            }
                            else if (noteData.cutDirection == NoteCutDirection.UpRight)
                            {
                                if (noteData.lineIndex == 0)
                                {
                                    tailLineCount = 8;
                                    tailLineIndex = 2;
                                }
                            }
                            else if (noteData.cutDirection == NoteCutDirection.UpLeft)
                            {
                                if (noteData.lineIndex == 3)
                                {
                                    tailLineCount = 8;
                                    tailLineIndex = 1;
                                }
                            }

                        }
                        
                        Logger.log.Debug($"time: {noteData.time}, nextTime: {noteData.timeToNextColorNote}, prevTime: {noteData.timeToPrevColorNote}");
                        if (isTailBeforeJumpLineLayer && tailLineIndex != -1)
                        {
                            float tailtime = (noteData.timeToNextColorNote * 0.5f) / 2f;
                            if (matchCount < 2)
                            {

                                noteData.ChangeToBurstSliderHead();
                                SliderData burstSlider = SliderData.CreateBurstSliderData(
                                    noteData.colorType,
                                    noteData.time,
                                    noteData.lineIndex,
                                    noteData.noteLineLayer,
                                    noteData.beforeJumpNoteLineLayer,
                                    noteData.cutDirection,
                                    noteData.time + tailtime,
                                    tailLineIndex,
                                    tailBeforeJumpLineLayer,
                                    tailBeforeJumpLineLayer,
                                    NoteCutDirection.Any,
                                    tailLineCount,
                                    1f
                                );
                                copy.AddBeatmapObjectData(burstSlider);
                            }
                        }

                    }
                    if (PluginConfig.Instance.noArrow)
                    {
                        noteData.SetNoteToAnyCutDirection();
                    }

                    if ((noteData.gameplayType == NoteData.GameplayType.Normal) && PluginConfig.Instance.allBurstSliderHead)
                    {
                        noteData.ChangeToBurstSliderHead();
                    }

                    if ((noteData.colorType == ColorType.ColorA) && PluginConfig.Instance.noRed)
                    {
                        noteData.ChangeNoteCutDirection(NoteCutDirection.None);
                    }
                    else if ((noteData.colorType == ColorType.ColorB) && PluginConfig.Instance.noBlue)
                    {
                        noteData.ChangeNoteCutDirection(NoteCutDirection.None);
                    }

                    if (noteData.colorType == ColorType.ColorA && PluginConfig.Instance.oneColorBlue)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchNoteColorType(noteData);
                    }
                    else if (noteData.colorType == ColorType.ColorB && PluginConfig.Instance.oneColorRed)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchNoteColorType(noteData);
                    }

                    if (PluginConfig.Instance.reverseArrows)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchNoteCutDirection(noteData);
                    }

                    if (PluginConfig.Instance.randomizeArrows)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.RandomizeNoteCutDirection(noteData);
                    }

                    if (PluginConfig.Instance.restrictedrandomizeArrows)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.RestrictedRandomizeNoteCutDirection(noteData);
                    }
                }

                if (sliderData != null && sliderData.headCutDirection != NoteCutDirection.None)
                {
                    if (sliderData.colorType == ColorType.ColorA && PluginConfig.Instance.oneColorBlue)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchSliderColorType(sliderData);
                    }
                    else if (sliderData.colorType == ColorType.ColorB && PluginConfig.Instance.oneColorRed)
                    {
                        BeatmapDataTransformHelperCreateTransformedBeatmapData.SwitchSliderColorType(sliderData);
                    }
                }
            }
            beatmapData = copy;
            
        }

        
    }
}