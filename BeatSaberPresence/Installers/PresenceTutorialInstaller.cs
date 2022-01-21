using Zenject;
using BeatSaberPresence.Config;

namespace BeatSaberPresence.Installers {
    internal class PresenceTutorialInstaller : Installer {
        public override void InstallBindings() {
            if (Container.Resolve<PluginConfig>().Enabled) {
                Container.BindInterfacesTo<TutorialPresenceManager>().AsSingle();
            }
        }
    }
}