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
        public static List<TerrainType> Collideable { get; } = new List<TerrainType>();

        public static void Setup() {
            string[] manifest = System.IO.File.ReadAllLines("Assets/Terrain/manifest.txt");

            Graphics[TT_AIR] = null;

            foreach (string s in manifest) {
                string[] data = s.Split(' ');
                TerrainType key = (TerrainType)int.Parse(data[0]);
                string name = $"Assets/Terrain/{data[1]}.png";
                bool collide = bool.Parse(data[2]);

                Graphics[key] = Sprite.Load(name);
                if (collide) Collideable.Add(key);
            }
        }

        public static bool IsSolid(Tile t) {
            return t != null && Collideable.Contains(t.Type);
        }
    }

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

    public class Chunk : Drawables.Drawable {
        public const int NumTiles = 20;
        public const int ChunkSize = Tile.TileSize * NumTiles;

        public Tile[,] Tiles { get; } = new Tile[NumTiles, NumTiles];
        public List<Rect> Colliders { get; } = new List<Rect>();
        public bool Empty { get; private set; } = true;
        public Rect ChunkBounds { get; private set; }

        int tileCount = 0;

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

            if (!Collision.RectsOverlap(new Rect(topLeft, bottomRight),
                                        new Rect(new Point(0, 0),
                                            CoreGame.Instance.ScreenWidth,
                                            CoreGame.Instance.ScreenHeight))
                                        ) return;

            for (int x = 0; x < NumTiles; x++) for (int y = 0; y < NumTiles; y++) {
                    Tile current = Tiles[x, y];
                    if (current == null) continue;
                    if (current.Type == TT_AIR) continue;

                    CoreGame.Instance.DrawSprite(new Point(x * Tile.TileSize + topLeft.X,
                                                           y * Tile.TileSize + topLeft.Y),
                                                 current.sprite);
                }

            foreach (Rect collider in Colliders) {
                CoreGame.Instance.DrawRect(collider.TopLeft + topLeft, collider.BottomRight + topLeft, Pixel.Presets.White);
            }
        }

        public void SetTile(Tile t, int x, int y) {
            if (!(Collision.Between(-1, NumTiles, x) || Collision.Between(-1, NumTiles, y)) || t == null) return;

            Tile alreadyThere = Tiles[x, y];
            t.Bounds = new Rect(new Point(x * Tile.TileSize, y * Tile.TileSize), Tile.TileSize, Tile.TileSize);
            Tiles[x, y] = t;

            Empty = false;
            GenerateColliders();
        }

        public Tile GetTile(int x, int y) {
            if (x < 0 || y < 0 || x >= NumTiles || y >= NumTiles) return null;
            return Tiles[x, y];
        }

        void GenerateColliders() {
            Colliders.Clear();

            for (int x = 0; x < NumTiles; x++) {
                List<int> boundaries = new List<int>();

                for (int y = 0; y < NumTiles; y++) {
                    if (TileManager.IsSolid(GetTile(x, y))) {
                        if ((!TileManager.IsSolid(GetTile(x, y - 1)) || !TileManager.IsSolid(GetTile(x, y + 1)) || y == NumTiles - 1))
                            boundaries.Add(y);
                        if ((!TileManager.IsSolid(GetTile(x, y - 1)) && !TileManager.IsSolid(GetTile(x, y + 1))))
                            boundaries.Add(y);
                    }
                }

                if (boundaries.Count >= 2)
                    for (int i = 0; i < boundaries.Count; i += 2) {
                        Colliders.Add(new Rect(
                            x * Tile.TileSize,
                            boundaries[i] * Tile.TileSize,
                            (x + 1) * Tile.TileSize,
                            boundaries[i + 1] * Tile.TileSize + Tile.TileSize
                            ));
                    }
            }

            TrimColliders();
        }

        void TrimColliders() {
            for (int i = Colliders.Count - 1; i >= 0; i--) {
                for (int j = Colliders.Count - 1; j >= 0; j--) {
                    if (i == j) continue;
                    Rect a = Colliders[i];
                    Rect b = Colliders[j];

                    if (a.Right == b.Left && a.Top == b.Top && a.Bottom == b.Bottom) {
                        a.Right = b.Right;
                        Colliders.Remove(b);
                    }
                    if (a.Bottom == b.Top && a.Right == b.Right && a.Left == b.Left) {
                        a.Bottom = b.Bottom;
                        Colliders.Remove(b);
                    }
                }
            }
        }
    }
}
