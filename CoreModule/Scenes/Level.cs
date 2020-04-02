using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using PixelEngine;
using CoreModule.Drawables;
using CoreModule.Entities;
using CoreModule.Shapes;
using CoreModule.Saving;
using CoreModule.Terrain;

using Point = CoreModule.Shapes.Point;

namespace CoreModule.Scenes {
    public class Level : Scene, ISerializable<Level> {
        public static Level Instance { get; private set; }

        public List<Entity> Entities { get; } = new List<Entity>();
        public PhysicsEntity TestPlayer { get; } = new PhysicsEntity(10, 100, 10, 20, null);
        public string Name { get; private set; } = "";
        public Point CameraLocation;
        public Chunk[,] chunks;
        int tileIndex = 2;

        #region Editor Variables
        public bool Editing { get; private set; } = false;
        List<Button> editorButtons = new List<Button>();
        #endregion Editor Variables

        #region Constructors
        public Level() {
            Console.WriteLine($"Initializing World...");
            Instance = this;
            CameraLocation = new Point();
            TileManager.Setup();
            Entities.Add(TestPlayer);
            TestPlayer.DrawDebug = true;

            Button editorButtonSave = new Button("Save",
                CoreGame.Instance.ScreenWidth - "Save".Length * 4 - 1,
                (4 * (editorButtons.Count() + 1)) + editorButtons.Count + 1);
            editorButtonSave.Pressed += EditorButtonSave_Pressed;
            editorButtons.Add(editorButtonSave);

            Button editorButtonTileDialog = new Button("Tiles",
                CoreGame.Instance.ScreenWidth - "Tiles".Length * 4 - 1,
                (4 * editorButtons.Count() + 1) + editorButtons.Count + 1);
            editorButtonTileDialog.Pressed += EditorButtonTileDialog_Pressed;
        }

        public Level(string name) : this() {
            Console.WriteLine($"Constructing new world with name {name}...");
            Name = name;

            chunks = new Chunk[10, 10];

            for (int i = 0; i < 10; i++) for (int j = 0; j < 10; j++) {
                    chunks[i, j] = new Chunk((i * Chunk.ChunkSize, j * Chunk.ChunkSize));
                    chunks[i, j].WorldPosition.X = i;
                    chunks[i, j].WorldPosition.Y = j;
                }
            //chunks[0, 0].SetTile(new Tile(byte.TT_DIRT), 2, 4);
        }
        #endregion Constructors

        #region Chunk Utilities
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
        /// <summary>
        /// Sets the tile in a chunk
        /// </summary>
        public void SetTile(Tile t, int worldX, int worldY, int tileX, int tileY) => GetChunk(worldX, worldY).SetTile(t, tileX, tileY);
        #endregion Chunk Utilities

        public void RefreshTextures() {
            for (int i = 0; i < chunks.GetLength(0); i++)
                for (int j = 0; j < chunks.GetLength(1); j++) {
                    GetChunk(i, j).GenerateColliders();
                    for (int x = 0; x < Chunk.NumTiles; x++)
                        for (int y = 0; y < Chunk.NumTiles; y++)
                            GetChunk(i, j).GetTile(x, y).Type = GetChunk(i, j).GetTile(x, y).Type;
                }
        }

        void Exit() {
            CoreGame.Instance.PushScene(new ExitConfirmDialogue());
        }

