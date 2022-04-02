using NoteMode.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NoteMode.Utilities
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class SliderUtil
    {
        public static int matchCount { get; private set; } = 0;
        public static NoteData _nextNoteData { get; private set; }
        public static float time { get; private set; } = 0f;
        public static int lineIndex { get; private set; }
        public static NoteLineLayer noteLineLayer { get; private set; }

        public static NoteData NextNoteData(NoteData noteData, BeatmapObjectData[] beatmapObjectDataItems)
        {
            SliderUtil._nextNoteData = null;
            foreach (NoteData noteData1 in beatmapObjectDataItems)
            {
                if (noteData.time == noteData1.time)
                {
                    matchCount++;
                }
                if (PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue)
                {
                    Plugin.Log.Debug($"oneColor: true");
                    if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time)
                    {
                        if (0 < matchCount)
                        {
                            if (noteData1.time == time && noteData1.lineIndex == lineIndex && noteData1.noteLineLayer == noteLineLayer)
                            {
                                continue;
                            }
                        }
                        SliderUtil._nextNoteData = noteData1;
                        SliderUtil.time = noteData1.time;
                        SliderUtil.lineIndex = noteData1.lineIndex;
                        SliderUtil.noteLineLayer = noteData1.noteLineLayer;
                        break;
                    }
                }
                else
                {
                    if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time && noteData.colorType == noteData1.colorType)
                    {
                        SliderUtil._nextNoteData = noteData1;
                        break;
                    }
                }
            }

            return SliderUtil._nextNoteData;
        }

        public static SliderData CreateSliderData(NoteData noteData, NoteData nextNoteData, ColorType colorType)
        {
            float headControllPointLength = 0.8f;
            if (nextNoteData.cutDirection == NoteCutDirection.Any)
            {
                headControllPointLength = 0f;
            }
            return SliderData.CreateSliderData(
                colorType,
                noteData.time,
                noteData.lineIndex,
                noteData.noteLineLayer,
                noteData.beforeJumpNoteLineLayer,
                headControllPointLength,
                noteData.cutDirection,
                noteData.time + noteData.timeToNextColorNote,
                nextNoteData.lineIndex,
                nextNoteData.noteLineLayer,
                nextNoteData.noteLineLayer,
                0.6f,
                nextNoteData.cutDirection,
                SliderMidAnchorMode.Straight
            );
        }

        public static SliderData CreateAnySliderData(NoteData noteData, NoteData nextNoteData, ColorType colorType)
        {
            return SliderData.CreateSliderData(
                colorType,
                noteData.time,
                noteData.lineIndex,
                noteData.noteLineLayer,
                noteData.beforeJumpNoteLineLayer,
                0.6f,
                NoteCutDirection.Any,
                noteData.time + noteData.timeToNextColorNote,
                nextNoteData.lineIndex,
                nextNoteData.noteLineLayer,
                nextNoteData.noteLineLayer,
                0.6f,
                NoteCutDirection.Any,
                SliderMidAnchorMode.Straight
            );
        }
    }
}
