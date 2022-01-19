using HarmonyLib;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(NoteController), "Init")]
    static class NoteControllerInit
    {
        static void Prefix(ref NoteData noteData, ref Vector3 moveStartPos, ref Vector3 moveEndPos, ref Vector3 jumpEndPos, Transform ____noteTransform)
        {
            if (Config.noArrow)
            {
                if (noteData.cutDirection != NoteCutDirection.None)
                {
                    Logger.log.Info($"NotesSize: No Arrow.");
                    noteData.SetNonPublicProperty("cutDirection", NoteCutDirection.Any);
                }
            }
        }

        static void Postfix(Transform ____noteTransform)
        {
            //____noteTransform.localScale = Vector3.one * 0.7f;
        }
    }
}