using BS_Utils.Gameplay;
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
            SceneManager.activeSceneChanged += OnActiveSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnActiveSceneChanged;
        }

        public void OnActiveSceneChanged(Scene oldScene, Scene newScene)
        {
            Logger.log.Debug($"OnActiveSceneChanged: {newScene.name}");

            _init = false;
            inGame = false;

            if (newScene.name == "GameCore")
            {
                inGame = true;

                Logger.log.Debug($"Mode: {BS_Utils.Plugin.LevelData.Mode}");
                if (BS_Utils.Plugin.LevelData.Mode != BS_Utils.Gameplay.Mode.Standard)
                {
                    Config.oneColorRed = false;
                    Config.oneColorBlue = false;
                    Config.noArrow = false;

                    ModifierUI.instance.updateUI();

                    return;
                }

                Config.Read();

                if (Config.oneColorRed || Config.oneColorBlue || Config.noArrow)
                {
                    ScoreSubmission.DisableSubmission(Plugin.Name);
                }

                StartCoroutine(OnGameCoreCoroutine());
            }
        }


        private GameObject FindChildren(GameObject parent, string name, bool includeInactive = false)
        {
            var children = parent.GetComponentsInChildren<Transform>(includeInactive);
            foreach (var transform in children)
            {
                if (transform.name == name)
                {
                    return transform.gameObject;
                }
            }
            return null;
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

            if (Config.noArrow || Config.oneColorRed || Config.oneColorBlue)
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

        private void Start()
        {
            Logger.log?.Debug($"{name}: Start()");
        }

        private void UpdateAdditionalSaber(Saber saber)
        {
            saber.ManualUpdate();
            if (_noteCutter != null)
            {
                _noteCutter.Cut(saber);
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
            instance = null;
        }
        #endregion
    }
}
