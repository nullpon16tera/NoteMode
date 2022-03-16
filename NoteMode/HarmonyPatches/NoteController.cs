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
        private static float ScaleLineIndex(NoteData noteData, float _scale)
        {
            float x = 0f;
            // Left
            if (noteData.lineIndex == 0)
            {
                x = (1f - _scale) * _scale + ((1f - _scale) / 2 - 0.1f);
            }
            // LeftCenter
            if (noteData.lineIndex == 1)
            {
                x = (1f - _scale) * _scale / 2 - ((1f - _scale) * 0.01f);
                //Plugin.Log.Debug($"NoteControllerInit: lineIndex[1]:{x}");
            }
            // RightCenter
            if (noteData.lineIndex == 2)
            {
                x = -((1f - _scale) * _scale / 2 - ((1f - _scale) * 0.01f));
                //Plugin.Log.Debug($"NoteControllerInit: lineIndex[2]:{x}");
            }
            // Right
            if (noteData.lineIndex == 3)
            {
                x = -(((1f - _scale) * _scale) + ((1f - _scale) / 2 - 0.1f));
            }

            return x;
        }

        static void Postfix(ref NoteData noteData, Transform ____noteTransform)
        {
            if (NoteModeController.instance.inGame == true)
            {
                if (PluginConfig.Instance.isNotesScale && PluginConfig.Instance.notesScale != 1f)
                {
                    float getScale = PluginConfig.Instance.notesScale;
                    float x = NoteControllerInit.ScaleLineIndex(noteData, getScale);
                    float y = 0f;

                    // Base Line (Bottom)
                    if (noteData.noteLineLayer == NoteLineLayer.Base)
                    {

                    }

                    // Upper Line (Middle)
                    if (noteData.noteLineLayer == NoteLineLayer.Upper)
                    {
                        //y = -((1f - getScale) / 2);
                        y = -(((1f - getScale) * getScale) / 2 + ((1f - getScale) * 0.01f));
                    }

                    // Top Line (Top)
                    if (noteData.noteLineLayer == NoteLineLayer.Top)
                    {
                        y = -((1f - getScale) * getScale + ((1f - getScale) * 0.01f));
                    }

                    ____noteTransform.gameObject.transform.localScale = Vector3.one * getScale;
                    ____noteTransform.gameObject.transform.localPosition = new Vector3(x, y, -(1f - getScale));
                }
            }
        }
    }
}