using System;
using Zenject;
using Discord;
using DiscordCore;
using SiraUtil.Tools;
using BeatSaberPresence.Config;

namespace BeatSaberPresence
{
    internal class PresenceController : IInitializable, IDisposable
    {
        private readonly SiraLog _siraLog;
        private readonly UserManager _userManager;
        private readonly PluginConfig _pluginConfig;
        private readonly DiscordInstance _discordInstance;
        internal const long clientID = 708741346403287113;

        public User User { get; private set; }

        internal PresenceController(SiraLog siraLog, PluginConfig pluginConfig)
        {
            _siraLog = siraLog;
            _pluginConfig = pluginConfig;
            _discordInstance = DiscordManager.instance.CreateInstance(
                new DiscordSettings
                {
                    appId = clientID,
                    handleInvites = false,
                    modId = nameof(BeatSaberPresence),
                    modName = nameof(BeatSaberPresence),
                });
            _userManager = DiscordClient.GetUserManager();
        }

        public void Initialize()
        {
            _siraLog.Debug("Initializing Presence Controller");
            _userManager.OnCurrentUserUpdate += CurrentUserUpdated;
        }

        private void CurrentUserUpdated()
        {
            User = _userManager.GetCurrentUser();
        }

        internal void SetActivity(Activity activity)
        {
            _discordInstance.ClearActivity();
            if (!_pluginConfig.Enabled)
            {
                return;
            }
            _discordInstance.UpdateActivity(activity);
        }

        public void Dispose()
        {
            if (DiscordManager.IsSingletonAvailable && DiscordCore.UI.Settings.IsSingletonAvailable)
            {
                _discordInstance.DestroyInstance();
            }
            _userManager.OnCurrentUserUpdate -= CurrentUserUpdated;
        }

        internal void ClearActivity()
        {
            _discordInstance.ClearActivity();
        }
    }
}