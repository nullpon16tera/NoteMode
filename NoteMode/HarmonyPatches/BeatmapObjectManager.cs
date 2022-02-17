using HarmonyLib;
using NoteMode.Configuration;
using System.Reflection;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectManager), "SpawnBasicNote")]
    public class BeatmapObjectManagerSpawnBasicNote
    {
        private static PropertyInfo s_colorTypeProperty;
        public static PropertyInfo ColorTypeProperty => s_colorTypeProperty ?? (s_colorTypeProperty = typeof(NoteData).GetProperty("colorType"));

        static bool Prefix(ref NoteData noteData, ref NoteModeModel __state)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if (PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue)
                {
                    __state.SetColorType(noteData.colorType);
                }
                if ((noteData.colorType == ColorType.ColorB) && PluginConfig.Instance.oneColorRed)
                {
                    ColorTypeProperty.SetValue(noteData, ColorType.ColorA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }
                else if ((noteData.colorType == ColorType.ColorA) && PluginConfig.Instance.oneColorBlue)
                {
                    ColorTypeProperty.SetValue(noteData, ColorType.ColorB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }
                if (PluginConfig.Instance.reverseArrows || PluginConfig.Instance.noArrow)
                {
                    __state.SetDirection(noteData.cutDirection);
                }

                if (PluginConfig.Instance.reverseArrows)
                {
                    if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        switch(noteData.cutDirection)
                        {
                            case NoteCutDirection.Left:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Right);
                                break;
                            case NoteCutDirection.Right:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Left);
                                break;
                            case NoteCutDirection.Up:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Down);
                                break;
                            case NoteCutDirection.Down:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Up);
                                break;
                            case NoteCutDirection.UpLeft:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.DownRight);
                                break;
                            case NoteCutDirection.UpRight:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.DownLeft);
                                break;
                            case NoteCutDirection.DownLeft:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.UpRight);
                                break;
                            case NoteCutDirection.DownRight:
                                noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.UpLeft);
                                break;
                            default:
                                break;
                        }
                    }
                }

                if (PluginConfig.Instance.noArrow)
                {
                    if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        noteData.SetNoteToAnyCutDirection();
                    }
                }

                if ((noteData.colorType == ColorType.ColorA) && PluginConfig.Instance.noRed)
                {
                    return false;
                }
                else if ((noteData.colorType == ColorType.ColorB) && PluginConfig.Instance.noBlue)
                {
                    return false;
                }

                if (PluginConfig.Instance.noNotesBomb)
                {
                    return false;
                }
            }

            return true;
        }

        public static void Postfix(NoteData noteData, ref NoteModeModel __state)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if ((__state.CopyColorType == ColorType.ColorB) && PluginConfig.Instance.oneColorRed)
                {
                    ColorTypeProperty.SetValue(noteData, ColorType.ColorB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }
                else if ((__state.CopyColorType == ColorType.ColorA) && PluginConfig.Instance.oneColorBlue)
                {
                    ColorTypeProperty.SetValue(noteData, ColorType.ColorA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }

                if (PluginConfig.Instance.reverseArrows || PluginConfig.Instance.noArrow)
                {
                    if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        noteData.SetNonPublicProperty("cutDirection", __state.CopyNoteCutDirection);
                    }
                }
            }
        }
    }

    [HarmonyPatch(typeof(BeatmapObjectManager), "SpawnBombNote")]
    public class BeatmapOjbectManagerSpawnBombNote
    {
        static bool Prefix(ref NoteData noteData)
        {
            if (PluginConfig.Instance.noNotesBomb)
            {
                return false;
            }
            return true;
        }
    }
}