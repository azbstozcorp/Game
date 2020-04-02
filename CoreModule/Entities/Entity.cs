using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;

namespace CoreModule.Entities {
    public abstract class Entity : Drawables.Drawable {
        public float X {
            get => this.Bounds.Left; set {
                Bounds.Right = value + Bounds.Width;
                Bounds.Left = value;
            }
        }
        public float Y {
            get => this.Bounds.Top; set {
                Bounds.Bottom = value + Bounds.Height;
                Bounds.Top = value;
            }
        }

        public new RectF Bounds { get; set; } = new RectF();
        public new PointF Position => Bounds.TopLeft;
    }
}
