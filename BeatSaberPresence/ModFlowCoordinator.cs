using HMUI;
using Zenject;
using BeatSaberMarkupLanguage;

namespace BeatSaberPresence {
    class ModFlowCoordinator : FlowCoordinator {
        private Settings settings;
        private MainFlowCoordinator mainFlowCoordinator;

        [Inject]
        public void Construct(Settings settings, MainFlowCoordinator mainFlowCoordinator) {
            this.settings = settings;
            this.mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, bool _, bool __) {
            if (firstActivation) {
                SetTitle(Plugin.Name);
                showBackButton = true;
            }
            ProvideInitialViewControllers(settings);
        }

        protected override void BackButtonWasPressed(ViewController _) {
            mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}