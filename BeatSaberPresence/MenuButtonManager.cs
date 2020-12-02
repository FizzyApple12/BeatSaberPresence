using System;
using Zenject;
using BeatSaberMarkupLanguage;
using BeatSaberMarkupLanguage.MenuButtons;

namespace BeatSaberPresence
{
    internal class MenuButtonManager : IInitializable, IDisposable
    {
        private readonly MenuButton _menuButton;
        private readonly ModFlowCoordinator _modFlowCoordinator;
        private readonly MainFlowCoordinator _mainFlowCoordinator;

        internal MenuButtonManager(ModFlowCoordinator modFlowCoordinator, MainFlowCoordinator mainFlowCoordinator)
        {
            _modFlowCoordinator = modFlowCoordinator;
            _mainFlowCoordinator = mainFlowCoordinator;
            _menuButton = new MenuButton("BeatSaberPresence", "", SummonFlowCoordinator);
        }

        public void Initialize()
        {
            MenuButtons.instance.RegisterButton(_menuButton);
        }

        public void Dispose()
        {
            if (MenuButtons.IsSingletonAvailable && BSMLParser.IsSingletonAvailable)
            {
                MenuButtons.instance.UnregisterButton(_menuButton);
            }
        }

        private void SummonFlowCoordinator()
        {
            _mainFlowCoordinator.PresentFlowCoordinator(_modFlowCoordinator);
        }
    }
}