        public override void Update(float fElapsedTime) {
            base.Update(fElapsedTime);
            if (CoreGame.Instance.GetKey(Key.Escape).Pressed) { Exit(); return; }
            if (CoreGame.Instance.GetKey(Key.T).Pressed) CoreGame.Instance.PushScene(new TileEditor());

            if (Editing) {
                int chunkMouseX = (CoreGame.Instance.MouseX - CameraLocation.X) / Chunk.ChunkSize;
                int chunkMouseY = (CoreGame.Instance.MouseY - CameraLocation.Y) / Chunk.ChunkSize;
                int tileMouseX = ((CoreGame.Instance.MouseX - CameraLocation.X) % Chunk.ChunkSize) / Tile.TileSize;
                int tileMouseY = ((CoreGame.Instance.MouseY - CameraLocation.Y) % Chunk.ChunkSize) / Tile.TileSize;

                if (CoreGame.Instance.GetKey(Key.W).Down) CameraLocation.Y++;
                if (CoreGame.Instance.GetKey(Key.S).Down) CameraLocation.Y--;
                if (CoreGame.Instance.GetKey(Key.A).Down) CameraLocation.X++;
                if (CoreGame.Instance.GetKey(Key.D).Down) CameraLocation.X--;

                tileIndex += (int)CoreGame.Instance.MouseScroll;
                if (tileIndex == 0) tileIndex = TileManager.MaxValue;
                if (tileIndex > TileManager.MaxValue) tileIndex = 1;

                if (CoreGame.Instance.GetMouse(Mouse.Left).Down && (byte)tileIndex != GetChunk(chunkMouseX, chunkMouseY).GetTile(tileMouseX, tileMouseY)?.Type) {
                    GetChunk(chunkMouseX, chunkMouseY).SetTile(new Tile((byte)tileIndex), tileMouseX, tileMouseY);
                }

                foreach (Button b in editorButtons) b.Update(fElapsedTime);
            }
            else {
                int vel = 0;
                if (CoreGame.Instance.GetKey(Key.A).Down) vel--;
                if (CoreGame.Instance.GetKey(Key.D).Down) vel++;
                TestPlayer.Velocity.X = vel;
                if (CoreGame.Instance.GetKey(Key.W).Pressed) TestPlayer.Velocity.Y = -2.5f;

                int cameraRatio = 1;
                Console.WriteLine(TestPlayer.X);
                float newX = -TestPlayer.X +
                            CoreGame.Instance.ScreenWidth / 2 +
                            TestPlayer.Bounds.Width / 2 -
                            CoreGame.Instance.MouseX / cameraRatio +
                            CoreGame.Instance.ScreenWidth / (cameraRatio * 2);
                float newY = -TestPlayer.Y +
                            CoreGame.Instance.ScreenHeight / 2 +
                            TestPlayer.Bounds.Height / 2 -
                            CoreGame.Instance.MouseY / cameraRatio +
                            CoreGame.Instance.ScreenHeight / (cameraRatio * 2);

                CameraLocation.X = (int)newX;
                CameraLocation.Y = (int)newY;

                foreach (PhysicsEntity e in Entities) e.Update(fElapsedTime);
            }

            if (CoreGame.Instance.GetKey(Key.E).Pressed) Editing = !Editing;
        }
        public override void Draw() {
            CoreGame.Instance.Clear(Pixel.Presets.DarkBlue);

            for (int i = 0; i < chunks.GetLength(0); i++) for (int j = 0; j < chunks.GetLength(1); j++) {
                    GetChunk(i, j).Draw();
                }
            foreach (PhysicsEntity e in Entities) e.Draw();

            if (Editing) {
                foreach (Button b in editorButtons) b.Draw();
                //CoreGame.Instance.DrawText(new Point(0, 0), $"{tileIndex}", Pixel.Presets.Red);
                Sprite currentTileSprite = TileManager.GetTexture((byte)tileIndex);
                if (currentTileSprite != null) {
                    Sprite previewTexture = new Sprite(Tile.TileSize, Tile.TileSize);
                    Sprite.Copy(currentTileSprite, previewTexture);
                    for (int x = 0; x < previewTexture.Width; x++) {
                        for (int y = 0; y < previewTexture.Height; y++) {
                            Pixel current = previewTexture[x, y];
                            previewTexture[x, y] = new Pixel(current.R, current.G, current.B, 100);
                        }
                    }

                    int tileLocX = (int)/*(int)Math.Round*/((double)(CoreGame.Instance.MouseX / Tile.TileSize)) * Tile.TileSize;
                    int tileLocY = (int)/*(int)Math.Round*/((double)(CoreGame.Instance.MouseY / Tile.TileSize)) * Tile.TileSize;

                    PixelEngine.Extensions.Transforms.Transform transform = new PixelEngine.Extensions.Transforms.Transform();
                    transform.Translate(tileLocX, tileLocY);
                    PixelEngine.Extensions.Transforms.Transform.DrawSprite(previewTexture, transform);
                }
            }
        }

        #region Editor Buttons
        private void EditorButtonSave_Pressed(Button pressed) => SaveLevel();
        private void EditorButtonTileDialog_Pressed(Button pressed) {
            throw new NotImplementedException();
        }
        #endregion Editor Buttons

        #region Saving / Loading
        public static Level LoadLevel(string levelName) {
            string path = $"Assets/Levels/{levelName}.bin";
            if (System.IO.File.Exists(path)) return LevelIO.LoadLevel(path);
            else return new Level(levelName);
        }
        public void SaveLevel() {
            string path = $"Assets/Levels/{Name}.bin";
            LevelIO.SaveLevel(this, path);
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

        class ExitConfirmDialogue : Scene {
            Button yes, no, cancel;
            Button[] controls;

            static string ask = "Would you like to save?";

            public ExitConfirmDialogue() {
                yes = new Button("Yes", ask.Length * 8 + 1 + "Yes".Length * 4 + 1 + 10, 5);
                no = new Button("No", ask.Length * 8 + 2 + "Yes".Length * 8 + 1 + "No".Length * 4 + 1 + 22, 5);
                cancel = new Button("Cancel", ask.Length * 8 + 2 + "Yes".Length * 8 + 2 + "No".Length * 8 + 1 + "Cancel".Length * 4 + 1 + 34, 5);
                controls = new[] { yes, no, cancel };

                yes.Pressed += Yes_Pressed;
                no.Pressed += No_Pressed;
                cancel.Pressed += Cancel_Pressed;
            }

            private void Yes_Pressed(Button pressed) {
                Instance.SaveLevel();
                CoreGame.Instance.PopScene();
                CoreGame.Instance.PopScene();
            }

            private void No_Pressed(Button pressed) {
                CoreGame.Instance.PopScene();
                CoreGame.Instance.PopScene();
            }

            private void Cancel_Pressed(Button pressed) {
                CoreGame.Instance.PopScene();
                Instance.Editing = true;
            }

            public override void Update(float fElapsedTime) {
                foreach (Button b in controls) b.Update(fElapsedTime);
                if (CoreGame.Instance.GetKey(Key.Escape).Pressed) cancel.Press();
            }

            public override void Draw() {
                CoreGame.Instance.FillRect(new Point(0, 0), CoreGame.Instance.ScreenWidth, 10, Pixel.Presets.Black);
                CoreGame.Instance.DrawText(new Point(0, 0), ask, Pixel.Presets.White);
                foreach (Button b in controls) b.Draw();
            }
        }
        #endregion Saving / Loading
    }
}
