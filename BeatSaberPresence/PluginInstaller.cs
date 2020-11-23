using IPA.Loader;
using System;
using UnityEngine;
using Zenject;

namespace BeatSaberPresence {
    class PluginInstaller : Installer {
        public override void InstallBindings() {
            Container.BindInterfacesAndSelfTo<ATSCHolder>().AsSingle().NonLazy();
        }
    }
}
