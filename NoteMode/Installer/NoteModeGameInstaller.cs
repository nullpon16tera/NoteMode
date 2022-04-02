using NoteMode.Controllers;
using SiraUtil;
using Zenject;

namespace NoteMode.Installer
{
    /// <summary>
    /// Monobehaviours (scripts) are added to GameObjects.
    /// For a full list of Messages a Monobehaviour can receive from the game, see https://docs.unity3d.com/ScriptReference/MonoBehaviour.html.
    /// </summary>
	public class NoteModeGameInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            this.Container.BindInterfacesAndSelfTo<NoteModeController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
            this.Container.BindInterfacesAndSelfTo<BeatmapDataTransformController>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();
        }
    }
}
