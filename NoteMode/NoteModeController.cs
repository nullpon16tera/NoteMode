using BS_Utils.Gameplay;
using NoteMode.Configuration;
using UnityEngine;

namespace NoteMode
{
    public class NoteModeController : MonoBehaviour
    {
        public static NoteModeController instance { get; private set; }

        public bool inGame = false;


        #region Monobehaviour Messages
        private void Awake()
        {
            if (
                PluginConfig.Instance.noRed ||
                PluginConfig.Instance.noBlue ||
                PluginConfig.Instance.oneColorRed ||
                PluginConfig.Instance.oneColorBlue ||
                PluginConfig.Instance.noArrow ||
                PluginConfig.Instance.noNotesBomb ||
                PluginConfig.Instance.reverseArrows ||
                PluginConfig.Instance.randomizeArrows ||
                PluginConfig.Instance.restrictedrandomizeArrows ||
                PluginConfig.Instance.arcMode ||
                PluginConfig.Instance.allBurstSliderHead ||
                PluginConfig.Instance.changeChainNotes ||
                PluginConfig.Instance.isNotesScale
            )
            {
                ScoreSubmission.DisableSubmission(Plugin.Name);
            }
        }

        private void Start()
        {
            
        }

        private void Update()
        {
        }

        private void OnDestroy()
        {
            Logger.log?.Debug($"{name}: OnDestroy()");
        }
        #endregion
    }
}
