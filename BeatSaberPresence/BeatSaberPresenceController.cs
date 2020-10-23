using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS_Utils;
using BS_Utils.Gameplay;
using DiscordRPC;
using DiscordRPC.Logging;
using UnityEngine;

namespace BeatSaberPresence {
    public class BeatSaberPresenceController : MonoBehaviour {
        public static BeatSaberPresenceController instance { get; private set; }

        public string clientId = "708741346403287113";
        public string steamId = "620980";

        public DiscordRpcClient client;

        private GameplayCoreSceneSetupData gameplayCoreSceneSetupData;

        private void Awake() {
            if (instance != null) {
                Logger.log?.Warn($"Instance of {this.GetType().Name} already exists, destroying.");
                GameObject.DestroyImmediate(this);
                return;
            }
            GameObject.DontDestroyOnLoad(this);
            instance = this;
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
                client: new DiscordRPC.Unity.UnityNamedPipe()
            );

            client.Logger = new FileLogger("./discordrpc.log") { Level = LogLevel.Warning };

            client.OnReady += (sender, e) =>
            {
                Logger.log?.Info($"Received Ready from user {e.User.Username}");
            };

            client.OnPresenceUpdate += (sender, e) => {
                Logger.log?.Info($"Received Update! {e.Presence}");
            };

            client.OnError += (sender, e) => {
                Logger.log?.Info($"Error: {e.Message}");
            };

            client.OnConnectionEstablished += (sender, e) => {
                Logger.log?.Info("Established Connection");
            };

            client.OnConnectionFailed += (sender, e) => {
                Logger.log?.Info("Failed Connection");
            };

            client.Initialize();
            Logger.log?.Info("Discord RPC initalized");

            menuSceneLoaded();

            BS_Utils.Utilities.BSEvents.menuSceneLoaded += menuSceneLoaded;

            BS_Utils.Utilities.BSEvents.gameSceneLoaded += gameSceneLoaded;

            Logger.log?.Info("BS_Utils handles initalized");
        }

        private void menuSceneLoaded() {
            Logger.log?.Info("Discord RPC set to Main Menu");
            client.SetPresence(new RichPresence() {
                Details = "Main Menu",
                State = "",
                Timestamps = new Timestamps() {
                    Start = DateTime.UtcNow
                },
                Assets = new Assets() {
                    LargeImageKey = "beat_saber_logo",
                    LargeImageText = "Beat Saber",
                    SmallImageKey = "beat_saber_saber",
                    SmallImageText = "On the Menu"
                }
            });
        }

        private void gameSceneLoaded() {
            Logger.log?.Info("Discord RPC set to In Game");

            gameplayCoreSceneSetupData = BS_Utils.Plugin.LevelData.GameplayCoreSceneSetupData;

            IDifficultyBeatmap diff = gameplayCoreSceneSetupData.difficultyBeatmap;
            IBeatmapLevel level = diff.level;

            client.SetPresence(new RichPresence() {
                Details = $"{level.songName} - {level.songAuthorName}",
                State = $"{diff.difficulty.Name()}",
                Timestamps = new Timestamps() {
                    Start = DateTime.UtcNow
                },
                Assets = new Assets() {
                    LargeImageKey = "beat_saber_logo",
                    LargeImageText = "Beat Saber",
                    SmallImageKey = "beat_saber_block",
                    SmallImageText = "In Game"
                }
            });
        }

        private void OnDisable() {
            client.Dispose();
            Logger.log?.Info("Discord RPC disposed");
        }

        private void OnDestroy() {
            instance = null;
        }
    }
}
