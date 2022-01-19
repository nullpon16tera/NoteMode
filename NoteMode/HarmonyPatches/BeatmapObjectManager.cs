using HarmonyLib;
using System.Reflection;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectManager), "SpawnBasicNote")]
    public class BeatmapObjectManagerSpawnBasicNote
    {
        static bool Prefix(ref NoteData noteData)
        {
            if ((noteData.colorType == ColorType.ColorB) && Config.oneColorRed)
            {
                PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                property.SetValue(noteData, ColorType.ColorA, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }
            else if ((noteData.colorType == ColorType.ColorA) && Config.oneColorBlue)
            {
                PropertyInfo property = typeof(NoteData).GetProperty("colorType");
                property.SetValue(noteData, ColorType.ColorB, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null);
            }

            if ((noteData.colorType == ColorType.ColorA) && Config.noRed)
            {
                return false;
            }
            else if ((noteData.colorType == ColorType.ColorB) && Config.noBlue)
            {
                return false;
            }

            return true;
        }
    }
}