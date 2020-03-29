using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Drawables {
    public abstract class Drawable {
        public static bool MouseOver { get; private set; }

        public Rect Bounds { get; set; } = new Rect();
        public Point Position => Bounds.TopLeft;

        public virtual void Update(float fElapsedTime) { }
        public virtual void Draw() { }
    }
}
