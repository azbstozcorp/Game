
using CoreModule.Shapes;

namespace CoreModule.Drawables {
    public class Button : Drawable {
        public delegate void ButtonPressedEventHandler(Button sender);
        public event ButtonPressedEventHandler Pressed;

        public string Text { get; set; }
        public bool IsPressed { get; private set; }

        public Button(string text, int centerX, int centerY) {
            Text = text;

            Bounds = new Rect(new Point(centerX - text.Length * 8 / 2, centerY - 8 / 2), text.Length * 8, 8);
        }

        public void Press() => Pressed?.Invoke(this);

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            IsPressed = false;
            if (Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true)) {
                if (CoreGame.Instance.GetMouse(PixelEngine.Mouse.Any).Pressed) Pressed?.Invoke(this);
                IsPressed = true;
            }
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.DrawText(Bounds.TopLeft, Text, PixelEngine.Pixel.Presets.White);
            if (Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true))
                CoreGame.Instance.DrawRect(Bounds.TopLeft - 1, Bounds.BottomRight + 1, CoreGame.Instance.Random(PixelEngine.Pixel.PresetPixels));
        }
    }
}
