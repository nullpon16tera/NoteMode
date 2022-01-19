using HarmonyLib;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteBasicCutInfoHelper), "GetBasicCutInfo")]
    static class NoteBasicCutInfoHelperGetBasicCutInfo
    {
        static void Postfix(ColorType colorType, ref bool saberTypeOK)
        {
            if (Config.oneColorBlue || Config.oneColorRed)
            {
                if ((colorType != ColorType.None))
                {
                    saberTypeOK = true;
                }
            }
        }
    }
}