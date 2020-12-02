using Zenject;
using SiraUtil;
using IPA.Logging;
using BeatSaberPresence.Config;

namespace BeatSaberPresence.Installers
{
    internal class PluginInstaller : Installer<Logger, PluginConfig, PluginInstaller>
    {
        private readonly Logger _logger;
        private readonly PluginConfig _pluginConfig;

        internal PluginInstaller(Logger logger, PluginConfig pluginConfig)
        {
            _logger = logger;
            _pluginConfig = pluginConfig;
        }

        public override void InstallBindings()
        {
            Container.BindInstance(_pluginConfig);
            Container.BindLoggerAsSiraLogger(_logger);
            Container.BindInterfacesAndSelfTo<PresenceController>().AsSingle();
        }
    }
}