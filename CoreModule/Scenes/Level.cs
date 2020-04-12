using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using PixelEngine;
using CoreModule.Shapes;
using CoreModule.Terrain;
using CoreModule.Entities;
using CoreModule.Drawables;
using CoreModule.Entities.Particles;

using Point = CoreModule.Shapes.Point;

namespace CoreModule.Scenes {
    public class Level : Scene {
        public static Level Instance { get; private set; }

        public string Name { get; private set; }
        public ChunkSet Chunks { get; private set; }
        public Point CameraLocation;

        public Chunk GetChunkWithPoint(Point point) => Chunks.GetChunk(point.X / Chunk.ChunkSize, point.Y / Chunk.ChunkSize);
        public static Point ScreenToWorld(Point p) => p - Instance.CameraLocation;
        public static Point WorldToScreen(Point p) => p + Instance.CameraLocation;

        public Level(string name) {
            TileManager.Setup();

            Instance = this;
            Name = name; 
            Chunks = new ChunkSet();
        }

        public void SetTile(Tile t, int worldX, int worldY, int tileX, int tileY) {
            if (Chunks.GetChunk(worldX, worldY) == null) Chunks.SetChunk(new Chunk(worldX, worldY), worldX, worldY);
            Chunks.GetChunk(worldX, worldY).SetTile(t, tileX, tileY);
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (CoreGame.Instance.GetMouse(Mouse.Any).Down) {
                Point chunkMouse = new Point(CoreGame.Instance.MouseX / Chunk.ChunkSize, CoreGame.Instance.MouseY / Chunk.ChunkSize);

                if (Chunks.GetChunk(chunkMouse.X, chunkMouse.Y) == null) Chunks.SetChunk(new Chunk(chunkMouse.X, chunkMouse.Y), chunkMouse.X, chunkMouse.Y);
                Chunks.GetChunk(chunkMouse.X, chunkMouse.Y).SetTile(new Tile(2), 0, 0);
                Save();
            }
        }
        public override void Draw() {
            base.Draw();
            Chunks.Draw();
        }

        #region    Saving / Loading
        public void Save() {
            string pathToLevel = $"Assets/Levels/{Name}";
            string pathToChunks = $"{pathToLevel}/chunks";

            Chunks.SaveTo(pathToChunks);
        }
        public void Load() {
            string pathToLevel = $"Assets/Levels/{Name}";
            string pathToChunks = $"{pathToLevel}/chunks";

            Chunks.LoadFrom(pathToChunks);
        }
        #endregion Saving / Loading
    }
}
