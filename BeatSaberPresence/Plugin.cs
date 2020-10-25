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

namespace BeatSaberPresence {
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin {
        internal static Plugin Instance { get; private set; }
        internal static BeatSaberPresenceController pluginController { get { return BeatSaberPresenceController.Instance; } }

        internal static string Name => "BeatSaberPresence";

        internal static IConfigProvider configProvider;

        private MenuButton menuButton;

        [Init]
        public Plugin(IPALogger logger) {
            Instance = this;

            Logger.log = logger;
            Logger.log.Debug("Logger initialized");
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
            new GameObject("BeatSaberPresenceController").AddComponent<BeatSaberPresenceController>();

            if (menuButton == null) menuButton = new MenuButton("BeatSaberPresence", "", SummonFlowCoordinator);

            MenuButtons.instance.RegisterButton(menuButton);
        }

        [OnDisable]
        public void OnDisable() {
            if (menuButton != null) MenuButtons.instance.UnregisterButton(menuButton);
            if (pluginController != null) GameObject.Destroy(pluginController);
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
