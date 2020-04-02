using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

namespace CoreModule.Terrain {
    public class Tile : Drawables.Drawable {
        public const int TileSize = 8;

        public byte Type {
            get => type; set {
                type = value;
                Sprite = TileManager.GetTexture(type);
            }
        }
        byte type;
        public Sprite Sprite { get; private set; }

        public Tile(byte type) {
            Sprite = TileManager.GetTexture(type);
            Type = type;
        }

        public override void Draw() {
            base.Draw();
        }
    }
}
