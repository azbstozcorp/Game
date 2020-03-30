using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;
using CoreModule.Terrain;

namespace CoreModule {
    public class World : Scene {
        public static World Instance { get; private set; }

        public Point CameraLocation;
        Chunk[,] chunks;

        public World() {
            Console.WriteLine("Initializing World...");
            Instance = this;
            CameraLocation = new Point();

            TileManager.Setup();
            chunks = new Chunk[10, 10];

            for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) {
                    chunks[i, j] = new Chunk((i * Chunk.ChunkSize, j * Chunk.ChunkSize));
                }

            chunks[0, 0].AddTile(new Tile(TerrainType.TT_DIRT), 2, 4);
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (CoreGame.Instance.GetKey(Key.Escape).Down) CoreGame.Instance.PopScene();

            int chunkMouseX = (CoreGame.Instance.MouseX - CameraLocation.X) / Chunk.ChunkSize;
            int chunkMouseY = (CoreGame.Instance.MouseY - CameraLocation.Y) / Chunk.ChunkSize;
            int tileMouseX = ((CoreGame.Instance.MouseX - CameraLocation.X) % Chunk.ChunkSize) / Tile.TileSize;
            int tileMouseY = ((CoreGame.Instance.MouseY - CameraLocation.Y) % Chunk.ChunkSize) / Tile.TileSize;

            if (CoreGame.Instance.GetMouse(Mouse.Left).Down) {
                chunks[chunkMouseX, chunkMouseY].AddTile(new Tile(TerrainType.TT_DIRT), tileMouseX, tileMouseY);
            }

            if (CoreGame.Instance.GetKey(Key.Left).Down) CameraLocation.X++;
            if (CoreGame.Instance.GetKey(Key.Right).Down) CameraLocation.X--;
            if (CoreGame.Instance.GetKey(Key.Up).Down) CameraLocation.Y++;
            if (CoreGame.Instance.GetKey(Key.Down).Down) CameraLocation.Y--;
        }

        public override void Draw() {
            CoreGame.Instance.Clear(Pixel.Presets.DarkBlue);

            for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) {
                    chunks[i, j].Draw();
                }
        }
    }
}
