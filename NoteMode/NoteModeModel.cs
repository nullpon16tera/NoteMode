using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace NoteMode
{
	public struct NoteModeModel
    {
        public NoteCutDirection CopyNoteCutDirection { get; set; }
        public ColorType CopyColorType { get; set; }

        public void SetDirection(NoteCutDirection noteCutDirection) => CopyNoteCutDirection = noteCutDirection;

        public void SetColorType(ColorType colorType)  => CopyColorType = colorType;
    }
}
