using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Parser;
using NoteMode.Configuration;


namespace NoteMode.UI
{
    public class ModifierController : PersistentSingleton<ModifierController>
    {
        [UIParams]
        BSMLParserParams parserParams;

        private static PluginConfig conf = PluginConfig.Instance;

        public void updateUI()
        {
            parserParams.EmitEvent("cancel");
        }

        [UIValue("noRed")]
        public bool noRed
        {
            get => conf.noRed;
            set
            {
                if (value)
                {
                    conf.noBlue = !value;
                    if (conf.oneColorBlue || conf.oneColorRed)
                    {
                        conf.oneColorRed = false;
                        conf.oneColorBlue = false;
                    }
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                }
                conf.noRed = value;
                
                updateUI();
            }
        }

        [UIValue("noBlue")]
        public bool noBlue
        {
            get => conf.noBlue;
            set
            {
                if (value)
                {
                    conf.noRed = !value;
                    if (conf.oneColorBlue || conf.oneColorRed)
                    {
                        conf.oneColorRed = false;
                        conf.oneColorBlue = false;
                    }
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                }
                conf.noBlue = value;

                updateUI();
            }
        }

        [UIValue("oneColorRed")]
        public bool oneColorRed
        {
            get => conf.oneColorRed;
            set
            {
                if (value)
                {
                    conf.oneColorBlue = !value;
                    if (conf.noRed || conf.noBlue)
                    {
                        conf.noRed = false;
                        conf.noBlue = false;
                    }
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                }
                conf.oneColorRed = value;
                updateUI();
            }
        }

        [UIValue("oneColorBlue")]
        public bool oneColorBlue
        {
            get => conf.oneColorBlue;
            set
            {
                if (value)
                {
                    conf.oneColorRed = !value;
                    if (conf.noRed || conf.noBlue)
                    {
                        conf.noRed = false;
                        conf.noBlue = false;
                    }
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                }
                conf.oneColorBlue = value;
                updateUI();
            }
        }

        [UIValue("noArrow")]
        public bool noArrow
        {
            get => conf.noArrow;
            set
            {
                if (value)
                {
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                    if (conf.reverseArrows) conf.reverseArrows = false;
                }
                conf.noArrow = value;
                updateUI();
            }
        }

        [UIValue("noNotesBomb")]
        public bool noNotesBomb
        {
            get => conf.noNotesBomb;
            set
            {
                if (value)
                {
                    if (conf.noRed) noRed = false;
                    if (conf.noBlue) noBlue = false;
                    if (conf.oneColorRed) oneColorRed = false;
                    if (conf.oneColorBlue) oneColorBlue = false;
                    if (conf.noArrow) noArrow = false;
                    if (conf.reverseArrows) conf.reverseArrows = false;
                }
                conf.noNotesBomb = value;
                updateUI();
            }
        }

        [UIValue("reverseArrows")]
        public bool reverseArrows
        {
            get => conf.reverseArrows;
            set
            {
                if (value)
                {
                    if (conf.noArrow) noArrow = false;
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                }
                conf.reverseArrows = value;
                updateUI();
            }
        }
    }
}
