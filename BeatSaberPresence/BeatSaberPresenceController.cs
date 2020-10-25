using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;
using DiscordRPC.Logging;
using IPA.Utilities;
using UnityEngine;
using BeatSaberPresence.Config;
using static BeatSaberAPI.DataTransferObjects.LevelScoreResult;

namespace BeatSaberPresence {
    public class BeatSaberPresenceController : MonoBehaviour {
        public static BeatSaberPresenceController Instance { get; private set; }

        public string clientId = "708741346403287113";
        public string steamId = "620980";

        public bool inGame = false;

        public DiscordRpcClient client = null;

        public User user;

        private GameplayCoreSceneSetupData gameplayCoreSceneSetupData;

        private void Awake() {
            if (Instance != null) {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            Instance = this;
        }

        private void Start() {

        }

        private void Update() {
            client.Invoke();
        }

        private void LateUpdate() {

        }

        private void OnEnable() {
            client = new DiscordRpcClient(
                applicationID: clientId,
                autoEvents: false,
                client: new DiscordRPC.Unity.UnityNamedPipe(),
                logger: new FileLogger("./discordrpc.log") { Level = LogLevel.Warning }
            );

            client.OnReady += (sender, e) => {
                user = e.User;
                Logger.log?.Info($"Received Ready from user {e.User.Username}");
                menuPresence();
            };

            client.OnPresenceUpdate += (sender, e) => Logger.log?.Info($"Received Update! {e.Presence}");

            client.OnError += (sender, e) => Logger.log?.Info($"Error: {e.Message}");

            client.OnConnectionEstablished += (sender, e) => Logger.log?.Info("Established Connection");

            client.OnConnectionFailed += (sender, e) => Logger.log?.Info("Failed Connection");

            client.Initialize();

            Logger.log?.Info("Discord RPC initalized");

            BS_Utils.Utilities.BSEvents.menuSceneLoaded += menuPresence;

            BS_Utils.Utilities.BSEvents.gameSceneLoaded += gamePresence;

            Logger.log?.Info("BS_Utils handles initalized");
        }

        private void menuPresence() {
            inGame = false;
            Logger.log?.Info("Discord RPC set to menu presence");

            RichPresence richPresence = new RichPresence() {
                Details = formatRpcString(PluginConfig.Instance.MenuTopLine),
                State = formatRpcString(PluginConfig.Instance.MenuBottomLine)
            };

            if (PluginConfig.Instance.ShowTimes) {
                richPresence.Timestamps = new Timestamps() {
                    Start = DateTime.UtcNow
                };
            }

            if (PluginConfig.Instance.ShowImages) {
                richPresence.Assets = new Assets() {
                    LargeImageKey = "beat_saber_logo",
                    LargeImageText = PluginConfig.Instance.MenuLargeImageLine
                };
                
                if (PluginConfig.Instance.ShowSmallImages) {
                    richPresence.Assets.SmallImageKey = "beat_saber_saber";
                    richPresence.Assets.SmallImageText = PluginConfig.Instance.MenuSmallImageLine;
                }
            }

            client.SetPresence(richPresence);
        }

        private void gamePresence() {
            inGame = true;
            Logger.log?.Info("Discord RPC set to game presence");

            RichPresence richPresence = new RichPresence() {
                Details = formatRpcString(PluginConfig.Instance.GameTopLine),
                State = formatRpcString(PluginConfig.Instance.GameBottomLine),
            };

            gameplayCoreSceneSetupData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;

            IDifficultyBeatmap diff = gameplayCoreSceneSetupData.difficultyBeatmap;
            IBeatmapLevel level = diff.level;

            if (PluginConfig.Instance.ShowTimes) {
                richPresence.Timestamps = new Timestamps() {
                    Start = DateTime.UtcNow
                };

                if (PluginConfig.Instance.InGameCountDown) {
                    richPresence.Timestamps.End = DateTime.UtcNow.AddSeconds(level.beatmapLevelData.audioClip.length);
                }
            }

            if (PluginConfig.Instance.ShowImages) {
                richPresence.Assets = new Assets() {
                    LargeImageKey = "beat_saber_logo",
                    LargeImageText = PluginConfig.Instance.GameLargeImageLine
                };

                if (PluginConfig.Instance.ShowSmallImages) {
                    richPresence.Assets.SmallImageKey = "beat_saber_block";
                    richPresence.Assets.SmallImageText = PluginConfig.Instance.GameSmallImageLine;
                }
            }

            client.SetPresence(richPresence);
        }

        public void setPresence() {
            if (inGame) gamePresence();
            else menuPresence();
        }

        public void clearPresence() {
            client.ClearPresence();
        }

        private string formatRpcString(string rpcString) {
            string formattedString = rpcString;

            formattedString = formattedString.Replace("{DiscordName}", user.Username);
            formattedString = formattedString.Replace("{DiscordDiscriminator}", user.Discriminator.ToString());

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

        private void OnDisable() {
            client.Dispose();
            client = null;
            Logger.log?.Info("Discord RPC disposed");
        }

        private void OnDestroy() {
            Instance = null;
        }
    }
}
