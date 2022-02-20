using BS_Utils.Gameplay;
using NoteMode.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NoteMode
{
    public class NoteModeController : MonoBehaviour
    {
        public static NoteModeController instance { get; private set; }

        public bool inGame = false;


        #region Monobehaviour Messages
        private void Awake()
        {
            if (instance != null)
            {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this); // Don't destroy this object on scene changes
            instance = this;
            Logger.log?.Debug($"{name}: Awake()");
        }

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void Start()
        {
            Logger.log.Debug($"{name}: Start()");
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void Update()
        {
        }

        private void OnDestroy()
        {
            Logger.log?.Debug($"{name}: OnDestroy()");
            if (instance == this)
            {
                instance = null;
            }
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }
        #endregion

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            inGame = false;

            if (BS_Utils.Plugin.LevelData.Mode == BS_Utils.Gameplay.Mode.Multiplayer) { return; }
            if (nextScene.name == "GameCore")
            {
                inGame = true;

                if (
                    PluginConfig.Instance.noRed ||
                    PluginConfig.Instance.noBlue ||
                    PluginConfig.Instance.oneColorRed ||
                    PluginConfig.Instance.oneColorBlue ||
                    PluginConfig.Instance.noArrow ||
                    PluginConfig.Instance.noNotesBomb ||
                    PluginConfig.Instance.reverseArrows ||
                    PluginConfig.Instance.randomizeArrows ||
                    PluginConfig.Instance.restrictedrandomizeArrows
                )
                {
                    ScoreSubmission.DisableSubmission(Plugin.Name);
                }
            }
            else if (nextScene.name == "MainMenu")
            {
                inGame = false;
            }
        }

        public void OnGameSceneLoaded()
        {

        }

        public void BeginGameCoreScene()
        {
            if (PluginConfig.Instance.noArrow)
            {
                this.StartCoroutine(this.TransformMap());
            }
        }

        private IEnumerator TransformMap()
        {
            yield return new WaitForSecondsRealtime(0.1f);
            
        }
    }
}
