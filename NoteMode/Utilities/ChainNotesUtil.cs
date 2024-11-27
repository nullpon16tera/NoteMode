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
	public class ChainNotesUtil
    {
        internal static NoteLineLayer tailBeforeLineLayer { get; private set; }
        internal static int tailRotation { get; private set; } = 1;
        internal static int tailLineIndex { get; private set; } = -1;
        internal static int tailLineCount { get; private set; } = 5;
        internal static bool isTailBeforeLineLayer { get; private set; } = false;

        public static void LayerSetup(NoteData noteData)
        {
            ChainNotesUtil.tailBeforeLineLayer = NoteLineLayer.Upper;
            ChainNotesUtil.isTailBeforeLineLayer = false;
            ChainNotesUtil.tailLineIndex = -1;
            ChainNotesUtil.tailLineCount = 5;
            if (noteData.noteLineLayer == NoteLineLayer.Upper)
            {
                if (ChainNotesUtil.IsCutDirectionDown(noteData.cutDirection))
                {
                    ChainNotesUtil.tailBeforeLineLayer = NoteLineLayer.Base;
                    ChainNotesUtil.isTailBeforeLineLayer = true;
                }
                else if (ChainNotesUtil.IsCutDirectionUp(noteData.cutDirection))
                {
                    ChainNotesUtil.tailBeforeLineLayer = NoteLineLayer.Top;
                    ChainNotesUtil.isTailBeforeLineLayer = true;
                }
                else
                {
                    ChainNotesUtil.isTailBeforeLineLayer = false;
                }

                switch (noteData.cutDirection)
                {
                    case NoteCutDirection.Up:
                    case NoteCutDirection.Down:
                        ChainNotesUtil.tailLineCount = 3;
                        if (noteData.lineIndex == 0)
                        {
                            ChainNotesUtil.tailLineIndex = 0;
                        }
                        else if (noteData.lineIndex == 3)
                        {
                            ChainNotesUtil.tailLineIndex = 3;
                        }
                        break;
                    case NoteCutDirection.UpLeft:
                    case NoteCutDirection.DownLeft:
                        ChainNotesUtil.tailLineCount = 8;
                        if (noteData.lineIndex == 3)
                        {
                            ChainNotesUtil.tailLineIndex = 1;
                        }
                        break;
                    case NoteCutDirection.UpRight:
                    case NoteCutDirection.DownRight:
                        ChainNotesUtil.tailLineCount = 8;
                        if (noteData.lineIndex == 0)
                        {
                            ChainNotesUtil.tailLineIndex = 2;
                        }
                        break;
                    default:
                        ChainNotesUtil.tailLineCount = -1;
                        break;
                }
            }
        }

        public static bool IsCutDirectionDown(NoteCutDirection cutDirection)
        {
            if (
                cutDirection == NoteCutDirection.Down ||
                cutDirection == NoteCutDirection.DownLeft ||
                cutDirection == NoteCutDirection.DownRight
            )
            {
                return true;
            }

            return false;
        }

        public static bool IsCutDirectionUp(NoteCutDirection cutDirection)
        {
            if (
                cutDirection == NoteCutDirection.Up ||
                cutDirection == NoteCutDirection.UpLeft ||
                cutDirection == NoteCutDirection.UpRight
            )
            {
                return true;
            }

            return false;
        }

        public static SliderData CreateBurstSliderData(NoteData noteData, NoteData nextNoteData, ColorType colorType)
        {
            ChainNotesUtil.LayerSetup(noteData);
            if (ChainNotesUtil.isTailBeforeLineLayer && ChainNotesUtil.tailLineIndex != -1)
            {
                if (noteData.cutDirection != nextNoteData.cutDirection || nextNoteData.cutDirection != NoteCutDirection.None)
                {
                    if (SliderUtil.matchCount < 2)
                    {
                        float tailtime = (noteData.timeToNextColorNote * 0.5f) / 2f;
                        return SliderData.CreateBurstSliderData(
                            noteData.colorType,
                            noteData.time,
                            noteData.beat,
                            noteData.rotation,
                            noteData.lineIndex,
                            noteData.noteLineLayer,
                            noteData.beforeJumpNoteLineLayer,
                            noteData.cutDirection,
                            noteData.time + tailtime,
                            ChainNotesUtil.tailRotation,
                            ChainNotesUtil.tailLineIndex,
                            ChainNotesUtil.tailBeforeLineLayer,
                            ChainNotesUtil.tailBeforeLineLayer,
                            ChainNotesUtil.tailLineCount,
                            1f
                        );
                    }
                }
            }
            return null;
        }
    }
}
