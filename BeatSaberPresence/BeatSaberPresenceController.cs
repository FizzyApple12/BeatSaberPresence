using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordCore;
using IPA.Utilities;
using UnityEngine;
using BeatSaberPresence.Config;
using static BeatSaberAPI.DataTransferObjects.LevelScoreResult;
using Discord;
using Zenject;

namespace BeatSaberPresence {
    public class BeatSaberPresenceController {
        public static BeatSaberPresenceController Instance { get; private set; }

        public long clientId = 708741346403287113;

        public bool inGame = false;

        public DiscordInstance discordInstance = null;
        public User discordUser;

        private GameplayCoreSceneSetupData gameplayCoreSceneSetupData;

        public void Initialize() {
            if (Instance != null) {
                Logger.log?.Info("BeatSaberPresenceController has already been initalized, stopping");
                return;
            }
            Instance = this;

            discordInstance = DiscordManager.instance.CreateInstance(new DiscordSettings() {
                modId = "BeatSaberPresence",
                modName = "BeatSaberPresence",
                appId = clientId,
                handleInvites = false
            });

            DiscordClient.GetUserManager().OnCurrentUserUpdate += updateUser;

            Logger.log?.Info("Discord RPC initalized");

            BS_Utils.Utilities.BSEvents.menuSceneLoaded += menuPresence;

            BS_Utils.Utilities.BSEvents.gameSceneLoaded += gamePresence;

            BS_Utils.Utilities.BSEvents.songPaused += pausePresence;
            BS_Utils.Utilities.BSEvents.songUnpaused += gamePresence;

            Logger.log?.Info("BS_Utils handles initalized");
        }

        private void updateUser() {
            discordUser = DiscordClient.GetUserManager().GetCurrentUser();

            setPresence();
        }

