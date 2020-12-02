using System;
using Discord;
using Zenject;
using BeatSaberPresence.Config;
using SiraUtil.Services;

namespace BeatSaberPresence
{
    internal class GamePresenceManager : IInitializable, ILateDisposable
    {
        private Activity? _gameActivity;
        private Activity? _pauseActivity;

        private readonly IGamePause _gamePause;
        private readonly Submission _submission;
        private readonly PluginConfig _pluginConfig;
        private readonly PresenceController _presenceController;
        private readonly AudioTimeSyncController _audioTimeSyncController;
        private readonly GameplayCoreSceneSetupData _gameplayCoreSceneSetupData;

        internal GamePresenceManager([InjectOptional] IGamePause gamePause, [InjectOptional] Submission submission, PluginConfig pluginConfig, PresenceController presenceController, AudioTimeSyncController audioTimeSyncController, GameplayCoreSceneSetupData gameplayCoreSceneSetupData)
        {
            _gamePause = gamePause;
            _submission = submission;
            _pluginConfig = pluginConfig;
            _presenceController = presenceController;
            _audioTimeSyncController = audioTimeSyncController;
            _gameplayCoreSceneSetupData = gameplayCoreSceneSetupData;
        }

        public void Initialize()
        {
            if (_gamePause != null)
            {
                _gamePause.didPauseEvent += DidPause;
                _gamePause.didResumeEvent += DidResume;
            }
            _pluginConfig.Reloaded += ConfigReloaded;
            DidResume();
        }

        private void DidPause()
        {
            Set(true);
        }

        private void DidResume()
        {
            Set(false);
        }

        public void LateDispose()
        {
            if (_gamePause != null)
            {
                _gamePause.didPauseEvent -= DidPause;
                _gamePause.didResumeEvent -= DidResume;
            }
            _pluginConfig.Reloaded -= ConfigReloaded;
        }

        private void ConfigReloaded(PluginConfig _)
        {
            _gameActivity = RebuildActivity();
            _pauseActivity = RebuildActivity(true);
            Set();
        }

        private void Set(bool isPaused = false)
        {
            if (isPaused)
            {
                if (!_pauseActivity.HasValue)
                {
                    _pauseActivity = RebuildActivity(true);
                }
                _presenceController.SetActivity(_pauseActivity.Value);
            }
            else
            {
                if (!_gameActivity.HasValue)
                {
                    _gameActivity = RebuildActivity(false);
                }

                if (_pluginConfig.InGameCountDown)
                {
                    Activity activity = _gameActivity.Value;
                    ActivityTimestamps timestamps = _gameActivity.Value.Timestamps;
                    timestamps.End = DateTimeOffset.UtcNow.AddSeconds(_audioTimeSyncController.songLength - _audioTimeSyncController.songTime).ToUnixTimeMilliseconds();
                    activity.Timestamps = timestamps;
                    _gameActivity = activity;
                }
                _presenceController.SetActivity(_gameActivity.Value);
            }
        }

        private Activity RebuildActivity(bool paused = false)
        {
            Activity activity = new Activity
            {
                Details = Format(paused ? _pluginConfig.PauseTopLine : _pluginConfig.GameTopLine),
                State = Format(paused ? _pluginConfig.PauseBottomLine : _pluginConfig.GameBottomLine),
            };
            
            if (_pluginConfig.ShowTimes)
            {
                activity.Timestamps = new ActivityTimestamps
                {
                    Start = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
                };
            }

            if (_pluginConfig.ShowImages)
            {
                activity.Assets = new ActivityAssets
                {
                    LargeImage = "beat_saber_logo",
                    LargeText = Format(paused ? _pluginConfig.PauseLargeImageLine : _pluginConfig.GameLargeImageLine)
                };

                if (_pluginConfig.ShowSmallImages)
                {
                    activity.Assets.SmallImage = "beat_saber_block";
                    activity.Assets.SmallText = Format(paused ? _pluginConfig.PauseSmallImageLine : _pluginConfig.GameSmallImageLine);
                }
            }

            return activity;
        }

        private string Format(string rpcString)
        {
            string formattedString = rpcString;

            formattedString = formattedString.Replace("{DiscordName}", _presenceController.User.Username);
            formattedString = formattedString.Replace("{DiscordDiscriminator}", _presenceController.User.Discriminator);

            IDifficultyBeatmap diff = _gameplayCoreSceneSetupData.difficultyBeatmap;
            GameplayModifiers gameplayModifiers = _gameplayCoreSceneSetupData.gameplayModifiers;
            IBeatmapLevel level = diff.level;

            TimeSpan totalTime = new TimeSpan(0, 0, (int)Math.Floor(level.beatmapLevelData.audioClip.length));

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
            formattedString = formattedString.Replace("{Submission}", _submission != null ? (_submission.Tickets().Length == 0) ? "Disabled" : "Enabled" : "Disabled");


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
    }
}