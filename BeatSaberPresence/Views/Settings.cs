using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using BeatSaberPresence.Config;

namespace BeatSaberPresence {
    [HotReload(@"C:\Users\FizzyApple12\Desktop\BeatSaberPresence\BeatSaberPresence\Views\Settings.bsml")]
    [ViewDefinition("BeatSaberPresence.Views.Settings.bsml")]
    class Settings : BSMLAutomaticViewController {
        [UIValue("enable")]
        public bool enable = PluginConfig.Instance.Enabled;

        [UIValue("largeImage")]
        public bool largeImage = PluginConfig.Instance.ShowImages;
        [UIValue("smallImage")]
        public bool smallImage = PluginConfig.Instance.ShowSmallImages;

        [UIValue("timer")]
        public bool timer = PluginConfig.Instance.ShowTimes;
        [UIValue("countDown")]
        public bool countDown = PluginConfig.Instance.InGameCountDown;

        [UIAction("enableChange")]
        private void enableChange(bool newValue) {
            PluginConfig.Instance.Enabled = newValue;

            if (!newValue) BeatSaberPresenceController.Instance.clearPresence();
        }

        [UIAction("largeImageChange")]
        private void imageChange(bool newValue) {
            PluginConfig.Instance.ShowImages = newValue;
        }

        [UIAction("smallImageChange")]
        private void smallImageChange(bool newValue) {
            PluginConfig.Instance.ShowSmallImages = newValue;
        }

        [UIAction("timerChange")]
        private void timerChange(bool newValue) {
            PluginConfig.Instance.ShowTimes = newValue;
        }

        [UIAction("countDownChange")]
        private void countDownChange(bool newValue) {
            PluginConfig.Instance.InGameCountDown = newValue;
        }
    }
}
