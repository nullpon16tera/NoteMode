using BS_Utils.Gameplay;
using BS_Utils.Utilities;
using NoteMode.Configuration;
using System;
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

        private PauseController _pauseController;

        private SaberManager _saberManager;
        private NoteCutter _noteCutter;
        private BeatmapObjectManager _beatmapObjectManager;

        private float _prevNoteTime;
        private List<NoteController> _noteList = new List<NoteController>();

        private bool _init;

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
            //SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        private void Start()
        {
            Logger.log?.Debug($"{name}: Start()");
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        public void OnActiveSceneChanged(Scene prevScene, Scene nextScene)
        {
            Logger.log?.Debug($"{prevScene.name}->{nextScene.name}");
            _init = false;
            inGame = false;

            if (nextScene.name == "GameCore")
            {
                inGame = true;

                if (BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Standard)
                {
                    return;
                }
                Logger.log.Debug("GameCore Scene Started");

                if (
                    PluginConfig.Instance.noRed ||
                    PluginConfig.Instance.noBlue ||
                    PluginConfig.Instance.oneColorRed ||
                    PluginConfig.Instance.oneColorBlue ||
                    PluginConfig.Instance.noArrow
                )
                {
                    ScoreSubmission.DisableSubmission(Plugin.Name);
                }

                StartCoroutine(OnGameCoreCoroutine());
            }
        }


        private IEnumerator OnGameCoreCoroutine()
        {
            yield return null;

            if (_pauseController == null)
            {
                _pauseController = Resources.FindObjectsOfTypeAll<PauseController>().FirstOrDefault();
                //_pauseController.didPauseEvent += OnPause;
                //_pauseController.didResumeEvent += OnPauseResume;
            }

            yield return new WaitUntil(() => FindObjectsOfType<Saber>().Any());
            yield return new WaitForSecondsRealtime(0.1f);

            if (_saberManager == null)
                _saberManager = FindObjectsOfType<SaberManager>().FirstOrDefault();
            if (_noteCutter == null)
            {
                CuttingManager cuttingManager = FindObjectsOfType<CuttingManager>().FirstOrDefault();
                _noteCutter = cuttingManager.GetPrivateField<NoteCutter>("_noteCutter");
            }

            if (
                PluginConfig.Instance.noArrow ||
                PluginConfig.Instance.oneColorRed ||
                PluginConfig.Instance.oneColorBlue ||
                PluginConfig.Instance.noRed ||
                PluginConfig.Instance.noBlue
            )
            {
                _beatmapObjectManager = _pauseController.GetPrivateField<BeatmapObjectManager>("_beatmapObjectManager");
                _beatmapObjectManager.noteWasSpawnedEvent -= OnNoteWasSpawned;
                _beatmapObjectManager.noteWasSpawnedEvent += OnNoteWasSpawned;

                _prevNoteTime = 0;
                _noteList.Clear();
            }

            if (_pauseController == null)
            {
                Logger.log.Debug("GameCore Init Fail");
                Logger.log.Debug($"{_pauseController}, {_noteCutter}");
            }
            else
            {
                Logger.log.Debug("GameCore Init Success");
            }

            _init = true;
        }

        private void OnNoteWasSpawned(NoteController noteController)
        {
            float time;

            if (noteController.noteData.colorType != ColorType.None)
            {
                time = noteController.noteData.time;
                if (time != _prevNoteTime)
                {
                    _prevNoteTime = time;
                    _noteList.Clear();
                }
                _noteList.Add(noteController);
            }
        }

        

        private void Update()
        {
            if (!_init)
            {
                return;
            }
        }

        private void OnDestroy()
        {
            Logger.log?.Debug($"{name}: OnDestroy()");
            if (instance == this)
                instance = null;
        }
        #endregion
    }
}
