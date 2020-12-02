﻿using IPA;
using SiraUtil.Zenject;
using IPA.Config.Stores;
using Conf = IPA.Config.Config;
using BeatSaberPresence.Config;
using BeatSaberPresence.Installers;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaberPresence
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal const string Name = "Beat Saber Presence";

        [Init]
        public Plugin(Conf conf, IPALogger logger, Zenjector zenjector)
        {
            zenjector.OnApp<PluginInstaller>().WithParameters(logger, conf.Generated<PluginConfig>());
            zenjector.OnGame<PresenceGameInstaller>(false);
            zenjector.OnMenu<PresenceMenuInstaller>();
        }

        [OnEnable, OnDisable]
        public void OnState() { /* We don't need this, but BSIPA logs a warning if there is nothing. */ }
    }
}