using HarmonyLib;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using SiraUtil.Zenject;
using NoteMode.Installers;
using System.Reflection;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace NoteMode
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }
        public const string HarmonyId = "com.github.nullpon16tera.NoteMode";
        private Harmony harmony;

        internal static string Name => "NoteMode";

        [Init]
        public void Init(IPALogger logger, Config conf, Zenjector zenjector)
        {
            Instance = this;
            Log = logger;
            Log.Info("NoteMode initialized.");
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Log.Debug("Logger initialized.");
            this.harmony = new Harmony(HarmonyId);
            zenjector.Install<NoteModeGameInstaller>(Location.Player);
            zenjector.Install<NoteModeMenuInstaller>(Location.Menu);
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
        public void OnApplicationStart() => Log.Debug("OnApplicationStart");

        [OnExit]
        public void OnApplicationQuit() => Log.Debug("OnApplicationQuit");

        [OnEnable]
        public void OnEnable() => this.harmony.PatchAll(Assembly.GetExecutingAssembly());

        [OnDisable]
        public void OnDisable() => this.harmony.UnpatchSelf();
    }
}
