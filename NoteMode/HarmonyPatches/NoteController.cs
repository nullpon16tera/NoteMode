using HarmonyLib;
using NoteMode.Configuration;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteController), "Init")]
    static class NoteControllerInit
    {
        static void Prefix(ref NoteData noteData)
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
}