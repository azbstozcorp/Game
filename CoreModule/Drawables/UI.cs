using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;

namespace CoreModule.Drawables {
    [Serializable]
    public class Button : Drawable {
        public delegate void ButtonPressedEventHandler();
        public event ButtonPressedEventHandler ButtonPressed;

        public string Text { get; set; }

        public Button(string text, int centerX, int centerY) {
            Text = text;

            Bounds = new Rect(new Point(centerX - text.Length * 8 / 2, centerY - 8 / 2), text.Length * 8, 8);
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            if (Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true))
                if (CoreGame.Instance.GetMouse(PixelEngine.Mouse.Any).Down) ButtonPressed?.Invoke();
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.DrawText(Bounds.TopLeft, Text, PixelEngine.Pixel.Presets.White);
            if (Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true))
                CoreGame.Instance.DrawRect(Bounds.TopLeft - 1, Bounds.BottomRight + 1, CoreGame.Instance.Random(PixelEngine.Pixel.PresetPixels));
        }
    }
}
