using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

namespace CoreModule.Drawables {
    public class ImageButton : Button {
        public Sprite Image { get; set; }

        public ImageButton(string text, int centerX, int centerY, Sprite image) : base(text, centerX, centerY) {
            Image = image;
        }

        public override void Draw(bool drawDebug = false) {
            base.Draw();

            CoreGame.Instance.PixelMode = Pixel.Mode.Alpha;
            CoreGame.Instance.DrawSprite(Bounds.TopRight, Image);
            CoreGame.Instance.PixelMode = Pixel.Mode.Normal;
        }
    }
}
