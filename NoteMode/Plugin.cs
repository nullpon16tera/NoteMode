using BeatSaberMarkupLanguage.GameplaySetup;
using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using NoteMode.Configuration;
using NoteMode.UI;
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
        internal static IPALogger Log { get; private set; }

        internal static string Name => "NoteMode";

        [Init]
        public void Init(IPALogger logger, Config conf)
        {
            instance = this;
            Log = logger;
            Log.Info("NoteMode initialized.");
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            Log.Debug("Logger initialized.");
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
            new GameObject("NoteModeController").AddComponent<NoteModeController>();
            GameplaySetup.instance.AddTab("NoteMode", $"{Name}.UI.Modifier.bsml", ModifierController.instance);
        }

        [OnExit]
        public void OnApplicationQuit() => Log.Debug("OnApplicationQuit");

        [OnEnable]
        public void OnEnable()
        {
            ApplyHarmonyPatches();
        }

        [OnDisable]
        public void OnDisable()
        {
            RemoveHarmonyPatches();
        }

        public static void ApplyHarmonyPatches()
        {
            try
            {
                Log.Debug("Applying Harmony pathces.");
                harmony.PatchAll(Assembly.GetExecutingAssembly());
            }
            catch (Exception ex)
            {
                Log.Critical("Error applying Harmony patches: " + ex.Message);
                Log.Debug(ex);
            }
        }

        public static void RemoveHarmonyPatches()
        {
            try
            {
                harmony.UnpatchSelf();
            }
            catch (Exception ex)
            {
                Log.Critical("Error removing Harmony patches: " + ex.Message);
                Log.Debug(ex);
            }
            
        }
    }
}
