using HMUI;
using BeatSaberMarkupLanguage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeatSaberPresence {
    class ModFlowCoordinator : FlowCoordinator {
        private Settings settings;
        protected override void DidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling) {
            if (firstActivation) {
                SetTitle("BeatSaberPresence");
                showBackButton = true;
            }

            settings = BeatSaberUI.CreateViewController<Settings>();
            ProvideInitialViewControllers(settings);
        }

        protected override void BackButtonWasPressed(ViewController topViewController) {
            BeatSaberUI.MainFlowCoordinator.DismissFlowCoordinator(this);
        }
    }
}
