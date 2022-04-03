using NoteMode.Configuration;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using BeatSaberMarkupLanguage.Parser;
using BeatSaberMarkupLanguage.ViewControllers;
using Zenject;

namespace NoteMode.Views
{
    [HotReload]
    internal class SettingTabViewController : BSMLAutomaticViewController, IInitializable
    {
        public string ResourceName => string.Join(".", this.GetType().Namespace, this.GetType().Name);

        [UIParams]
        BSMLParserParams parserParams;

        private static PluginConfig conf = PluginConfig.Instance;

        public void updateUI()
        {
            parserParams.EmitEvent("cancel");
        }

        [UIAction("size_reset_click")]
        private void OnSizeReset()
        {
            conf.notesScale = 1.0f;
            updateUI();
        }

        [UIValue("notesScale")]
        public float notesScale
        {
            get => conf.notesScale;
            set
            {
                if (conf.isNotesScale)
                {
                    conf.notesScale = value;
                }
            }
        }

        [UIValue("isNotesScale")]
        public bool isNotesScale
        {
            get => conf.isNotesScale;
            set => conf.isNotesScale = value;
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
                    if (conf.randomizeArrows) conf.randomizeArrows = false;
                    if (conf.restrictedrandomizeArrows) conf.restrictedrandomizeArrows = false;
                    if (conf.changeChainNotes) conf.changeChainNotes = false;
                }
                conf.noArrow = value;
                updateUI();
            }
        }

        [UIValue("allBurstSliderHead")]
        public bool allBurstSliderHead
        {
            get => conf.allBurstSliderHead;
            set
            {
                if (value)
                {
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                }
                conf.allBurstSliderHead = value;
                updateUI();
            }
        }

        [UIValue("arcMode")]
        public bool arcMode
        {
            get => conf.arcMode;
            set
            {
                if (value)
                {
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                    if (conf.restrictedArcMode) conf.restrictedArcMode = false;
                }
                conf.arcMode = value;
                updateUI();
            }
        }

        [UIValue("restrictedArcMode")]
        public bool restrictedArcMode
        {
            get => conf.restrictedArcMode;
            set
            {
                if (value)
                {
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                    if (conf.arcMode) conf.arcMode = false;
                }
                conf.restrictedArcMode = value;
                updateUI();
            }
        }

        [UIValue("changeChainNotes")]
        public bool changeChainNotes
        {
            get => conf.changeChainNotes;
            set
            {
                if (value)
                {
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                    if (conf.noArrow) conf.noArrow = false;
                }
                conf.changeChainNotes = value;
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
                    if (conf.randomizeArrows) conf.randomizeArrows = false;
                    if (conf.restrictedrandomizeArrows) conf.restrictedrandomizeArrows = false;
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

        [UIValue("randomizeArrows")]
        public bool randomizeArrows
        {
            get => conf.randomizeArrows;
            set
            {
                if (value)
                {
                    if (conf.noArrow) noArrow = false;
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                    if (conf.restrictedrandomizeArrows) conf.restrictedrandomizeArrows = false;
                }
                conf.randomizeArrows = value;
                updateUI();
            }
        }

        [UIValue("restrictedrandomizeArrows")]
        public bool restrictedrandomizeArrows
        {
            get => conf.restrictedrandomizeArrows;
            set
            {
                if (value)
                {
                    if (conf.noArrow) noArrow = false;
                    if (conf.noNotesBomb) conf.noNotesBomb = false;
                    if (conf.randomizeArrows) conf.randomizeArrows = false;
                }
                conf.restrictedrandomizeArrows = value;
                updateUI();
            }
        }

        [UIValue("rainbowColor")]
        public bool rainbowColor
        {
            get => conf.rainbowColor;
            set
            {
                conf.rainbowColor = value;
                updateUI();
            }
        }

        protected override void OnDestroy()
        {
            GameplaySetup.instance.RemoveTab("NoteMode");
            base.OnDestroy();
        }

        public void Initialize() => GameplaySetup.instance.AddTab("NoteMode", this.ResourceName, this);
    }
}
