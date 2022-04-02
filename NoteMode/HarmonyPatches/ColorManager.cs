using HarmonyLib;
using NoteMode.Configuration;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorManager), "ColorForSaberType")]
    static class ColorManagerColorForSaberType
    {
        static void Prefix(ref SaberType type)
        {
            if ((PluginConfig.Instance.noBlue || PluginConfig.Instance.oneColorRed) && type == SaberType.SaberB)
            {
                type = SaberType.SaberA;
            }
            else if ((PluginConfig.Instance.noRed || PluginConfig.Instance.oneColorBlue) && type == SaberType.SaberA)
            {
                type = SaberType.SaberB;
            }
        }

        [HarmonyPatch(typeof(ColorManager), "EffectsColorForSaberType")]
        static class ColorManagerEffectsColorForSaberType
        {
            static void Prefix(ref SaberType type)
            {
                if ((PluginConfig.Instance.noBlue || PluginConfig.Instance.oneColorRed) && type == SaberType.SaberB)
                {
                    type = SaberType.SaberA;
                }
                else if ((PluginConfig.Instance.noRed || PluginConfig.Instance.oneColorBlue) && type == SaberType.SaberA)
                {
                    type = SaberType.SaberB;
                }
            }
        }
    }
}