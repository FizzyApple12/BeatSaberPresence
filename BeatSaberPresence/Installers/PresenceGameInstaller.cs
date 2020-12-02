using Zenject;
using BeatSaberPresence.Config;

namespace BeatSaberPresence.Installers
{
    internal class PresenceGameInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (Container.Resolve<PluginConfig>().Enabled)
            {
                Container.BindInterfacesTo<GamePresenceManager>().AsSingle();
            }
        }
    }
}