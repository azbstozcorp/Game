using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule;
using CoreModule.Drawables;

namespace CoreModule.Guns {
    public class Gun : Drawable{
        public Gun() { }
        
        public void Fire() {

        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);


        }

        public override void Draw() {
            base.Draw();
        }
    }

    public class GunPart : Drawable {
        public Animator Animator { get; protected set; }
    }
}
