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
        static void Postfix(ref NoteData noteData, Transform ____noteTransform)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if (PluginConfig.Instance.isNotesScale)
                {
                    float getScale = PluginConfig.Instance.notesScale;
                    float x = 0f;
                    float y = 0f;
                    if (noteData.lineIndex == 0)
                    {
                        x = 1f - getScale + 0.1f;
                    }
                    if (noteData.lineIndex == 3)
                    {
                        x = -(1f - getScale + 0.1f);
                    }

                    ____noteTransform.gameObject.transform.localScale = Vector3.one * getScale;
                    ____noteTransform.gameObject.transform.localPosition = new Vector3(x, 0f, -(1f - getScale));
                }
            }
        }
    }
}