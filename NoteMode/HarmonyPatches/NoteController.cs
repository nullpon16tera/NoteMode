using HarmonyLib;
using NoteMode.Configuration;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteController), "Init")]
    static class NoteControllerInit
    {
        static void Prefix(ref NoteData noteData, ref float worldRotation, Transform ____noteTransform)
        {
            if (NoteModeController.instance?.inGame == true)
            {
                if (PluginConfig.Instance.noArrow)
                {
                    if (noteData.cutDirection != NoteCutDirection.None)
                    {
                        noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
                    }
                }
            }
        }


        static void Postfix(Transform ____noteTransform)
        {
            if (NoteModeController.instance?.inGame == true)
            {
                if (PluginConfig.Instance.isNotesScale)
                {
                    ____noteTransform.gameObject.transform.localScale = Vector3.one * PluginConfig.Instance.notesScale;
                }
            }
        }
    }
}