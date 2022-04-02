using HarmonyLib;
using IPA.Utilities;
using NoteMode.Configuration;
using NoteMode.Utilities;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataTransformHelper), "CreateTransformedBeatmapData")]
    internal static class BeatmapDataTransformHelperCreateTransformedBeatmapData
    {
        private static PluginConfig conf = PluginConfig.Instance;
        private static void Prefix(ref IReadonlyBeatmapData beatmapData)
        {
            var copy = beatmapData.GetCopy();
            if (conf.arcMode || conf.changeChainNotes)
            {
                var beatmapObjectDataItems = copy.allBeatmapDataItems.Where(x => x is NoteData).Select(x => x as NoteData).ToArray();

                foreach (NoteData noteData in beatmapObjectDataItems)
                {
                    if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                    {
                        NoteData nextNoteData = SliderUtil.NextNoteData(noteData, beatmapObjectDataItems);

                        if (conf.arcMode && nextNoteData != null)
                        {
                            if (conf.noArrow)
                            {
                                if (conf.oneColorRed)
                                {
                                    SliderData sliderDataAny = SliderUtil.CreateAnySliderData(noteData, nextNoteData, ColorType.ColorA);
                                    copy.AddBeatmapObjectData(sliderDataAny);
                                }
                                else if (conf.oneColorBlue)
                                {
                                    SliderData sliderDataAny = SliderUtil.CreateAnySliderData(noteData, nextNoteData, ColorType.ColorB);
                                    copy.AddBeatmapObjectData(sliderDataAny);
                                }
                                else
                                {
                                    if (noteData.colorType == nextNoteData.colorType)
                                    {
                                        SliderData sliderDataAny = SliderUtil.CreateAnySliderData(noteData, nextNoteData, noteData.colorType);
                                        copy.AddBeatmapObjectData(sliderDataAny);
                                    }
                                }
                            }
                            else
                            {
                                if (noteData.cutDirection != NoteCutDirection.Any && noteData.colorType == nextNoteData.colorType)
                                {
                                    SliderData sliderData = SliderUtil.CreateSliderData(noteData, nextNoteData, noteData.colorType);
                                    copy.AddBeatmapObjectData(sliderData);
                                }
                            }
                        }

                        /*if (PluginConfig.Instance.arcMode || PluginConfig.Instance.changeChainNotes)
                        {
                            var noteDataItems = beatmapObjectDataItems.Where(x => x is NoteData).Select(x => x as NoteData).ToArray();
                            int matchCount = 0;
                            NoteData noteData2 = null;
                            foreach (NoteData noteData1 in noteDataItems)
                            {
                                if (noteData.time == noteData1.time)
                                {
                                    matchCount++;
                                }
                                if (PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue)
                                {
                                    if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time)
                                    {
                                        noteData2 = noteData1;
                                        break;
                                    }
                                }
                                else
                                {
                                    if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time && noteData.colorType == noteData1.colorType)
                                    {
                                        noteData2 = noteData1;
                                        break;
                                    }
                                }
                            }

                            if (noteData.cutDirection != NoteCutDirection.None)
                            {
                                if (PluginConfig.Instance.arcMode && noteData2 != null)
                                {
                                    SliderMidAnchorMode sliderMidAnchorMode = SliderMidAnchorMode.Straight;
                                    if (noteData.cutDirection != NoteCutDirection.Any)
                                    {
                                        if (noteData.noteLineLayer == NoteLineLayer.Base && noteData2.noteLineLayer == NoteLineLayer.Base)
                                        {
                                            sliderMidAnchorMode = SliderMidAnchorMode.Straight;
                                        }
                                        else if (noteData.noteLineLayer == NoteLineLayer.Upper && noteData2.noteLineLayer == NoteLineLayer.Upper)
                                        {
                                            sliderMidAnchorMode = SliderMidAnchorMode.CounterClockwise;
                                        }
                                        else if (noteData.noteLineLayer == NoteLineLayer.Top && noteData2.noteLineLayer == NoteLineLayer.Top)
                                        {
                                            sliderMidAnchorMode = SliderMidAnchorMode.Clockwise;
                                        }
                                    }

                                    NoteCutDirection noteCutDirection = noteData.cutDirection;
                                    NoteCutDirection nextNoteCutDirection = noteData2.cutDirection;
                                    if (PluginConfig.Instance.noArrow)
                                    {
                                        noteCutDirection = NoteCutDirection.Any;
                                        nextNoteCutDirection = NoteCutDirection.Any;
                                    }
                                    if ((PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue) && noteData.colorType != ColorType.None)
                                    {
                                        ColorType colorType = noteData.colorType;
                                        if (PluginConfig.Instance.oneColorRed)
                                        {
                                            colorType = ColorType.ColorA;
                                        }
                                        else if (PluginConfig.Instance.oneColorBlue)
                                        {
                                            colorType = ColorType.ColorB;
                                        }

                                        SliderData sliderData1 = SliderData.CreateSliderData(
                                            colorType,
                                            noteData.time,
                                            noteData.lineIndex,
                                            noteData.noteLineLayer,
                                            noteData.beforeJumpNoteLineLayer,
                                            0.5f,
                                            noteCutDirection,
                                            noteData.time + noteData.timeToNextColorNote,
                                            noteData2.lineIndex,
                                            noteData2.noteLineLayer,
                                            noteData2.noteLineLayer,
                                            1f,
                                            nextNoteCutDirection,
                                            sliderMidAnchorMode
                                        );
                                        copy.AddBeatmapObjectData(sliderData1);
                                    }
                                    else
                                    {
                                        if (noteData.colorType == noteData2.colorType)
                                        {
                                            SliderData sliderData1 = SliderData.CreateSliderData(
                                                noteData.colorType,
                                                noteData.time,
                                                noteData.lineIndex,
                                                noteData.noteLineLayer,
                                                noteData.beforeJumpNoteLineLayer,
                                                0.5f,
                                                noteCutDirection,
                                                noteData.time + noteData.timeToNextColorNote,
                                                noteData2.lineIndex,
                                                noteData2.noteLineLayer,
                                                noteData2.noteLineLayer,
                                                1f,
                                                nextNoteCutDirection,
                                                sliderMidAnchorMode
                                            );
                                            copy.AddBeatmapObjectData(sliderData1);
                                        }
                                    }
                                }

                                if (PluginConfig.Instance.changeChainNotes && noteData2 != null)
                                {

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

                                    if (isTailBeforeJumpLineLayer && tailLineIndex != -1)
                                    {
                                        float tailtime = (noteData.timeToNextColorNote * 0.5f) / 2f;
                                        if (noteData.cutDirection != noteData2.cutDirection || noteData2.cutDirection != NoteCutDirection.None)
                                        {
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
                                }

                            }
                        }*/

                    }
                }

            }
            beatmapData = copy;
        }
    }
}