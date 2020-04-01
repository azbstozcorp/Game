using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Shapes;

namespace CoreModule.Drawables {
    public class Button : Drawable {
        public delegate void ButtonPressedEventHandler();
        public event ButtonPressedEventHandler Pressed;

        public string Text { get; set; }
        public bool IsPressed { get; private set; }

        public Button(string text, int centerX, int centerY) {
            Text = text;

            Bounds = new Rect(new Point(centerX - text.Length * 8 / 2, centerY - 8 / 2), text.Length * 8, 8);
        }

        public void Press() => Pressed?.Invoke();

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            IsPressed = false;
            if (Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true)) {
                if (CoreGame.Instance.GetMouse(PixelEngine.Mouse.Any).Pressed) Pressed?.Invoke();
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

    public class TextBox : Button {
        public delegate void TextChangedEventHandler(string newText);
        public event TextChangedEventHandler TextChanged;

        bool selected = false;
        int location = 0;

        public TextBox(string text, int centerX, int centerY, bool startAtEnd = true) : base(text, centerX, centerY) {
            Pressed += () => selected = true;
            if (startAtEnd) location = text.Length;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            if (selected &&
               !Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true) &&
               CoreGame.Instance.GetMouse(PixelEngine.Mouse.Left).Down) selected = false;

            if (selected) {
                StringBuilder text = new StringBuilder(Text);

                if (CoreGame.Instance.GetKey(PixelEngine.Key.Left).Pressed) {
                    if (CoreGame.Instance.GetKey(PixelEngine.Key.Control).Down) {
                        if (location < text.Length && location > 0 && text[location - 1] == ' ') location--;
                        else while (location > 0) {
                                location--;
                                if (text[location] == ' ') {
                                    location++;
                                    break;
                                }
                            }
                    }
                    else
                        location--;
                }
                if (CoreGame.Instance.GetKey(PixelEngine.Key.Right).Pressed) {
                    if (CoreGame.Instance.GetKey(PixelEngine.Key.Control).Down) {
                        if (location < text.Length - 1 && text[location] == ' ') location++;
                        else while (location < text.Length && text[location] != ' ') location++;
                    }
                    else
                        location++;
                }
                if (CoreGame.Instance.GetKey(PixelEngine.Key.Delete).Pressed) {
                    if (CoreGame.Instance.GetKey(PixelEngine.Key.Control).Down) {
                        if (location < text.Length && text[location] == ' ') text.Remove(location, 1);
                        else
                            while (location < text.Length)
                                if (location + 1 == text.Length)
                                    text.Remove(location, 1);
                                else if (text[location + 1] != ' ')
                                    text.Remove(location, 1);
                    }
                    else text.Remove(location, 1);
                }
                if (CoreGame.Instance.GetKey(PixelEngine.Key.Back).Pressed && location > 0) {
                    if (CoreGame.Instance.GetKey(PixelEngine.Key.Control).Down) {
                        if (location > 0 && text[location - 1] == ' ') text.Remove(--location, 1);
                        else while (location > 0 && text[location - 1] != ' ')
                                text.Remove(--location, 1);
                    }
                    else text.Remove(--location, 1);
                }
                if (CoreGame.Instance.GetKey(PixelEngine.Key.Home).Pressed) location = 0;
                if (CoreGame.Instance.GetKey(PixelEngine.Key.End).Pressed) location = text.Length;

                if (location > text.Length) location = 0;
                if (location < 0) location = Text.Length;

                if (CoreGame.Instance.GetKey(PixelEngine.Key.Any).Pressed) {
                    bool shiftDown = CoreGame.Instance.GetKey(PixelEngine.Key.Shift).Down;

                    for (char i = (char)PixelEngine.Key.A; i <= (char)PixelEngine.Key.Z; i++) {
                        if (CoreGame.Instance.GetKey((PixelEngine.Key)i).Pressed) {
                            text.Insert(location, shiftDown ? char.ToUpper((char)(i + 'a')) : (char)(i + 'a'));
                            location++;
                        }
                    }
                    for (int i = (int)PixelEngine.Key.K0; i <= (int)PixelEngine.Key.K9; i++) {
                        if (CoreGame.Instance.GetKey((PixelEngine.Key)i).Pressed) {
                            text.Insert(location, i - 26);
                            location++;
                        }
                    }
                    if (CoreGame.Instance.GetKey(PixelEngine.Key.Space).Pressed) { text.Insert(location, ' '); location++; }
                }

                int oldWidth = (Text.Length == 0 ? 1 : Text.Length) * 8;
                int newWidth = (text.Length == 0 ? 1 : text.Length) * 8;
                Bounds.Left = (Bounds.Left + oldWidth / 2) - newWidth / 2;
                Bounds.Right = Bounds.Left + newWidth;
                Text = text.ToString();

                if (oldWidth != newWidth) TextChanged?.Invoke(Text);
            }
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.DrawText(Bounds.TopLeft, Text, PixelEngine.Pixel.Presets.White);
            if (selected) {
                CoreGame.Instance.DrawRect(Bounds.TopLeft - 1, Bounds.BottomRight + 1, CoreGame.Instance.Random(PixelEngine.Pixel.PresetPixels));
                CoreGame.Instance.DrawLine(Bounds.TopLeft + new Point(location * 8, 0),
                                           Bounds.BottomLeft + new Point(location * 8, -1),
                                           PixelEngine.Pixel.Presets.Yellow);
            }
        }
    }
}
