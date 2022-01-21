using Zenject;
using SiraUtil;

namespace BeatSaberPresence.Installers {
    internal class PresenceMenuInstaller : Installer {
        public override void InstallBindings() {
            Container.BindInterfacesTo<MenuButtonManager>().AsSingle();
            Container.Bind<Settings>().FromNewComponentAsViewController().AsSingle();
            Container.Bind<ModFlowCoordinator>().FromNewComponentOnNewGameObject().AsSingle();
            Container.BindInterfacesTo<MenuPresenceManager>().FromNewComponentOnRoot().AsSingle();
        }
    }
}