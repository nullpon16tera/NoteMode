using NoteMode.Views;
using SiraUtil;
using Zenject;

namespace NoteMode.Installers
{
    public class NoteModeMenuInstaller : MonoInstaller
    {
        public override void InstallBindings() => this.Container.BindInterfacesAndSelfTo<SettingTabViewController>().FromNewComponentAsViewController().AsSingle().NonLazy();
    }
}
