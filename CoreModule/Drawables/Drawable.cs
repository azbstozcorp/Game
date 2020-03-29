using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Drawables {
    public abstract class Drawable {
        public delegate void MouseMovedEventHandler(Point mouse);
        public static event MouseMovedEventHandler MouseMoved;
        public delegate void MouseDownEventHandler(Point mouse);
        public static event MouseDownEventHandler MouseDown;
        public delegate void MouseUpEventHandler(Point mouse);
        public static event MouseUpEventHandler MouseUp;

        public static bool MouseOver { get; private set; }

        static Point lastMouse = new Point();

        public Rect Bounds { get; set; } = new Rect();
        public Point Position => Bounds.TopLeft;

        public virtual void Update(float fElapsedTime) {
            Point mouse = new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY);

            if (Collision.WithinRect(Bounds, mouse, true))
                MouseOver = true;
            else MouseOver = false;

            if (mouse != lastMouse && MouseOver) MouseMoved?.Invoke(mouse);
            if (CoreGame.Instance.GetMouse(PixelEngine.Mouse.Any).Down && MouseOver) MouseDown?.Invoke(mouse);
            if (CoreGame.Instance.GetMouse(PixelEngine.Mouse.Any).Up && MouseOver) MouseUp?.Invoke(mouse);

            lastMouse = mouse;
        }
        public virtual void Draw(CoreGame instance) { }
    }
}
