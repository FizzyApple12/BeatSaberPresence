using System.Runtime.CompilerServices;
using IPA.Config.Stores;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatSaberPresence.Config {
    internal class PluginConfig {
        public static PluginConfig Instance { get; set; }

        public virtual bool Enabled { get; set; } = true;

        public virtual bool ShowImages { get; set; } = true;
        public virtual bool ShowSmallImages { get; set; } = true;

        public virtual bool ShowTimes { get; set; } = true;
        public virtual bool InGameCountDown { get; set; } = true;

        public virtual string MenuTopLine { get; set; } = "Main Menu";
        public virtual string MenuBottomLine { get; set; } = "";
        public virtual string MenuLargeImageLine { get; set; } = "Beat Saber";
        public virtual string MenuSmallImageLine { get; set; } = "In the Menus";

        public virtual string GameTopLine { get; set; } = "{SongName} - {SongAuthorName}";
        public virtual string GameBottomLine { get; set; } = "{Difficulty}";
        public virtual string GameLargeImageLine { get; set; } = "Beat Saber";
        public virtual string GameSmallImageLine { get; set; } = "In Game";

        public virtual void OnReload() {
            if (BeatSaberPresenceController.Instance != null) BeatSaberPresenceController.Instance.setPresence();
        }

        public virtual void Changed() {
            if (BeatSaberPresenceController.Instance != null) BeatSaberPresenceController.Instance.setPresence();
        }

        public virtual void CopyFrom(PluginConfig other) {

        }
    }
}
