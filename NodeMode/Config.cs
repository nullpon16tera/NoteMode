using IPA.Utilities;
using System.Collections;
using System.IO;
using UnityEngine;

namespace NoteMode
{
    public static class Config
    {
        private static BS_Utils.Utilities.Config _config = new BS_Utils.Utilities.Config(Plugin.Name);

        public static bool oneColorRed = false;
        public static bool oneColorBlue = false;
        public static bool noArrow = false;
        public static float noteSize = 1f;

        public static FileSystemWatcher watcher = new FileSystemWatcher(UnityGame.UserDataPath)
        {
            NotifyFilter = NotifyFilters.LastWrite,
            Filter = Plugin.Name + ".ini",
            EnableRaisingEvents = true
        };

        private static bool _init;
        private static bool _ignoreConfigChanged;

        private static void Init()
        {
            watcher.Changed += OnConfigChanged;
            _init = true;
        }

        private static void OnConfigChanged(object sender, FileSystemEventArgs fileSystemEventArgs)
        {
            //Logger.log.Debug("OnConfigChanged");
            if (!_ignoreConfigChanged)
            {
                Config.Read();
                ModifierUI.instance.updateUI();
            }
        }

        public static void Read()
        {
            if (!_init)
            {
                Init();
            }

            oneColorRed = _config.GetBool(Plugin.Name, "oneColorRed", false, true);
            Logger.log.Debug($"NoteMode: {oneColorRed}");
            oneColorBlue = _config.GetBool(Plugin.Name, "oneColorBlue", false, true);
            noArrow = _config.GetBool(Plugin.Name, "noArrow", false, true);
            noteSize = _config.GetFloat(Plugin.Name, "noteSize", 1f, true);
        }

        public static void Write()
        {
            PersistentSingleton<SharedCoroutineStarter>.instance.StartCoroutine(DisableWatcherTemporaryCoroutine());
            _config.SetBool(Plugin.Name, "oneColorRed", oneColorRed);
            _config.SetBool(Plugin.Name, "oneColorBlue", oneColorBlue);
            _config.SetBool(Plugin.Name, "noArrow", noArrow);
            _config.SetFloat(Plugin.Name, "noteSize", noteSize);
        }

        private static IEnumerator DisableWatcherTemporaryCoroutine()
        {
            _ignoreConfigChanged = true;
            yield return new WaitForSecondsRealtime(1f);
            _ignoreConfigChanged = false;
        }
    }
}