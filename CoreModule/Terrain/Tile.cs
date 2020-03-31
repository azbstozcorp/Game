using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;

namespace CoreModule.Terrain {

    public class Tile : Drawables.Drawable {
        public const int TileSize = 4;

        public TerrainType Type { get; private set; }
        public Sprite sprite;

        public Tile(TerrainType type) {
            sprite = TileManager.Graphics[type];
            Type = type;
        }

        public override void Draw() {
            base.Draw();
        }
    }
}
