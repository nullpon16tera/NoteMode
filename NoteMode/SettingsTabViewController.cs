using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.Components;
using BeatSaberMarkupLanguage.ViewControllers;
using System;
using System.Collections.Generic;


namespace NoteMode.Views
{
    [HotReload(RelativePathToLayout = @"SettingsTabViewController.bsml")]
    [ViewDefinition("NoteMode.Views.SettingsTabViewController.bsml")]
    internal class SettingsTabViewController : BSMLAutomaticViewController
    {
        private string yourTextField = "Hello World";
        public string YourTextProperty
        {
            get { return yourTextField; }
            set
            {
                if (yourTextField == value) return;
                yourTextField = value;
                NotifyPropertyChanged();
            }
        }

        [UIAction("#post-parse")]
        internal void PostParse()
        {
            // Code to run after BSML finishes
        }
    }
}
