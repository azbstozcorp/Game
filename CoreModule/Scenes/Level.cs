using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;
using CoreModule.Shapes;
using CoreModule.Saving;
using CoreModule.Terrain;
using CoreModule.Drawables.Entities;

using Point = CoreModule.Shapes.Point;

namespace CoreModule.Scenes {
    public class Level : Scene, ISerializable<Level> {
        public static Level Instance { get; private set; }

        public List<PhysicsEntity> Entities { get; } = new List<PhysicsEntity>();
        public string Name { get; private set; } = "";
        public Point CameraLocation;
        public Chunk[,] chunks;
        int tileIndex = 2;

        bool editing = false;
        List<Button> editorButtons = new List<Button>();

        public Level() {
            Console.WriteLine($"Initializing World...");
            Instance = this;
            CameraLocation = new Point();
            TileManager.Setup();
            Entities.Add(new PhysicsEntity(10, 10, 10, 20, null));

            Button swapLevel = new Button("Load", CoreGame.Instance.ScreenWidth - 20, 40);
            swapLevel.ButtonPressed += SwapLevel_ButtonPressed;
            editorButtons.Add(swapLevel);
        }

        private void SwapLevel_ButtonPressed() {
            CoreGame.Instance.PopScene();
            CoreGame.Instance.PushScene(LoadLevel("Newtest"));
        }

        public Level(string name) : this() {
            Console.WriteLine($"Constructing new world with name {name}...");
            Name = name;

            chunks = new Chunk[5, 3];

            for (int i = 0; i < chunks.GetLength(0); i++) for (int j = 0; j < chunks.GetLength(1); j++) {
                    chunks[i, j] = new Chunk((i * Chunk.ChunkSize, j * Chunk.ChunkSize));
                    chunks[i, j].WorldPosition.X = i;
                    chunks[i, j].WorldPosition.Y = j;
                }
            //chunks[0, 0].SetTile(new Tile(TerrainType.TT_DIRT), 2, 4);
        }

        public static Level LoadLevel(string levelName) {
            string path = $"Assets/Levels/{levelName}.bin";
            if (System.IO.File.Exists(path)) return LevelIO.LoadLevel(path);
            else return new Level(levelName);
        }

        public void SaveLevel() {
            string path = $"Assets/Levels/{Name}.bin";
            LevelIO.SaveLevel(this, path);
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

        void Exit() {
            SaveLevel();
            CoreGame.Instance.PopScene();
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (CoreGame.Instance.GetKey(Key.Escape).Down) Exit();

            int chunkMouseX = (CoreGame.Instance.MouseX - CameraLocation.X) / Chunk.ChunkSize;
            int chunkMouseY = (CoreGame.Instance.MouseY - CameraLocation.Y) / Chunk.ChunkSize;
            int tileMouseX = ((CoreGame.Instance.MouseX - CameraLocation.X) % Chunk.ChunkSize) / Tile.TileSize;
            int tileMouseY = ((CoreGame.Instance.MouseY - CameraLocation.Y) % Chunk.ChunkSize) / Tile.TileSize;

            if (editing) {
                tileIndex += (int)CoreGame.Instance.MouseScroll;
                if (tileIndex == 0) tileIndex = (int)TerrainType.TT_COUNT - 1;
                if (tileIndex == (int)TerrainType.TT_COUNT) tileIndex = 1;
                if (CoreGame.Instance.GetMouse(Mouse.Left).Down) {
                    GetChunk(chunkMouseX, chunkMouseY)?.SetTile(new Tile((TerrainType)tileIndex), tileMouseX, tileMouseY);
                }
                foreach (Button b in editorButtons) b.Update(fElapsedTime);

                if (CoreGame.Instance.GetKey(Key.W).Down) CameraLocation.Y++;
                if (CoreGame.Instance.GetKey(Key.S).Down) CameraLocation.Y--;
                if (CoreGame.Instance.GetKey(Key.A).Down) CameraLocation.X++;
                if (CoreGame.Instance.GetKey(Key.D).Down) CameraLocation.X--;
            }

            if (CoreGame.Instance.GetKey(Key.Left).Down) Entities[0].Velocity.X = -1;
            if (CoreGame.Instance.GetKey(Key.Right).Down) Entities[0].Velocity.X = +1;
            if (CoreGame.Instance.GetKey(Key.Up).Pressed) Entities[0].Velocity.Y = -1.5f;

            if (CoreGame.Instance.GetKey(Key.E).Pressed) editing = !editing;

            foreach (PhysicsEntity e in Entities) e.Update(fElapsedTime);
        }

        public override void Draw() {
            CoreGame.Instance.Clear(Pixel.Presets.DarkBlue);

            for (int i = 0; i < chunks.GetLength(0); i++) for (int j = 0; j < chunks.GetLength(1); j++) {
                    GetChunk(i, j)?.Draw();
                }
            foreach (PhysicsEntity e in Entities) e.Draw();

            if (editing) {
                CoreGame.Instance.DrawRect(new Point(0, 0) + CameraLocation, new Point(Chunk.ChunkSize * chunks.GetLength(0), Chunk.ChunkSize * chunks.GetLength(1)) + CameraLocation, Pixel.Presets.Black);
                CoreGame.Instance.DrawText(new Point(0, 0), $"{tileIndex}", Pixel.Presets.Red);
                foreach (Button b in editorButtons) b.Draw();
            }
        }

        public byte[] GetSaveData() {
            byte[] name = Encoding.ASCII.GetBytes(Name);
            byte nameLength = (byte)name.Length;

            byte chunkWidthX = BitConverter.GetBytes(chunks.GetLength(0)).First();
            byte chunkWidthY = BitConverter.GetBytes(chunks.GetLength(1)).First();

            List<byte[]> chunkData = new List<byte[]>(); // Will be /* (0,0)(0,1)(0,2)(0,3) ... */
            for (int x = 0; x < chunks.GetLength(0); x++) {         /* (1,0)(1,1)(1,2)(1,3) ... */
                for (int y = 0; y < chunks.GetLength(1); y++) {
                    chunkData.Add(chunks[x, y].GetSaveData());
                }
            }

            List<byte> data = new List<byte>();
            data.Add(nameLength);
            data.AddRange(name);
            data.Add(chunkWidthX);
            data.Add(chunkWidthY);
            foreach (byte[] chunkBytes in chunkData)
                data.AddRange(chunkBytes);
            return data.ToArray();
        }

        public void LoadSaveData(byte[] data) {
            int location = 0;

            byte nameLength = data[location]; /*                             */ location++;
            string name = Encoding.ASCII.GetString(data, location, nameLength); location += nameLength;

            byte chunkWidthX = data[location]; /*                            */ location++;
            byte chunkWidthY = data[location]; /*                            */ location++;
            chunks = new Chunk[chunkWidthX, chunkWidthY];

            int chunkByteSize = Chunk.NumTiles * Chunk.NumTiles;
            for (int x = 0; x < chunkWidthX; x++) {
                for (int y = 0; y < chunkWidthY; y++) {
                    chunks[x, y] = new Chunk((x * Chunk.ChunkSize, y * Chunk.ChunkSize));
                    chunks[x, y].WorldPosition.X = x;
                    chunks[x, y].WorldPosition.Y = y;
                    chunks[x, y].LoadSaveData(data.Skip(location).Take(chunkByteSize).ToArray());
                    location += chunkByteSize;
                }
            }

            Name = name;
        }
    }
}
