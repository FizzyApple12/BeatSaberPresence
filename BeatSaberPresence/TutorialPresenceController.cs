using System;
using Discord;
using Zenject;
using BeatSaberPresence.Config;
using SiraUtil.Submissions;

namespace BeatSaberPresence {
    internal class TutorialPresenceManager : IInitializable, ILateDisposable {
        private Activity? gameActivity;
        private Activity? pauseActivity;

        private readonly IGamePause gamePause;
        private readonly PluginConfig pluginConfig;
        private readonly PresenceController presenceController;
        private readonly AudioTimeSyncController audioTimeSyncController;

        internal TutorialPresenceManager([InjectOptional] IGamePause gamePause, PluginConfig pluginConfig, PresenceController presenceController, AudioTimeSyncController audioTimeSyncController) {
            this.gamePause = gamePause;
            this.pluginConfig = pluginConfig;
            this.presenceController = presenceController;
            this.audioTimeSyncController = audioTimeSyncController;
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

            TimeSpan totalTime = new TimeSpan(0, 0, (int) Math.Floor(audioTimeSyncController.songLength));

            formattedString = formattedString.Replace("{SongName}", "Tutorial");
            formattedString = formattedString.Replace("{SongSubName}", "Learning to play the game");
            formattedString = formattedString.Replace("{SongAuthorName}", "Jarslov Beck");
            formattedString = formattedString.Replace("{SongDuration}", totalTime.ToString(@"mm\:ss"));
            formattedString = formattedString.Replace("{SongDurationSeconds}", Math.Floor(audioTimeSyncController.songLength).ToString());
            formattedString = formattedString.Replace("{LevelAuthorName}", "Beat Games");
            formattedString = formattedString.Replace("{Difficulty}", "Tutorial");
            formattedString = formattedString.Replace("{SongBPM}", "115");
            formattedString = formattedString.Replace("{LevelID}", "Tutorial");
            formattedString = formattedString.Replace("{EnvironmentName}", "Tutorial");
            formattedString = formattedString.Replace("{Submission}", "Disabled");


            formattedString = formattedString.Replace("{NoFail}", "On");
            formattedString = formattedString.Replace("{NoBombs}", "Off");
            formattedString = formattedString.Replace("{NoObsticles}", "Off");
            formattedString = formattedString.Replace("{NoArrows}", "Off");
            formattedString = formattedString.Replace("{SlowerSong}", "Off");
            formattedString = formattedString.Replace("{InstaFail}", "Off");
            formattedString = formattedString.Replace("{BatteryEnergy}", "Off");
            formattedString = formattedString.Replace("{GhostNotes}", "Off");
            formattedString = formattedString.Replace("{DisappearingArrows}", "Off");
            formattedString = formattedString.Replace("{FasterSong}", "Off");

            return formattedString;
        }
    }
}