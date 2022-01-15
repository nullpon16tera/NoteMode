using HarmonyLib;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorManager), "ColorForSaberType")]
    static class ColorManagerColorForSaberType
    {
        static void Prefix(ref SaberType type)
        {
            if (NoteModeController.instance?.inGame == true)
            {
                if (Config.oneColorRed && type == SaberType.SaberB)
                {
                    type = SaberType.SaberA;
                }
                else if (Config.oneColorBlue && type == SaberType.SaberA)
                {
                    type = SaberType.SaberB;
                }
            }
        }

        [HarmonyPatch(typeof(ColorManager), "EffectsColorForSaberType")]
        static class ColorManagerEffectsColorForSaberType
        {
            static void Prefix(ref SaberType type)
            {
                if (NoteModeController.instance?.inGame == true)
                {
                    if (Config.oneColorRed && type == SaberType.SaberB)
                    {
                        type = SaberType.SaberA;
                    }
                    else if (Config.oneColorBlue && type == SaberType.SaberA)
                    {
                        type = SaberType.SaberB;
                    }
                }
            }
        }
    }
}