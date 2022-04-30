using HarmonyLib;
using NoteMode.Configuration;
using System;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(ColorManager), nameof(ColorManager.ColorForType), new Type[] { typeof(ColorType) })]
    [HarmonyPriority(600)]
    public class ColorManagerColorForTypePatch
    {
        private static bool oneColor = (PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue);

        public static bool Prefix(ref ColorType type, ref Color __result)
        {
            if (!Enable)
            {
                return true;
            }
            switch (type)
            {
                case ColorType.ColorA:
                    __result = LeftColor;
                    break;
                case ColorType.ColorB:
                    __result = RightColor;
                    break;
                default:
                    __result = Color.clear;
                    break;
            }

            return false;
        }

        internal static bool Enable { get; set; }
        internal static Color LeftColor { get; set; }
        internal static Color RightColor { get; set; }
    }

    [HarmonyPatch(typeof(ColorManager), "ColorForSaberType")]
    [HarmonyPriority(600)]
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

    }

    [HarmonyPatch(typeof(ColorManager), "EffectsColorForSaberType")]
    [HarmonyPriority(600)]
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