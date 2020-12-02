using System;
using IPA.Config.Stores;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo(GeneratedStore.AssemblyVisibilityTarget)]
namespace BeatSaberPresence.Config {
    internal class PluginConfig
    {
        internal event Action<PluginConfig> Reloaded;

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

        public virtual string PauseTopLine { get; set; } = "{SongName} - {SongAuthorName}";
        public virtual string PauseBottomLine { get; set; } = "{Difficulty} (Paused)";
        public virtual string PauseLargeImageLine { get; set; } = "Beat Saber";
        public virtual string PauseSmallImageLine { get; set; } = "In Game (Paused)";

        public virtual void Changed()
        {
            Reloaded?.Invoke(this);
        }
    }
}