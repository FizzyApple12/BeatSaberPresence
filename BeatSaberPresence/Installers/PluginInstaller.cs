using Zenject;
using SiraUtil;
using IPA.Logging;
using BeatSaberPresence.Config;

namespace BeatSaberPresence.Installers
{
    internal class PluginInstaller : Installer<Logger, PluginConfig, PluginInstaller>
    {
        private readonly Logger logger;
        private readonly PluginConfig pluginConfig;

        internal PluginInstaller(Logger logger, PluginConfig pluginConfig)
        {
            this.logger = logger;
            this.pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(pluginConfig);
            Container.BindLoggerAsSiraLogger(logger);
            Container.BindInterfacesAndSelfTo<PresenceController>().AsSingle();
        }
    }
}