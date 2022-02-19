using HarmonyLib;
using NoteMode.Configuration;
using System;
using System.Reflection;


namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapDataTransformHelper), "CreateTransformedBeatmapData")]
    internal static class BeatmapDataTransformHelperCreateTransformedBeatmapData
    {
        private static PropertyInfo s_colorTypeProperty;
        public static PropertyInfo ColorTypeProperty => s_colorTypeProperty ?? (s_colorTypeProperty = typeof(NoteData).GetProperty("colorType"));

        private static void SetNoteColorType(NoteData noteData, ColorType colorType)
        {
            BeatmapDataTransformHelperCreateTransformedBeatmapData.ColorTypeProperty.GetSetMethod(true).Invoke(noteData, new object[]
            {
                colorType
            });
        }

        private static void SwitchNoteColorType(NoteData noteData)
        {
            ColorType colorType = (ColorType)BeatmapDataTransformHelperCreateTransformedBeatmapData.ColorTypeProperty.GetValue(noteData);
            Plugin.Log.Debug($"ColorType: {ColorType.ColorA}");
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

        [HarmonyPriority(600)]
        private static void Prefix(ref IReadonlyBeatmapData beatmapData, IPreviewBeatmapLevel beatmapLevel, GameplayModifiers gameplayModifiers, PracticeSettings practiceSettings, bool leftHanded, EnvironmentEffectsFilterPreset environmentEffectsFilterPreset, EnvironmentIntensityReductionOptions environmentIntensityReductionOptions, bool screenDisplacementEffectsEnabled)
        {
            if (!PluginConfig.Instance.noArrow && !PluginConfig.Instance.oneColorBlue && !PluginConfig.Instance.oneColorRed && !PluginConfig.Instance.reverseArrows)
            {
                return;
            }

            BeatmapData copy = beatmapData.GetCopy();
            foreach (BeatmapObjectData beatmapObjectData in copy.beatmapObjectsData)
            {
                NoteData noteData = beatmapObjectData as NoteData;
                if (noteData != null && noteData.cutDirection != NoteCutDirection.None)
                {
                    if (PluginConfig.Instance.noArrow)
                    {
                        noteData.SetNoteToAnyCutDirection();
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
                }
            }
            beatmapData = copy;
        }

        
    }
}