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
        internal static int matchCount { get; private set; } = 0;
        internal static NoteData _nextNoteData { get; private set; }
        internal static float time { get; private set; } = 0f;
        internal static int lineIndex { get; private set; }
        internal static NoteLineLayer noteLineLayer { get; private set; }

        public static NoteData NextNoteData(NoteData noteData, BeatmapObjectData[] beatmapObjectDataItems)
        {
            SliderUtil._nextNoteData = null;
            SliderUtil.matchCount = 0;
            foreach (NoteData noteData1 in beatmapObjectDataItems)
            {
                if (noteData.time == noteData1.time)
                {
                    matchCount++;
                }
                if (PluginConfig.Instance.oneColorRed || PluginConfig.Instance.oneColorBlue)
                {
                    /*if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time)
                    {
                        if (1 < matchCount)
                        {
                            if (noteData1.time == SliderUtil.time && noteData1.lineIndex == SliderUtil.lineIndex && noteData1.noteLineLayer == SliderUtil.noteLineLayer)
                            {
                                continue;
                            }
                        }
                        SliderUtil._nextNoteData = noteData1;
                        SliderUtil.time = noteData1.time;
                        SliderUtil.lineIndex = noteData1.lineIndex;
                        SliderUtil.noteLineLayer = noteData1.noteLineLayer;
                        break;
                    }*/
                    if ((noteData.time + noteData.timeToNextColorNote) <= noteData1.time)
                    {
                        if (noteData.colorType != noteData1.colorType)
                        {
                            continue;
                        }
                        if (1 < SliderUtil.matchCount)
                        {
                            if (noteData1.time == SliderUtil.time && noteData1.lineIndex == SliderUtil.lineIndex && noteData1.noteLineLayer == SliderUtil.noteLineLayer)
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
                else if (PluginConfig.Instance.noArrow)
                {
                    if ((noteData.time + noteData.timeToNextColorNote) <= noteData1.time)
                    {
                        if (noteData.colorType != noteData1.colorType)
                        {
                            continue;
                        }
                        if (1 < matchCount)
                        {
                            if (noteData1.time == SliderUtil.time && noteData1.lineIndex == SliderUtil.lineIndex && noteData1.noteLineLayer == SliderUtil.noteLineLayer)
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
                    if (PluginConfig.Instance.restrictedArcMode)
                    {
                        if ((noteData.time + noteData.timeToNextColorNote) == noteData1.time && noteData.colorType == noteData1.colorType)
                        {
                            SliderUtil._nextNoteData = noteData1;
                            break;
                        }
                    }
                    else
                    {
                        if ((noteData.time + noteData.timeToNextColorNote) <= noteData1.time)
                        {
                            if (noteData.colorType != noteData1.colorType)
                            {
                                continue;
                            }
                            /*if (0 < matchCount)
                            {
                                if (noteData1.time == time && noteData1.lineIndex == lineIndex && noteData1.noteLineLayer == noteLineLayer)
                                {
                                    continue;
                                }
                            }*/
                            SliderUtil._nextNoteData = noteData1;
                            SliderUtil.time = noteData1.time;
                            SliderUtil.lineIndex = noteData1.lineIndex;
                            SliderUtil.noteLineLayer = noteData1.noteLineLayer;
                            break;
                        }
                    }
                }
            }

            return SliderUtil._nextNoteData;
        }

        public static SliderData CreateSliderData(NoteData noteData, NoteData nextNoteData, ColorType colorType)
        {
            float headControllPointLength = 0.8f;
            float nextTime = noteData.time + noteData.timeToNextColorNote;
            NoteCutDirection cutDirection = noteData.cutDirection;
            NoteCutDirection nextCutDirection = nextNoteData.cutDirection;

            if (noteData.cutDirection == NoteCutDirection.Any && nextNoteData.cutDirection == NoteCutDirection.Any)
            {
                headControllPointLength = 0f;
            }
            if (PluginConfig.Instance.arcMode)
            {
                nextTime = nextNoteData.time;
            }

            if (PluginConfig.Instance.reverseArrows)
            {
                cutDirection = CutDirectionUtil.SwitchNoteCutDirection(noteData.cutDirection);
                nextCutDirection = CutDirectionUtil.SwitchNoteCutDirection(nextNoteData.cutDirection);
            }

            return SliderData.CreateSliderData(
                colorType, // ColorType colorType
                noteData.time, // float headTime
                noteData.beat, // float headBeat
                noteData.rotation, // int headRotation
                noteData.lineIndex, // int headLineIndex
                noteData.noteLineLayer, // NoteLineLayer headNoteLineLayer
                noteData.beforeJumpNoteLineLayer, // NoteLineLayer headBeforeJumpNoteLineLayer
                headControllPointLength, // float headControlPointLengthMultiplier
                cutDirection, // NoteCutDirection headCutDirection
                nextTime, // float tailTime
                nextNoteData.rotation, // int tailRotation
                nextNoteData.lineIndex, // int tailLineIndex
                nextNoteData.noteLineLayer, // NoteLineLayer tailNoteLineLayer
                nextNoteData.noteLineLayer, // NoteLineLayer tailBeforeJumpNoteLineLayer
                0.6f, // float tailControlPointLengthMultiplier
                nextCutDirection, // NoteCutDirection tailCutDirection
                SliderMidAnchorMode.Straight // SliderMidAnchorMode midAnchorMode
            );
        }

        public static SliderData CreateAnySliderData(NoteData noteData, NoteData nextNoteData, ColorType colorType)
        {
            float nextTime = noteData.time + noteData.timeToNextColorNote;
            if (PluginConfig.Instance.noArrow || PluginConfig.Instance.oneColorBlue || PluginConfig.Instance.oneColorRed)
            {
                nextTime = nextNoteData.time;
            }
            return SliderData.CreateSliderData(
                colorType,
                noteData.time,
                noteData.beat,
                noteData.rotation,
                noteData.lineIndex,
                noteData.noteLineLayer,
                noteData.beforeJumpNoteLineLayer,
                0.6f,
                NoteCutDirection.Any,
                nextTime,
                nextNoteData.rotation,
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
