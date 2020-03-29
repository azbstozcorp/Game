using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Drawables {
    public class Button : Drawable {
        public string Text { get; set; }

        public Button(string text, int centerX, int centerY) {
            Text = text;

            // TODO: Once spritefont is implemented, find width and properly center.
            Bounds = new Rect(new Point(centerX - 10, centerY - 5), 20, 10);
        }

        public override void Draw(CoreGame instance) {
            base.Draw(instance);

            // TODO: Once spritefont is implemented, draw with spritefont.
            instance.DrawText(Bounds.TopLeft, Text, PixelEngine.Pixel.Presets.White);
            if (Collision.WithinRect(Bounds, new Point(instance.MouseX, instance.MouseY), true))
                instance.DrawRect(Bounds.TopLeft - 1, Bounds.BottomRight + 1, instance.Random(PixelEngine.Pixel.PresetPixels));
        }
    }
}
