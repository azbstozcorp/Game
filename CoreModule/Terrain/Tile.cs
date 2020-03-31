using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

namespace CoreModule.Terrain {
    public class Tile : Drawables.Drawable {
        public const int TileSize = 4;

        public TerrainType Type {
            get => type; set {
                type = value;
                Sprite = TileManager.Graphics[type];
            }
        }
        TerrainType type;
        public Sprite Sprite { get; private set; }

        public Tile(TerrainType type) {
            Sprite = TileManager.Graphics[type];
            Type = type;
        }

        public override void Draw() {
            base.Draw();
        }
    }
}
