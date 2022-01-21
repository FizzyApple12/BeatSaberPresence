using Zenject;
using BeatSaberPresence.Config;

namespace BeatSaberPresence.Installers {
    internal class PresenceMultiplayerInstaller : Installer {
        public override void InstallBindings() {
            if (Container.Resolve<PluginConfig>().Enabled) {
                Container.BindInterfacesTo<MultiplayerPresenceManager>().AsSingle();
            }
        }
    }
}