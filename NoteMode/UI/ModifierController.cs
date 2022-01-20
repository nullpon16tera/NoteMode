using BeatSaberMarkupLanguage.Attributes;
using NoteMode.Configuration;


namespace NoteMode.UI
{
    public class ModifierController : PersistentSingleton<ModifierController>
    {
        [UIValue("noRed")]
        public bool noRed
        {
            get => PluginConfig.Instance.noRed;
            set => PluginConfig.Instance.noRed = value;
        }

        [UIValue("noBlue")]
        public bool noBlue
        {
            get => PluginConfig.Instance.noBlue;
            set => PluginConfig.Instance.noBlue = value;
        }

        [UIValue("oneColorRed")]
        public bool oneColorRed
        {
            get => PluginConfig.Instance.oneColorRed;
            set => PluginConfig.Instance.oneColorRed = value;
        }

        [UIValue("oneColorBlue")]
        public bool oneColorBlue
        {
            get => PluginConfig.Instance.oneColorBlue;
            set => PluginConfig.Instance.oneColorBlue = value;
        }

        [UIValue("noArrow")]
        public bool noArrow
        {
            get => PluginConfig.Instance.noArrow;
            set => PluginConfig.Instance.noArrow = value;
        }
    }
}
