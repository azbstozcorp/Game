using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Drawables {
    public class CheckBox : Button {
        public bool On { get; set; } = false;
        public CheckBox(int centerX, int centerY) : base(" ", centerX, centerY) {
            Pressed += Clicked;
        }

        private void Clicked(Button sender) {
            On ^= true;
        }

        public override void Draw(bool drawDebug = false) {
            base.Draw();
            CoreGame.Instance.DrawRect(Bounds.TopLeft, Bounds.BottomRight, PixelEngine.Pixel.Presets.DarkGrey);
            if (On) CoreGame.Instance.FillRect(Bounds.TopLeft + 1, Bounds.BottomRight, PixelEngine.Pixel.Presets.White);
        }
    }
}
