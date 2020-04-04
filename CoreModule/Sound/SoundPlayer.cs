using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using IrrKlang;

namespace CoreModule.Sound {
    public static class SoundPlayer {
        static ISoundEngine engine;

        public static void Init() {
            engine = new ISoundEngine();
        }

        public static void PlaySound(string name) {
            engine.Play2D(name);
        }
    }
}
