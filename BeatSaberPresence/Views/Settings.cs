using Zenject;
using BeatSaberPresence.Config;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;

namespace BeatSaberPresence {
    [HotReload("Settings.bsml")]
    [ViewDefinition("BeatSaberPresence.Views.Settings.bsml")]
    internal class Settings : BSMLAutomaticViewController {
        private PluginConfig _pluginConfig;
        private PresenceController _presenceController;

        [Inject]
        protected void Construct(PluginConfig pluginConfig, PresenceController presenceController) {
            _pluginConfig = pluginConfig;
            _presenceController = presenceController;
        }

        [UIValue("enabled")]
        public bool Enable {
            get => _pluginConfig.Enabled;
            set {
                _pluginConfig.Enabled = value;
                _presenceController.ClearActivity();
            }
        }

        [UIValue("large-image")]
        public bool LargeImage {
            get => _pluginConfig.ShowImages;
            set => _pluginConfig.ShowImages = value;
        }

        [UIValue("small-image")]
        public bool SmallImage {
            get => _pluginConfig.ShowSmallImages;
            set => _pluginConfig.ShowSmallImages = value;
        }

        [UIValue("timer")]
        public bool Timer {
            get => _pluginConfig.ShowTimes;
            set => _pluginConfig.ShowTimes = value;
        }

        [UIValue("countdown")]
        public bool Countdown {
            get => _pluginConfig.InGameCountDown;
            set => _pluginConfig.InGameCountDown = value;
        }
    }
}