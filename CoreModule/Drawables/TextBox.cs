using System.Text;

using CoreModule.Shapes;

namespace CoreModule.Drawables {

    public class TextBox : Button {
        public delegate void TextChangedEventHandler(string newText);
        public event TextChangedEventHandler TextChanged;

        public bool Selected { get; set; } = false;
        protected int location = 0;

        public TextBox(string text, int centerX, int centerY, bool startAtEnd = true) : base(text, centerX, centerY) {
            Pressed += (Button pressed) => Selected = true;
            if (startAtEnd) location = text.Length;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            if (Selected &&
               !Collision.WithinRect(Bounds, new Point(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY), true) &&
               CoreGame.Instance.GetMouse(PixelEngine.Mouse.Left).Down) Selected = false;

            StringBuilder text = new StringBuilder(Text);
            if (Selected) {

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


            }
            int oldWidth = (Text.Length == 0 ? 1 : Text.Length) * 8;
            int newWidth = (text.Length == 0 ? 1 : text.Length) * 8;
            Bounds.Left = (Bounds.Left + oldWidth / 2) - newWidth / 2;
            Bounds.Right = Bounds.Left + newWidth;
            /*if (oldWidth != newWidth)*/
            if (text.ToString() != Text) TextChanged?.Invoke(text.ToString());
            Text = text.ToString();
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.DrawText(Bounds.TopLeft, Text, PixelEngine.Pixel.Presets.White);
            if (Selected) {
                CoreGame.Instance.DrawRect(Bounds.TopLeft - 1, Bounds.BottomRight + 1, CoreGame.Instance.Random(PixelEngine.Pixel.PresetPixels));
                CoreGame.Instance.DrawLine(Bounds.TopLeft + new Point(location * 8, 0),
                                           Bounds.BottomLeft + new Point(location * 8, -1),
                                           PixelEngine.Pixel.Presets.Yellow);
            }
        }
    }
}
