using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.Parser;


namespace NoteMode
{
    internal class ModifierUI : NotifiableSingleton<ModifierUI>
    {
        [UIParams]
        BSMLParserParams parserParams;

        public void updateUI()
        {
            parserParams.EmitEvent("cancel");
        }

        [UIValue("noRed")]
        public bool noRed
        {
            get => Config.noRed;
            set
            {
                Config.noRed = value;
                Config.Write();
            }
        }

        [UIValue("noBlue")]
        public bool noBlue
        {
            get => Config.noBlue;
            set
            {
                Config.noBlue = value;
                Config.Write();
            }
        }

        [UIValue("oneColorRed")]
        public bool oneColorRed
        {
            get => Config.oneColorRed;
            set
            {
                Config.oneColorRed = value;
                Config.Write();
            }
        }

        [UIValue("oneColorBlue")]
        public bool oneColorBlue
        {
            get => Config.oneColorBlue;
            set
            {
                Config.oneColorBlue = value;
                Config.Write();
            }
        }

        [UIValue("noArrow")]
        public bool noArrow
        {
            get => Config.noArrow;
            set
            {
                Config.noArrow = value;
                Config.Write();
            }
        }
    }
}
