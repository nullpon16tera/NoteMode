using HarmonyLib;
using NoteMode.Configuration;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NoteMode.HarmonyPatches
{
    [HarmonyPatch(typeof(BeatmapObjectsInTimeRowProcessor), "ProcessAllNotesInTimeRow")]
    static class BeatmapObjectsInTimeRowProcessorProcessAllNotesInTimeRow
    {
        static void Prefix(List<NoteData> notesInTimeRow)
        {
            if (notesInTimeRow.Count == 0)
            {
                return;
            }

            for (int j = 0; j < notesInTimeRow.Count; j++)
            {
                NoteData noteData = notesInTimeRow[j];
                Plugin.Log.Debug($"{noteData.lineIndex}");
            }
            notesInTimeRow.Clear();
        }
    }
}