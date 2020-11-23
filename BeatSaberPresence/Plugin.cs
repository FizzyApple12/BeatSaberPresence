using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Utilities;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;
using IPA.Config.Stores;
using BeatSaberPresence.Config;
using BeatSaberMarkupLanguage.MenuButtons;
using BeatSaberMarkupLanguage;
using SiraUtil.Zenject;

namespace BeatSaberPresence {
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static BeatSaberPresenceController pluginController { get { return BeatSaberPresenceController.Instance; } }

        internal static string Name => "BeatSaberPresence";

        internal static IConfigProvider configProvider;

        private MenuButton menuButton;

        private BeatSaberPresenceController beatSaberPresenceController;

        [Init]
        public Plugin(IPALogger logger, Zenjector zenjector) {
            Instance = this;

            Logger.log = logger;
            Logger.log.Debug("Logger initialized");

            zenjector.OnGame<PluginInstaller>();

            beatSaberPresenceController = new BeatSaberPresenceController();
        }

        [Init]
        public void Init(IPA.Config.Config conf) {
            PluginConfig.Instance = conf.Generated<PluginConfig>();
            Logger.log.Debug("Loaded config");
        }

        [OnStart]
        public void OnApplicationStart() {
            Logger.log.Debug("OnApplicationStart");
        }

        [OnEnable]
        public void OnEnable() {
            if (menuButton == null) menuButton = new MenuButton("BeatSaberPresence", "", SummonFlowCoordinator);

            beatSaberPresenceController.Initialize();

            MenuButtons.instance.RegisterButton(menuButton);
        }

        [OnDisable]
        public void OnDisable() {
            if (menuButton != null) MenuButtons.instance.UnregisterButton(menuButton);

            if (beatSaberPresenceController != null) beatSaberPresenceController.Dispose();
        }

        [OnExit]
        public void OnApplicationQuit() {
            Logger.log.Debug("OnApplicationQuit");
        }

        private void SummonFlowCoordinator() {
            var flowCoordinator = BeatSaberUI.CreateFlowCoordinator<ModFlowCoordinator>();
            if (flowCoordinator != null) BeatSaberUI.MainFlowCoordinator.PresentFlowCoordinator(flowCoordinator);
        }
    }
}
