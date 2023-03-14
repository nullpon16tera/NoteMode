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
    [HarmonyPriority(600)]
    internal static class BeatmapDataTransformHelperCreateTransformedBeatmapData
    {
        private static PluginConfig conf = PluginConfig.Instance;
        private static void Prefix(ref IReadonlyBeatmapData beatmapData)
        {
            var copy = beatmapData.GetCopy();


            if (conf.arcMode || conf.restrictedArcMode || conf.changeChainNotes)
            {
                var beatmapObjectDataItems = copy.allBeatmapDataItems.Where(x => x is NoteData).Select(x => x as NoteData).ToArray();


                foreach (NoteData noteData in beatmapObjectDataItems)
                {
                    if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                    {

                        NoteData nextNoteData = SliderUtil.NextNoteData(noteData, beatmapObjectDataItems);
                        

                        if ((conf.arcMode || conf.restrictedArcMode) && nextNoteData != null)
                        {
                            if (conf.noArrow || conf.oneColorRed || conf.oneColorBlue)
                            {
                                SliderData sliderDataAny = SliderUtil.CreateAnySliderData(noteData, nextNoteData, noteData.colorType);
                                copy.AddBeatmapObjectData(sliderDataAny);
                            }
                            else
                            {
                                if (nextNoteData.cutDirection != NoteCutDirection.Any && noteData.colorType == nextNoteData.colorType)
                                {
                                    SliderData sliderData = SliderUtil.CreateSliderData(noteData, nextNoteData, noteData.colorType);
                                    copy.AddBeatmapObjectData(sliderData);
                                }
                            }
                        }

                        if (conf.changeChainNotes && nextNoteData != null)
                        {
                            SliderData burstSliderData = ChainNotesUtil.CreateBurstSliderData(noteData, nextNoteData, noteData.colorType);
                            if (burstSliderData != null)
                            {
                                noteData.ChangeToBurstSliderHead();
                                copy.AddBeatmapObjectData(burstSliderData);
                            }
                        }
                        
                    }
                }

            }

            beatmapData = copy;
        }
    }
}