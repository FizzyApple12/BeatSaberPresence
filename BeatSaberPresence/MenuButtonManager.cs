using System;
using Zenject;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace BeatSaberPresence {
    internal class MenuButtonManager : IInitializable, IDisposable {
        private readonly MenuButton menuButton;
        private readonly ModFlowCoordinator modFlowCoordinator;
        private readonly MainFlowCoordinator mainFlowCoordinator;

        internal MenuButtonManager(ModFlowCoordinator modFlowCoordinator, MainFlowCoordinator mainFlowCoordinator) {
            this.modFlowCoordinator = modFlowCoordinator;
            this.mainFlowCoordinator = mainFlowCoordinator;
            menuButton = new MenuButton("BeatSaberPresence", "", SummonFlowCoordinator);
        }

        public void Initialize() {
            MenuButtons.instance.RegisterButton(menuButton);
        }

        public void Dispose() {
            if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable) MenuButtons.instance.UnregisterButton(menuButton);
        }

        private void SummonFlowCoordinator() {
            mainFlowCoordinator.PresentFlowCoordinator(modFlowCoordinator);
        }
    }
}