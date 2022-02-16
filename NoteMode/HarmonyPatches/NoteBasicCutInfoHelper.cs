using HarmonyLib;
using NoteMode.Configuration;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteBasicCutInfoHelper), "GetBasicCutInfo")]
    static class NoteBasicCutInfoHelperGetBasicCutInfo
    {
        static void Postfix(ColorType colorType, ref bool saberTypeOK)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if (
                    PluginConfig.Instance.noRed ||
                    PluginConfig.Instance.noBlue ||
                    PluginConfig.Instance.oneColorBlue ||
                    PluginConfig.Instance.oneColorRed
                )
                {
                    if ((colorType != ColorType.None))
                    {
                        saberTypeOK = true;
                    }
                }
            }
        }
    }
}