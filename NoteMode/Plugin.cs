using BeatSaberMarkupLanguage.GameplaySetup;
using HarmonyLib;
using IPA;
using System;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace NoteMode
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        public const string HarmonyId = "com.github.nullpon16tera.NoteMode";
        internal static Harmony harmony = new Harmony(HarmonyId);

        internal static Plugin instance { get; private set; }

        internal static string Name => "NoteMode";

        internal static string TabName => "NoteMode";

        internal static NoteModeController PluginController { get { return NoteModeController.instance; } }

        [Init]
        public Plugin(IPALogger logger)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        /*
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Config loaded");
        }
        */
        #endregion

        [OnStart]
        public void OnApplicationStart()
        {
            Config.Read();
            GameplaySetup.instance.AddTab(TabName, $"{Name}.UI.ModifierUI.bsml", ModifierUI.instance);
            new GameObject("NoteModeController").AddComponent<NoteModeController>();
            //ApplyHarmonyPatches();
        }

        [OnEnable]
        public void OnEnable() => harmony.PatchAll(Assembly.GetExecutingAssembly());

        [OnDisable]
        public void OnDisable() => harmony.UnpatchSelf();
    }
}
