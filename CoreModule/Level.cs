using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;
using CoreModule.Shapes;
using CoreModule.Terrain;
using CoreModule.Drawables.Entities;

using PPoint = PixelEngine.Point;
using Point = CoreModule.Shapes.Point;

namespace CoreModule {
    public class Level : Scene {
        public static Level Instance { get; private set; }

        public List<PhysicsEntity> Entities { get; } = new List<PhysicsEntity>();
        public Point CameraLocation;
        Chunk[,] chunks;
        int tileIndex = 2;

        public Level() {
            Console.WriteLine("Initializing World...");
            Instance = this;
            CameraLocation = new Point();

            TileManager.Setup();
            chunks = new Chunk[10, 10];

            for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) {
                    chunks[i, j] = new Chunk((i * Chunk.ChunkSize, j * Chunk.ChunkSize));
                    chunks[i, j].WorldPosition.X = i;
                    chunks[i, j].WorldPosition.Y = j;
                }

            Entities.Add(new PhysicsEntity(10, 10, 10, 20, null));
            //chunks[0, 0].SetTile(new Tile(TerrainType.TT_DIRT), 2, 4);
        }

        /// <summary>
        /// Returns the chunk at a given index into the world, or null if the index is out of range.
        /// </summary>
        public Chunk GetChunk(int worldX, int worldY) {
            if (worldX < 0 || worldY < 0 || worldX >= chunks.GetLength(0) || worldY >= chunks.GetLength(1)) return null;
            return chunks[worldX, worldY];
        }

        /// <summary>
        /// Returns the chunk which contains a given position.
        /// </summary>
        public Chunk GetChunkWithPoint(Point position) {
            int cX = position.X / Chunk.ChunkSize;
            int cY = position.Y / Chunk.ChunkSize;
            return GetChunk(cX, cY);
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (CoreGame.Instance.GetKey(Key.Escape).Down) CoreGame.Instance.PopScene();

            //Entities[0].X = CoreGame.Instance.MouseX;
            //Entities[0].Y = CoreGame.Instance.MouseY;

            int chunkMouseX = (CoreGame.Instance.MouseX - CameraLocation.X) / Chunk.ChunkSize;
            int chunkMouseY = (CoreGame.Instance.MouseY - CameraLocation.Y) / Chunk.ChunkSize;
            int tileMouseX = ((CoreGame.Instance.MouseX - CameraLocation.X) % Chunk.ChunkSize) / Tile.TileSize;
            int tileMouseY = ((CoreGame.Instance.MouseY - CameraLocation.Y) % Chunk.ChunkSize) / Tile.TileSize;

            tileIndex += (int)CoreGame.Instance.MouseScroll;
            if (tileIndex == 0) tileIndex = (int)TerrainType.TT_COUNT - 1;
            if (tileIndex == (int)TerrainType.TT_COUNT) tileIndex = 1;
            if (CoreGame.Instance.GetMouse(Mouse.Left).Down) {
                chunks[chunkMouseX, chunkMouseY].SetTile(new Tile((TerrainType)tileIndex), tileMouseX, tileMouseY);
            }

            //if (CoreGame.Instance.GetKey(Key.Left).Down)    CameraLocation.X++;
            //if (CoreGame.Instance.GetKey(Key.Right).Down)   CameraLocation.X--;
            //if (CoreGame.Instance.GetKey(Key.Up).Down)      CameraLocation.Y++;
            //if (CoreGame.Instance.GetKey(Key.Down).Down)    CameraLocation.Y--;

            if (CoreGame.Instance.GetKey(Key.Left).Down) Entities[0].X--;
            if (CoreGame.Instance.GetKey(Key.Right).Down) Entities[0].X++;
            if (CoreGame.Instance.GetKey(Key.Up).Down) {
                Entities[0].Velocity.Y = -1;
                //Entities[0].X = 10; Entities[0].Y = 10;
                //Entities[0].ResetBounds();
            }
            if (CoreGame.Instance.GetKey(Key.Down).Down) ;

            foreach (PhysicsEntity e in Entities) e.Update(fElapsedTime);
        }

        public override void Draw() {
            CoreGame.Instance.Clear(Pixel.Presets.DarkBlue);


            for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) {
                    chunks[i, j].Draw();
                }
            foreach (PhysicsEntity e in Entities) e.Draw();

            CoreGame.Instance.DrawText(new Point(0, 0), $"{tileIndex}", Pixel.Presets.Red);
        }
    }
}
