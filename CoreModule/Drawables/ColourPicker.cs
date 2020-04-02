using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Shapes;
using Point = CoreModule.Shapes.Point;

namespace CoreModule.Drawables {
    public class ColourPicker : Drawable {
        public List<Drawable> Drawables { get; } = new List<Drawable>();

        public Pixel Value {
            get => value; set {
                values[0].Value = value.R;
                values[1].Value = value.G;
                values[2].Value = value.B;
                values[3].Value = value.A;
            }
        }
        Pixel value;

        int selected = 0;
        Button[] minus;
        NumberBox[] values;
        Button[] plus;

        public ColourPicker(Point topLeft) {
            minus = new[] {
            new Button("-", topLeft.X + 5, topLeft.Y + 00),
            new Button("-", topLeft.X + 5, topLeft.Y + 10),
            new Button("-", topLeft.X + 5, topLeft.Y + 20),
            new Button("-", topLeft.X + 5, topLeft.Y + 30), };

            values = new[] {
            new NumberBox(255, topLeft.X + 28, topLeft.Y + 00, 0, 255),
            new NumberBox(255, topLeft.X + 28, topLeft.Y + 10, 0, 255),
            new NumberBox(255, topLeft.X + 28, topLeft.Y + 20, 0, 255),
            new NumberBox(255, topLeft.X + 28, topLeft.Y + 30, 0, 255), };

            plus = new[] {
            new Button("+", topLeft.X + 60, topLeft.Y + 00),
            new Button("+", topLeft.X + 60, topLeft.Y + 10),
            new Button("+", topLeft.X + 60, topLeft.Y + 20),
            new Button("+", topLeft.X + 60, topLeft.Y + 30), };

            foreach (Button b in minus) b.Pressed += Decrease;
            foreach (NumberBox b in values) b.Pressed += Deselect;
            foreach (Button b in plus) b.Pressed += Increase;

            Drawables.AddRange(minus);
            Drawables.AddRange(values);
            Drawables.AddRange(plus);

            Bounds = new Rect(topLeft - (10, 10), plus[3].Bounds.BottomRight + (5, 5));
        }

        private void Decrease(Button sender) {
            values[minus.ToList().IndexOf(sender)].Value--;
        }
        private void Deselect(Button sender) {
            if(sender is NumberBox n) {
                selected = values.ToList().IndexOf(n);
            }
            //if (sender is TextBox s)
            //    s.Selected = false;
        }
        private void Increase(Button sender) {
            values[plus.ToList().IndexOf(sender)].Value++;
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);

            if (CoreGame.Instance.GetKey(Key.Up).Pressed) selected--;
            if (CoreGame.Instance.GetKey(Key.Down).Pressed) selected++;
            if (selected < 0) selected = 3;
            if (selected > 3) selected = 0;

            if (CoreGame.Instance.GetKey(Key.Left).Down) minus[selected].Press();
            if (CoreGame.Instance.GetKey(Key.Right).Down) plus[selected].Press();

            foreach (Drawable d in Drawables) d.Update(fElapsedTime);
        }

        public override void Draw() {
            base.Draw();

            CoreGame.Instance.FillRect(Bounds.TopLeft, Bounds.BottomRight, Pixel.Presets.DarkGrey);

            Pixel colour = new Pixel(
            (byte)values[0].Value,
            (byte)values[1].Value,
            (byte)values[2].Value,
            (byte)values[3].Value);
            value = colour;

            Point colourRectTopLeft = values[0].Bounds.TopRight + (5, 0);
            Point colourRectBottomRight = values[3].Bounds.BottomRight + (15, 0);

            CoreGame.Instance.DrawText(values[selected].Bounds.TopRight, "<", colour);
            CoreGame.Instance.FillRect(colourRectTopLeft, colourRectBottomRight, colour);
            foreach (Drawable d in Drawables) d.Draw();
        }
    }
}
