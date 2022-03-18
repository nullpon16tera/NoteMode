using HarmonyLib;
using NoteMode.Configuration;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(SliderShaderHelper), "SetSaberAttractionPoint")]
    static class SliderShaderHelperSetSaberAttractionPoint
    {
        static void Prefix(ref MaterialPropertyBlock materialPropertyBlock, ref Vector3 attractPoint)
        {
            if (NoteModeController.instance.inGame == true)
            {

                //attractPoint = new Vector3(2f, 0f, 0f);
            }
        }
    }
}