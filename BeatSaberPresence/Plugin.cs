using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using UnityEngine.SceneManagement;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace BeatSaberPresence {

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin {
        internal static Plugin instance { get; private set; }
        internal static string Name => "BeatSaberPresence";

        [Init]
        public void Init(IPALogger logger) {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");
        }

        [OnStart]
        public void OnApplicationStart() {
            Logger.log.Debug("OnApplicationStart");
            new GameObject("BeatSaberPresenceController").AddComponent<BeatSaberPresenceController>();
        }

        [OnExit]
        public void OnApplicationQuit() {
            Logger.log.Debug("OnApplicationQuit");
        }
    }
}
