using HarmonyLib;
using NoteMode.Configuration;
using System.Reflection;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectManager), "SpawnBasicNote")]
    public class BeatmapObjectManagerSpawnBasicNote
    {
        public static NoteCutDirection copyNoteCutDirection;
        private static ColorType copyColorType;

        static bool Prefix(ref NoteData noteData, ref BeatmapObjectSpawnMovementData.NoteSpawnData noteSpawnData, float rotation, float cutDirectionAngleOffset)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if (PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue)
                {
                    copyColorType = noteData.colorType;
                }
                if ((noteData.colorType == ColorType.ColorB) && PluginConfig.Instance.oneColorRed)
                {
                    PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                    property.SetValue(noteData, ColorType.ColorA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }
                else if ((noteData.colorType == ColorType.ColorA) && PluginConfig.Instance.oneColorBlue)
                {
                    PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                    property.SetValue(noteData, ColorType.ColorB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }
                if (PluginConfig.Instance.reverseArrows || PluginConfig.Instance.noArrow)
                {
                    copyNoteCutDirection = noteData.cutDirection;
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

        public static void Postfix(ref NoteData noteData, ref BeatmapObjectSpawnMovementData.NoteSpawnData noteSpawnData, float rotation, float cutDirectionAngleOffset)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if ((copyColorType == ColorType.ColorB) && PluginConfig.Instance.oneColorRed)
                {
                    PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                    property.SetValue(noteData, ColorType.ColorB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }
                else if ((copyColorType == ColorType.ColorA) && PluginConfig.Instance.oneColorBlue)
                {
                    PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                    property.SetValue(noteData, ColorType.ColorA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
                }

                if (PluginConfig.Instance.reverseArrows || PluginConfig.Instance.noArrow)
                {
                    if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        noteData.SetNonPublicProperty("cutDirection", copyNoteCutDirection);
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