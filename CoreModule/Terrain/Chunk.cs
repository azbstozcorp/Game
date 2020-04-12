using System;
using System.Collections.Generic;

using PixelEngine;
using CoreModule.Saving;
using CoreModule.Shapes;
using CoreModule.Scenes;

using Point = CoreModule.Shapes.Point;

namespace CoreModule.Terrain {

    [Saveable]
    public class Chunk : Drawables.Drawable {
        public const int NumTiles = 40;
        public const int ChunkSize = Tile.TileSize * NumTiles;

        [At("root")] public Point WorldPosition { get; } = new Point();
        [At("root")] public Tile[,] Tiles { get; } = new Tile[NumTiles, NumTiles];
        public List<Rect> Colliders { get; } = new List<Rect>();
        public bool Empty { get; private set; } = true;

        public Chunk(int worldX, int worldY) {
            Bounds = new Rect(worldX * ChunkSize, worldY * ChunkSize, ChunkSize, ChunkSize);
            WorldPosition.X = worldX;
            WorldPosition.Y = worldY;

            for (int x = 0; x < NumTiles; x++) {
                for (int y = 0; y < NumTiles; y++) {
                    Tiles[x, y] = new Tile((1));
                }
            }
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
        }

        public override void Draw(bool drawDebug = false) {
            base.Draw();
            Pixel lines = new Pixel(255, 255, 255, 40);

            Point topLeft = new Point(Level.Instance.CameraLocation.X + Bounds.Left, Level.Instance.CameraLocation.Y + Bounds.Top);
            Point bottomRight = new Point(Level.Instance.CameraLocation.X + Bounds.Right, Level.Instance.CameraLocation.Y + Bounds.Bottom);

            if (Empty)
                return; // If there are no tiles in the chunk, don't draw

            if (!Collision.RectsOverlap(new Rect(topLeft, bottomRight),
                                        new Rect(new Point(0, 0),
                                            CoreGame.Instance.ScreenWidth,
                                            CoreGame.Instance.ScreenHeight))
                                        ) return;

            for (int x = 0; x < NumTiles; x++) for (int y = 0; y < NumTiles; y++) {
                    Point tilePos = new Point(x * Tile.TileSize + topLeft.X, y * Tile.TileSize + topLeft.Y);
                    if (tilePos.X + Tile.TileSize < 0) continue;
                    if (tilePos.Y + Tile.TileSize < 0) continue;
                    if (tilePos.X > CoreGame.Instance.ScreenWidth) continue;
                    if (tilePos.Y > CoreGame.Instance.ScreenHeight) continue;

                    Tile current = Tiles[x, y];
                    if (current == null) continue;
                    if (current.Sprite == null) continue;


                    var transform = new PixelEngine.Extensions.Transforms.Transform();
                    transform.Translate(tilePos.X, tilePos.Y);
                    PixelEngine.Extensions.Transforms.Transform.DrawSprite(current.Sprite, transform);
                }

            if (drawDebug) {
                CoreGame.Instance.DrawRect(topLeft, bottomRight, lines);
                foreach (Rect collider in Colliders) {
                    //CoreGame.Instance.DrawRect(collider.TopLeft + topLeft, collider.BottomRight + topLeft, lines);
                }
            }
        }

        public void SetTile(Tile t, int x, int y) {
            if (x < 0 || y < 0 || x >= NumTiles || y >= NumTiles) return;

            t.Bounds = new Rect(new Point(x * Tile.TileSize, y * Tile.TileSize), Tile.TileSize, Tile.TileSize);
            Tiles[x, y] = t;

            Empty = false;
            GenerateColliders();
        }

        public Tile GetTile(int x, int y) {
            if (x < 0 || y < 0 || x >= NumTiles || y >= NumTiles) return null;
            return Tiles[x, y];
        }

        public void GenerateColliders() {
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

                    if (a.Right == b.Left && a.Bottom == b.Bottom && a.Top != b.Top && b.Top < a.Top) {
                        a.Right = b.Right;
                        b.Bottom = a.Top;
                    }
                    if (a.Right == b.Left && a.Top == b.Top && a.Bottom != b.Bottom && b.Top > a.Top) {
                        a.Right = b.Right;
                        b.Top = a.Bottom;
                    }
                }
            }
        }
    }
}
