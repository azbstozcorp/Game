using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

using CoreModule.Drawables;
using CoreModule.Terrain;

namespace CoreModule.Scenes {
    public class TileEditor : Scene {
        string[] ids = new string[256];
        Dictionary<byte, TextBox> manifest = new Dictionary<byte, TextBox>();

        public TileEditor() {
            TileManager.Setup();

            Drawables.Add(new ColourPicker(new Shapes.Point(0, 100)));
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);


        }

        public override void Draw() {
            base.Draw();
        }
    }
}
