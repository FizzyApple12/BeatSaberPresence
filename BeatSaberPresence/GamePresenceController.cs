using System;
using Discord;
using Zenject;
using BeatSaberPresence.Config;
using SiraUtil.Submissions;

namespace BeatSaberPresence {
    internal class GamePresenceManager : IInitializable, ILateDisposable {
        private Activity? gameActivity;
        private Activity? pauseActivity;

        private readonly IGamePause gamePause;
        private readonly Submission submission;
        private readonly PluginConfig pluginConfig;
        private readonly PresenceController presenceController;
        private readonly AudioTimeSyncController audioTimeSyncController;
        private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData;

        internal GamePresenceManager([InjectOptional] IGamePause gamePause, [InjectOptional] Submission submission, PluginConfig pluginConfig, PresenceController presenceController, AudioTimeSyncController audioTimeSyncController, GameplayCoreSceneSetupData gameplayCoreSceneSetupData) {
            this.gamePause = gamePause;
            this.submission = submission;
            this.pluginConfig = pluginConfig;
            this.presenceController = presenceController;
            this.audioTimeSyncController = audioTimeSyncController;
            this.gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
        }

        public void Initialize() {
            if (gamePause != null) {
                gamePause.didPauseEvent += DidPause;
                gamePause.didResumeEvent += DidResume;
            }
            pluginConfig.Reloaded += ConfigReloaded;
            DidResume();
        }

        private void DidPause() {
            Set(true);
        }

        private void DidResume() {
            Set(false);
        }

        public void LateDispose() {
            if (gamePause != null) {
                gamePause.didPauseEvent -= DidPause;
                gamePause.didResumeEvent -= DidResume;
            }
            pluginConfig.Reloaded -= ConfigReloaded;
        }

        private void ConfigReloaded(PluginConfig _) {
            gameActivity = RebuildActivity();
            pauseActivity = RebuildActivity(true);
            Set();
        }

        private void Set(bool isPaused = false) {
            if (isPaused) {
                if (!pauseActivity.HasValue) pauseActivity = RebuildActivity(true);

                presenceController.SetActivity(pauseActivity.Value);
            } else {
                if (!gameActivity.HasValue) gameActivity = RebuildActivity(false);

                if (pluginConfig.InGameCountDown) {
                    Activity activity = gameActivity.Value;
                    ActivityTimestamps timestamps = gameActivity.Value.Timestamps;
                    timestamps.End = DateTimeOffset.UtcNow.AddSeconds(audioTimeSyncController.songLength - audioTimeSyncController.songTime).ToUnixTimeMilliseconds();
                    activity.Timestamps = timestamps;
                    gameActivity = activity;
                }
                presenceController.SetActivity(gameActivity.Value);
            }
        }

        private Activity RebuildActivity(bool paused = false) {
            Activity activity = new Activity {
                Details = Format(paused ? pluginConfig.PauseTopLine : pluginConfig.GameTopLine),
                State = Format(paused ? pluginConfig.PauseBottomLine : pluginConfig.GameBottomLine),
            };

            if (pluginConfig.ShowTimes) {
                activity.Timestamps = new ActivityTimestamps {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
            }

            if (pluginConfig.ShowImages) {
                activity.Assets = new ActivityAssets {
                    LargeImage = "beat_saber_logo",
                    LargeText = Format(paused ? pluginConfig.PauseLargeImageLine : pluginConfig.GameLargeImageLine)
                };

                if (pluginConfig.ShowSmallImages) {
                    activity.Assets.SmallImage = "beat_saber_block";
                    activity.Assets.SmallText = Format(paused ? pluginConfig.PauseSmallImageLine : pluginConfig.GameSmallImageLine);
                }
            }

            return activity;
        }

        private string Format(string rpcString) {
            string formattedString = rpcString;

            if (presenceController.User != null) formattedString = formattedString.Replace("{DiscordName}", presenceController.User.Value.Username);
            if (presenceController.User != null) formattedString = formattedString.Replace("{DiscordDiscriminator}", presenceController.User.Value.Discriminator);

            IDifficultyBeatmap diff = gameplayCoreSceneSetupData.difficultyBeatmap;
            GameplayModifiers gameplayModifiers = gameplayCoreSceneSetupData.gameplayModifiers;
            IBeatmapLevel level = diff.level;

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
            formattedString = formattedString.Replace("{Submission}", submission != null ? (submission.Tickets().Length == 0) ? "Disabled" : "Enabled" : "Disabled");


            formattedString = formattedString.Replace("{NoFail}", (gameplayModifiers.noFailOn0Energy) ? "On" : "Off");
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
    }
}