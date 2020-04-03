using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;

namespace CoreModule.Drawables {
    public abstract class Drawable {
        public static bool MouseOver { get; private set; }
        public List<Drawable> Children { get; } = new List<Drawable>();

        public int X {
            get => Bounds.Left; set {
                Bounds.Right = Bounds.Width + value;
                Bounds.Left = value;
            }
        }
        public int Y {
            get => Bounds.Top; set {
                Bounds.Bottom = Bounds.Height + value;
                Bounds.Top = value;
            }
        }

        public Rect Bounds { get; set; } = new Rect();
        public Point Position => Bounds.TopLeft;

        public virtual void Update(float fElapsedTime) {
            foreach (Drawable child in Children) child.Update(fElapsedTime);
        }
        public virtual void Draw() {
            foreach (Drawable child in Children) child.Draw();
        }
    }
}
