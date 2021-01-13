using System;
using Zenject;
using Discord;
using UnityEngine;
using BeatSaberPresence.Config;

namespace BeatSaberPresence {
    internal class MenuPresenceManager : MonoBehaviour, IInitializable, IDisposable {
        private Activity? _menuActivity;
        private PluginConfig _pluginConfig;
        private PresenceController _presenceController;

        [Inject]
        internal void Construct(PluginConfig pluginConfig, PresenceController presenceController) {
            _pluginConfig = pluginConfig;
            _presenceController = presenceController;
            Set();
        }

        protected void OnEnable() {
            if (_presenceController != null) {
                _menuActivity = RebuildActivity();
                Set();
            }
        }

        #region Config Reloading

        public void Initialize() {
            _pluginConfig.Reloaded += ConfigReloaded;
        }

        public void Dispose() {
            _pluginConfig.Reloaded -= ConfigReloaded;
        }

        private void ConfigReloaded(PluginConfig _) {
            _menuActivity = RebuildActivity();
        }

        #endregion

        private void Set() {
            if (!_menuActivity.HasValue) _menuActivity = RebuildActivity();
            Activity activity = _menuActivity.Value;
            ActivityTimestamps timestamps = _menuActivity.Value.Timestamps;
            timestamps.Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            activity.Timestamps = timestamps;
            _menuActivity = activity;
            _presenceController.SetActivity(_menuActivity.Value);
        }

        private Activity RebuildActivity() {
            var activity = new Activity {
                State = Format(_pluginConfig.MenuBottomLine),
                Details = Format(_pluginConfig.MenuTopLine)
            };
            if (_pluginConfig.ShowTimes) {
                activity.Timestamps = new ActivityTimestamps {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
            }
            if (_pluginConfig.ShowImages) {
                activity.Assets = new ActivityAssets {
                    LargeImage = "beat_saber_logo",
                    LargeText = Format(_pluginConfig.MenuLargeImageLine)
                };

                if (_pluginConfig.ShowSmallImages) {
                    activity.Assets.SmallImage = "beat_saber_block";
                    activity.Assets.SmallText = Format(_pluginConfig.MenuSmallImageLine);
                }
            }
            return activity;
        }

        private string Format(string rpcString) {
            string formattedString = rpcString;

            if (_presenceController.User != null) formattedString = formattedString.Replace("{DiscordName}", _presenceController.User.Value.Username);
            if (_presenceController.User != null) formattedString = formattedString.Replace("{DiscordDiscriminator}", _presenceController.User.Value.Discriminator);

            return formattedString;
        }
    }
}