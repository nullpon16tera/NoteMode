using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NoteMode.Utilities
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class CutDirectionUtil
    {
        public static NoteCutDirection SwitchNoteCutDirection(NoteCutDirection noteCutDirection)
        {
            switch (noteCutDirection)
            {
                case NoteCutDirection.Left:
                    noteCutDirection = NoteCutDirection.Right;
                    break;
                case NoteCutDirection.Right:
                    noteCutDirection = NoteCutDirection.Left;
                    break;
                case NoteCutDirection.Up:
                    noteCutDirection = NoteCutDirection.Down;
                    break;
                case NoteCutDirection.Down:
                    noteCutDirection = NoteCutDirection.Up;
                    break;
                case NoteCutDirection.UpLeft:
                    noteCutDirection = NoteCutDirection.DownRight;
                    break;
                case NoteCutDirection.UpRight:
                    noteCutDirection = NoteCutDirection.DownLeft;
                    break;
                case NoteCutDirection.DownLeft:
                    noteCutDirection = NoteCutDirection.UpRight;
                    break;
                case NoteCutDirection.DownRight:
                    noteCutDirection = NoteCutDirection.UpLeft;
                    break;
                default:
                    break;
            }

            return noteCutDirection;
        }

        public static NoteCutDirection RandomizeNoteCutDirection(NoteData noteData)
        {
            NoteCutDirection noteCutDirection = noteData.cutDirection;
            UnityEngine.Random.InitState(DateTime.Now.Millisecond + (int)(noteData.time * 1000) + (int)noteData.noteLineLayer + noteData.lineIndex);
            int rand = UnityEngine.Random.Range(0, 3);

            switch (noteData.cutDirection)
            {
                case NoteCutDirection.Left:
                    if (rand == 1) noteCutDirection = NoteCutDirection.UpLeft;
                    if (rand == 2) noteCutDirection = NoteCutDirection.DownLeft;
                    break;
                case NoteCutDirection.Right:
                    if (rand == 1) noteCutDirection = NoteCutDirection.UpRight;
                    if (rand == 2) noteCutDirection = NoteCutDirection.DownRight;
                    break;
                case NoteCutDirection.Up:
                    if (rand == 1) noteCutDirection = NoteCutDirection.UpLeft;
                    if (rand == 2) noteCutDirection = NoteCutDirection.UpRight;
                    break;
                case NoteCutDirection.Down:
                    if (rand == 1) noteCutDirection = NoteCutDirection.DownLeft;
                    if (rand == 2) noteCutDirection = NoteCutDirection.DownRight;
                    break;
                case NoteCutDirection.UpLeft:
                    if (rand == 1) noteCutDirection = NoteCutDirection.Up;
                    if (rand == 2) noteCutDirection = NoteCutDirection.Left;
                    break;
                case NoteCutDirection.UpRight:
                    if (rand == 1) noteCutDirection = NoteCutDirection.Up;
                    if (rand == 2) noteCutDirection = NoteCutDirection.Right;
                    break;
                case NoteCutDirection.DownLeft:
                    if (rand == 1) noteCutDirection = NoteCutDirection.Down;
                    if (rand == 2) noteCutDirection = NoteCutDirection.Left;
                    break;
                case NoteCutDirection.DownRight:
                    if (rand == 1) noteCutDirection = NoteCutDirection.Down;
                    if (rand == 2) noteCutDirection = NoteCutDirection.Right;
                    break;
                default:
                    noteCutDirection = noteData.cutDirection;
                    break;
            }

            return noteCutDirection;
        }

        public static NoteCutDirection RestrictedRandomizeNoteCutDirection(NoteData noteData)
        {
            NoteCutDirection noteCutDirection = noteData.cutDirection;

            UnityEngine.Random.InitState(DateTime.Now.Millisecond + (int)(noteData.time * 1000));
            int rand3 = UnityEngine.Random.Range(0, 3);
            int rand2 = UnityEngine.Random.Range(0, 2);

            switch (noteData.cutDirection)
            {
                case NoteCutDirection.Left:
                    if ((int)noteData.noteLineLayer == 0)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.DownLeft;
                    }
                    else if ((int)noteData.noteLineLayer == 2)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.UpLeft;
                    }
                    else
                    {
                        //if (rand3 == 1) noteCutDirection = NoteCutDirection.UpLeft;
                        //if (rand3 == 2) noteCutDirection = NoteCutDirection.DownLeft;
                    }
                    break;
                case NoteCutDirection.Right:
                    if ((int)noteData.noteLineLayer == 0)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.DownRight;
                    }
                    else if ((int)noteData.noteLineLayer == 2)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.UpRight;
                    }
                    else
                    {
                        //if (rand3 == 1) noteCutDirection = NoteCutDirection.UpRight;
                        //if (rand3 == 2) noteCutDirection = NoteCutDirection.DownRight;
                    }
                    break;
                case NoteCutDirection.Up:
                    if (noteData.lineIndex == 0)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.UpLeft;
                    }
                    else if (noteData.lineIndex == 3)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.UpRight;
                    }
                    else
                    {
                        if (rand3 == 1) noteCutDirection = NoteCutDirection.UpLeft;
                        if (rand3 == 2) noteCutDirection = NoteCutDirection.UpRight;
                    }
                    break;
                case NoteCutDirection.Down:
                    if (noteData.lineIndex == 0)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.DownLeft;
                    }
                    else if (noteData.lineIndex == 3)
                    {
                        if (rand2 == 1) noteCutDirection = NoteCutDirection.DownRight;
                    }
                    else
                    {
                        if (rand3 == 1) noteCutDirection = NoteCutDirection.DownLeft;
                        if (rand3 == 2) noteCutDirection = NoteCutDirection.DownRight;
                    }
                    break;
                case NoteCutDirection.UpLeft:
                    if (rand2 == 1) noteCutDirection = NoteCutDirection.Up;
                    break;
                case NoteCutDirection.UpRight:
                    if (rand2 == 1) noteCutDirection = NoteCutDirection.Up;
                    break;
                case NoteCutDirection.DownLeft:
                    if (rand2 == 1) noteCutDirection = NoteCutDirection.Down;
                    break;
                case NoteCutDirection.DownRight:
                    if (rand2 == 1) noteCutDirection = NoteCutDirection.Down;
                    break;
                default:
                    noteCutDirection = noteData.cutDirection;
                    break;
            }

            return noteCutDirection;
        }
    }
}
