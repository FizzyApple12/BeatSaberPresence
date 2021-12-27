using System;
using Zenject;
using Discord;
using DiscordCore;
using SiraUtil.Logging;
using BeatSaberPresence.Config;

namespace BeatSaberPresence {
    internal class PresenceController : IInitializable, IDisposable {
        private readonly SiraLog siraLog;
        private readonly UserManager userManager;
        private readonly PluginConfig pluginConfig;
        private readonly DiscordInstance discordInstance;
        internal const long clientID = 708741346403287113;

        private bool didInstantiateUserManagerProperly = true;

        public Nullable<User> User { get; private set; } = null;

        internal PresenceController(SiraLog siraLog, PluginConfig pluginConfig) {
            this.siraLog = siraLog;
            this.pluginConfig = pluginConfig;
            this.discordInstance = DiscordManager.instance.CreateInstance(new DiscordSettings {
                appId = clientID,
                handleInvites = false,
                modId = nameof(BeatSaberPresence),
                modName = nameof(BeatSaberPresence),
            });
            this.userManager = DiscordClient.GetUserManager();

            if (this.userManager == null) didInstantiateUserManagerProperly = false;
        }

        public void Initialize() {
            siraLog.Debug("Initializing Presence Controller");
            if (didInstantiateUserManagerProperly) userManager.OnCurrentUserUpdate += CurrentUserUpdated;
        }

        private void CurrentUserUpdated() {
            if (didInstantiateUserManagerProperly) User = userManager.GetCurrentUser();
        }

        internal void SetActivity(Activity activity) {
            discordInstance.ClearActivity();
            if (!pluginConfig.Enabled) return;
            discordInstance.UpdateActivity(activity);
        }

        public void Dispose() {
            if (DiscordManager.IsSingletonAvailable && DiscordCore.UI.Settings.IsSingletonAvailable) discordInstance.DestroyInstance();
            if (didInstantiateUserManagerProperly) userManager.OnCurrentUserUpdate -= CurrentUserUpdated;
        }

        internal void ClearActivity() {
            discordInstance.ClearActivity();
        }
    }
}