        private void menuPresence() {
            inGame = false;
            Logger.log?.Info("Discord RPC set to menu presence");

            Activity activity = new Activity() {
                Details = formatRpcString(PluginConfig.Instance.MenuTopLine),
                State = formatRpcString(PluginConfig.Instance.MenuBottomLine)
            };

            if (PluginConfig.Instance.ShowTimes) {
                activity.Timestamps = new ActivityTimestamps() {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
            }

            if (PluginConfig.Instance.ShowImages) {
                activity.Assets = new ActivityAssets() {
                    LargeImage = "beat_saber_logo",
                    LargeText = formatRpcString(PluginConfig.Instance.MenuLargeImageLine),
                };

                if (PluginConfig.Instance.ShowSmallImages) {
                    activity.Assets.SmallImage = "beat_saber_block";
                    activity.Assets.SmallText = formatRpcString(PluginConfig.Instance.MenuSmallImageLine);
                }
            }

            discordInstance.UpdateActivity(activity);
        }

        private void pausePresence() {
            inGame = true;
            Logger.log?.Info("Discord RPC set to pause presence");

            Activity activity = new Activity() {
                Details = formatRpcString(PluginConfig.Instance.PauseTopLine),
                State = formatRpcString(PluginConfig.Instance.PauseBottomLine),
            };

            gameplayCoreSceneSetupData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;

            IDifficultyBeatmap diff = gameplayCoreSceneSetupData.difficultyBeatmap;
            IBeatmapLevel level = diff.level;

            if (PluginConfig.Instance.ShowTimes) {
                activity.Timestamps = new ActivityTimestamps() {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
            }

            if (PluginConfig.Instance.ShowImages) {
                activity.Assets = new ActivityAssets() {
                    LargeImage = "beat_saber_logo",
                    LargeText = formatRpcString(PluginConfig.Instance.PauseLargeImageLine),
                };

                if (PluginConfig.Instance.ShowSmallImages) {
                    activity.Assets.SmallImage = "beat_saber_block";
                    activity.Assets.SmallText = formatRpcString(PluginConfig.Instance.PauseSmallImageLine);
                }
            }

            discordInstance.UpdateActivity(activity);
        }

        private void gamePresence() {
            inGame = true;
            Logger.log?.Info("Discord RPC set to game presence");

            Activity activity = new Activity() {
                Details = formatRpcString(PluginConfig.Instance.GameTopLine),
                State = formatRpcString(PluginConfig.Instance.GameBottomLine),
            };

            gameplayCoreSceneSetupData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;

            IDifficultyBeatmap diff = gameplayCoreSceneSetupData.difficultyBeatmap;
            IBeatmapLevel level = diff.level;

            if (PluginConfig.Instance.ShowTimes) {
                activity.Timestamps = new ActivityTimestamps() {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };

                if (PluginConfig.Instance.InGameCountDown) {
                    activity.Timestamps.End = DateTimeOffset.UtcNow.AddSeconds(ATSCHolder.Instance.audioTimeSyncController.songLength - ATSCHolder.Instance.audioTimeSyncController.songTime).ToUnixTimeMilliseconds();
                }
            }

            if (PluginConfig.Instance.ShowImages) {
                activity.Assets = new ActivityAssets() {
                    LargeImage = "beat_saber_logo",
                    LargeText = formatRpcString(PluginConfig.Instance.GameLargeImageLine),
                };

                if (PluginConfig.Instance.ShowSmallImages) {
                    activity.Assets.SmallImage = "beat_saber_block";
                    activity.Assets.SmallText = formatRpcString(PluginConfig.Instance.GameSmallImageLine);
                }
            }

            discordInstance.UpdateActivity(activity);
        }

        public void setPresence() {
            if (inGame) gamePresence();
            else menuPresence();
        }

        public void clearPresence() {
            discordInstance.ClearActivity();
        }

        private string formatRpcString(string rpcString) {
            string formattedString = rpcString;

            formattedString = formattedString.Replace("{DiscordName}", discordUser.Username);
            formattedString = formattedString.Replace("{DiscordDiscriminator}", discordUser.Discriminator);

            if (!inGame) return formattedString;

            gameplayCoreSceneSetupData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;

            IDifficultyBeatmap diff = gameplayCoreSceneSetupData.difficultyBeatmap;
            IBeatmapLevel level = diff.level;
            GameplayModifiers gameplayModifiers = gameplayCoreSceneSetupData.gameplayModifiers;

            TimeSpan totalTime = new TimeSpan(0, 0, (int) Math.Floor(level.beatmapLevelData.audioClip.length));

            formattedString = formattedString.Replace("{SongName}", level.songName);
            formattedString = formattedString.Replace("{SongSubName}", level.songSubName);
            formattedString = formattedString.Replace("{SongAuthorName}", level.songAuthorName);
            formattedString = formattedString.Replace("{SongDuration}", totalTime.ToString(@"mm\:ss"));
            formattedString = formattedString.Replace("{SongDurationSeconds}", Math.Floor(level.beatmapLevelData.audioClip.length).ToString());
            formattedString = formattedString.Replace("{LevelAuthorName}", level.levelAuthorName);
            formattedString = formattedString.Replace("{Difficulty}", diff.difficulty.Name());
            formattedString = formattedString.Replace("{SongBPM}", level.beatsPerMinute.ToString());
            formattedString = formattedString.Replace("{LevelID}", level.levelID);
            formattedString = formattedString.Replace("{EnvironmentName}", level.environmentInfo.environmentName);
            formattedString = formattedString.Replace("{Submission}", (BS_Utils.Gameplay.ScoreSubmission.Disabled) ? "Disabled" : "Enabled");

            formattedString = formattedString.Replace("{NoFail}", (gameplayModifiers.noFail) ? "On" : "Off");
            formattedString = formattedString.Replace("{NoBombs}", (gameplayModifiers.noBombs) ? "On" : "Off");
            formattedString = formattedString.Replace("{NoObsticles}", (gameplayModifiers.enabledObstacleType == GameplayModifiers.EnabledObstacleType.NoObstacles) ? "On" : "Off");
            formattedString = formattedString.Replace("{NoArrows}", (gameplayModifiers.noArrows) ? "On" : "Off");
            formattedString = formattedString.Replace("{SlowerSong}", (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Slower) ? "On" : "Off");
            formattedString = formattedString.Replace("{InstaFail}", (gameplayModifiers.instaFail) ? "On" : "Off");
            formattedString = formattedString.Replace("{BatteryEnergy}", (gameplayModifiers.energyType == GameplayModifiers.EnergyType.Battery) ? "On" : "Off");
            formattedString = formattedString.Replace("{GhostNotes}", (gameplayModifiers.ghostNotes) ? "On" : "Off");
            formattedString = formattedString.Replace("{DisappearingArrows}", (gameplayModifiers.disappearingArrows) ? "On" : "Off");
            formattedString = formattedString.Replace("{FasterSong}", (gameplayModifiers.songSpeed == GameplayModifiers.SongSpeed.Faster) ? "On" : "Off");

            return formattedString;
        }

        public void Dispose() {
            discordInstance.DestroyInstance();
            discordInstance = null;
            Instance = null;
            Logger.log?.Info("Discord RPC disposed");
        }
    }
}
