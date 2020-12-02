using HMUI;
using Zenject;
using BeatSaberMarkupLanguage;

namespace BeatSaberPresence
{
    class ModFlowCoordinator : FlowCoordinator
    {
        private Settings _settings;
        private MainFlowCoordinator _mainFlowCoordinator;

        [Inject]
        public void Construct(Settings settings, MainFlowCoordinator mainFlowCoordinator)
        {
            _settings = settings;
            _mainFlowCoordinator = mainFlowCoordinator;
        }

        protected override void DidActivate(bool firstActivation, bool _, bool __)
        {
            if (firstActivation)
            {
                SetTitle(Plugin.Name);
                showBackButton = true;
            }
            ProvideInitialViewControllers(_settings);
        }

        protected override void BackButtonWasPressed(ViewController _)
        {
            _mainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}