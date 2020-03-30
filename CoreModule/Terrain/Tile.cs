using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using static CoreModule.Terrain.TerrainType;

namespace CoreModule.Terrain {
    public static class TileManager {
        public static Dictionary<TerrainType, Sprite> Graphics { get; } = new Dictionary<TerrainType, Sprite>();

        public static void Setup() {
            string[] manifest = System.IO.File.ReadAllLines("Assets/Terrain/manifest.txt");

            foreach (string s in manifest) {
                string[] data = s.Split(' ');
                int key = int.Parse(data[0]);
                string name = $"Assets/Terrain/{data[1]}.png";

                Graphics[(TerrainType)key] = Sprite.Load(name);
            }
        }
    }

    public class Tile : Drawables.Drawable {
        public const int TileSize = 10;

        public TerrainType Type { get; private set; }
        public Sprite sprite;

        public Tile(TerrainType type) {
            sprite = TileManager.Graphics[type];
        }

        public override void Draw() {
            base.Draw();
        }
    }

    public class Chunk : Drawables.Drawable {
        public const int NumTiles = 10;
        public const int ChunkSize = Tile.TileSize * NumTiles;

        public Tile[,] Tiles { get; } = new Tile[NumTiles, NumTiles];
        public bool Empty { get; private set; } = true;

        public Rect ChunkBounds { get; private set; }

        public Chunk() {
            ChunkBounds = new Rect();
        }
        public Chunk(Point location) {
            ChunkBounds = new Rect(location, ChunkSize, ChunkSize);
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
        }

        public override void Draw() {
            base.Draw();
            Point topLeft = new Point(World.Instance.CameraLocation.X + ChunkBounds.Left, World.Instance.CameraLocation.Y + ChunkBounds.Top);
            Point bottomRight = new Point(World.Instance.CameraLocation.X + ChunkBounds.Right, World.Instance.CameraLocation.Y + ChunkBounds.Bottom);
            CoreGame.Instance.DrawRect(topLeft, bottomRight, CoreGame.Instance.Random(Pixel.PresetPixels));

            if (Empty)
                return; // If there are no tiles in the chunk, don't draw

            if (!Collision.RectsOverlap(new Rect(topLeft, bottomRight), new Rect(new Point(0, 0), CoreGame.Instance.ScreenWidth, CoreGame.Instance.ScreenHeight))) return;
           // if (!Collision.RectsOverlap(new Rect(World.Instance.CameraLocation, CoreGame.Instance.ScreenWidth, CoreGame.Instance.ScreenHeight), new Rect(topLeft, bottomRight)))
           //     return; // If the chunk is not onscreen, don't draw

            for (int x = 0; x < NumTiles; x++) for (int y = 0; y < NumTiles; y++) {
                    Tile current = Tiles[x, y];
                    if (current == null) continue;
                    if (current.Type == TT_AIR) continue;

                    CoreGame.Instance.DrawSprite(new Point(x * Tile.TileSize + topLeft.X,
                                                           y * Tile.TileSize + topLeft.Y),
                                                 current.sprite);
                }
        }

        public void AddTile(Tile t, int x, int y) {
            if (!(Collision.Between(-1, NumTiles, x) || Collision.Between(-1, NumTiles, y))) return;
            Tiles[x, y] = t;
            Empty = false;
        }
    }
}
