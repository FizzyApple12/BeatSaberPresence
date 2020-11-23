using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace BeatSaberPresence {
    class ATSCHolder : IInitializable, IDisposable {
        public static ATSCHolder Instance { get; private set; }

        [Inject] public AudioTimeSyncController audioTimeSyncController;

        public void Initialize() {
            if (Instance != null) {
                Logger.log?.Info("ATSCHolder has already been initalized, stopping");
                return;
            }
            Instance = this;
        }

        public void Dispose() {
            Instance = null;
        }
    }
}
