using IPA;
using SiraUtil.Zenject;
using IPA.Config.Stores;
using Conf = IPA.Config.Config;
using BeatSaberPresence.Config;
using BeatSaberPresence.Installers;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaberPresence {
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin {
        internal const string Name = "BeatSaberPresence";

        [Init]
        public Plugin(Conf conf, IPALogger logger, Zenjector zenjector) {
            zenjector.UseLogger(logger);
            zenjector.Install<PluginInstaller>(Location.App, conf.Generated<PluginConfig>());
            zenjector.Install<PresenceGameInstaller>(Location.GameCore);
            zenjector.Install<PresenceMenuInstaller>(Location.Menu);
        }

        [OnEnable, OnDisable]
        public void OnState() { /* We don't need this, but BSIPA logs a warning if there is nothing. */ }
    }
}