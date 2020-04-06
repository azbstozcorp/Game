using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using PixelEngine;
using CoreModule.Shapes;
using CoreModule.Saving;
using CoreModule.Terrain;
using CoreModule.Entities;
using CoreModule.Drawables;
using CoreModule.Entities.Particles;

using Point = CoreModule.Shapes.Point;

namespace CoreModule.Scenes {
    public class Level : Scene, ISerializable<Level> {
        public static Level Instance { get; private set; }
        public string Name { get; private set; } = "";

        public LevelState CurrentState { get; private set; }
        PlayState PlayingState = new PlayState();
        EditorState EditingState = new EditorState();

        public List<Entity> Entities { get; } = new List<Entity>();
        public List<LevelTrigger> LevelTriggers { get; } = new List<LevelTrigger>();
        public ParticleManager ParticleManager { get; } = new ParticleManager();

        public Player Player { get; private set; }
        public Point CameraLocation;
        public Chunk[,] chunks;

        #region Editor Variables
        public bool Editing { get; private set; } = false;
        #endregion Editor Variables

        #region Constructors
        public Level() {
            Console.WriteLine($"Initializing World...");
            Instance = this;
            CameraLocation = new Point();
            TileManager.Setup();
            Drawables.Add(ParticleManager);
            Player = new Player(20, 80);

            CurrentState = PlayingState;
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

        public static Point ScreenToWorld(Point p) => p - Instance.CameraLocation;
        public static Point WorldToScreen(Point p) => p + Instance.CameraLocation;
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
            CurrentState = CurrentState.TryMoveNext();
            CurrentState.Update(fElapsedTime);

            if (CoreGame.Instance.GetKey(Key.Escape).Pressed) {
                Exit();
                return;
            }
            if (CoreGame.Instance.GetKey(Key.T).Pressed) CoreGame.Instance.PushScene(new TileEditor());
        }
        public override void Draw() {
            CoreGame.Instance.Clear(Pixel.Presets.DarkBlue);

            Player.Draw();
            for (int i = 0; i < chunks.GetLength(0); i++) for (int j = 0; j < chunks.GetLength(1); j++) {
                    GetChunk(i, j).Draw();
                }
            CurrentState.Draw();
            foreach (PhysicsEntity e in Entities) e.Draw();
            foreach (LevelTrigger t in LevelTriggers) t.Draw();

            ParticleManager.Draw();
        }

        #region States
        class PlayState : LevelState {
            public override LevelState TryMoveNext() {
                if (CoreGame.Instance.GetKey(Key.E).Pressed && CoreGame.Instance.GetKey(Key.Control).Down) {
                    Instance.Editing = true;
                    return Instance.EditingState;
                }
                else return this;
            }

            public override void Update(float fElapsedTime) {
                base.Update(fElapsedTime);

                Instance.Player.Update(fElapsedTime);

                int cameraRatio = 1;
                float newX = -Instance.Player.X +
                            CoreGame.Instance.ScreenWidth / 2 +
                            Instance.Player.Bounds.Width / 2 -
                            CoreGame.Instance.MouseX / cameraRatio +
                            CoreGame.Instance.ScreenWidth / (cameraRatio * 2);
                float newY = -Instance.Player.Y +
                            CoreGame.Instance.ScreenHeight / 2 +
                            Instance.Player.Bounds.Height / 2 -
                            CoreGame.Instance.MouseY / cameraRatio +
                            CoreGame.Instance.ScreenHeight / (cameraRatio * 2);

                newX = CoreGame.Instance.Lerp(Instance.CameraLocation.X, newX, 0.1f);
                newY = CoreGame.Instance.Lerp(Instance.CameraLocation.Y, newY, 0.1f);

                Instance.CameraLocation.X = (int)newX;
                Instance.CameraLocation.Y = (int)newY;

                foreach (PhysicsEntity e in Instance.Entities) e.Update(fElapsedTime);
                foreach (LevelTrigger t in Instance.LevelTriggers) t.Update(fElapsedTime);
            }

            public override void Draw() {
                base.Draw();

                if (CoreGame.Instance.GetMouse(Mouse.Left).Pressed) {
                    Sound.SoundPlayer.PlayOnce("C:/Users/horac/source/repos/CoreModule/Game/Game/Assets/Audio/GS_handgun_bass.wav");
                    Point from = Instance.Player.Bounds.TopLeft;
                    PointF through = ScreenToWorld(new PointF(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY));

                    float angle = (float)(Math.Atan2(through.Y - from.Y, through.X - from.X));
                    Line result = new Line(from, angle, 1000);

                    int dirX = -Math.Sign(through.X - Instance.Player.X);
                    int dirY = -Math.Sign(through.Y - Instance.Player.Y);
                    Instance.CameraLocation.X -= dirX * 10;
                    Instance.CameraLocation.Y -= dirY * 3;

                    Rect screen = new Rect(ScreenToWorld(new Point(0, 0)), ScreenToWorld(new Point(CoreGame.Instance.ScreenWidth, CoreGame.Instance.ScreenHeight)));
                    HashSet<Chunk> chunks = new HashSet<Chunk> {
                        Instance.GetChunkWithPoint(screen.TopLeft),
                        Instance.GetChunkWithPoint(screen.TopRight),
                        Instance.GetChunkWithPoint(screen.BottomLeft),
                        Instance.GetChunkWithPoint(screen.BottomRight)
                    };
                    //List<Chunk> chunks = Instance.chunks.Cast<Chunk>().ToList();

                    List<Point> points = new List<Point>();
                    foreach (Chunk c in chunks) {
                        foreach (Rect r in c.Colliders) {
                            Rect check = r.Copy;
                            check.Move(c.Bounds.Left, c.Bounds.Top);
                            if (Collision.LineOverlapsRect(result, check)) {
                                if (Collision.LinesIntersect(result, check.TLBL)) points.Add(Collision.IntersectionOf(result, check.TLBL));
                                if (Collision.LinesIntersect(result, check.BRTR)) points.Add(Collision.IntersectionOf(result, check.BRTR));
                                if (Collision.LinesIntersect(result, check.TLTR)) points.Add(Collision.IntersectionOf(result, check.TLTR));
                                if (Collision.LinesIntersect(result, check.BRBL)) points.Add(Collision.IntersectionOf(result, check.BRBL));
                            }
                        }
                    }

                    if (points.Count > 0) {
                        Point closest = WorldToScreen(Collision.Closest(from, points.ToArray()));

                        for (int i = 0; i < 5; i++) {
                            Instance.ParticleManager.AddParticle(
                                new Hit(ScreenToWorld(closest + (dirX, 0)).X, ScreenToWorld(closest).Y, CoreGame.Instance.Random(dirX * 1f, dirX * 4f),
                                    CoreGame.Instance.Random(-4f, 4f),
                                    CoreGame.Instance.GetScreenPixel(closest.X - dirX * 2, closest.Y - dirY)) 
                                        { Bounciness = CoreGame.Instance.Random(0.3f, 0.5f) }
                                );
                        }


                        CoreGame.Instance.DrawLine(WorldToScreen(from), closest, Pixel.Presets.White);
                    }
                    else CoreGame.Instance.DrawLine(WorldToScreen(from), WorldToScreen(result.End), Pixel.Presets.White);
                }
            }
        }

        class EditorState : LevelState {
            int tileIndex = 2;

            public override LevelState TryMoveNext() {
                if (CoreGame.Instance.GetKey(Key.E).Pressed && CoreGame.Instance.GetKey(Key.Control).Down) {
                    Instance.Editing = false;
                    return Instance.PlayingState;
                }
                else return this;
            }

            public EditorState() {
                Button editorButtonSave = new Button("Save", CoreGame.Instance.ScreenWidth - "Save".Length * 4 - 1, (10 * (Drawables.Count() + 1)) + Drawables.Count + 1);
                editorButtonSave.Pressed += EditorButtonSave_Pressed;
                Drawables.Add(editorButtonSave);

                Button editorButtonTileDialog = new Button("Tiles", CoreGame.Instance.ScreenWidth - "Tiles".Length * 4 - 1, (10 * Drawables.Count() + 1) + Drawables.Count + 1);
                editorButtonTileDialog.Pressed += EditorButtonTileDialog_Pressed;
                Drawables.Add(editorButtonTileDialog);
            }

            private void EditorButtonSave_Pressed(Button pressed) => Instance.SaveLevel();
            private void EditorButtonTileDialog_Pressed(Button pressed) {
            }

            public override void Update(float fElapsedTime) {
                base.Update(fElapsedTime);
                int chunkMouseX = (CoreGame.Instance.MouseX - Instance.CameraLocation.X) / Chunk.ChunkSize;
                int chunkMouseY = (CoreGame.Instance.MouseY - Instance.CameraLocation.Y) / Chunk.ChunkSize;
                int tileMouseX = ((CoreGame.Instance.MouseX - Instance.CameraLocation.X) % Chunk.ChunkSize) / Tile.TileSize;
                int tileMouseY = ((CoreGame.Instance.MouseY - Instance.CameraLocation.Y) % Chunk.ChunkSize) / Tile.TileSize;

                if (CoreGame.Instance.GetKey(Key.W).Down) Instance.CameraLocation.Y++;
                if (CoreGame.Instance.GetKey(Key.S).Down) Instance.CameraLocation.Y--;
                if (CoreGame.Instance.GetKey(Key.A).Down) Instance.CameraLocation.X++;
                if (CoreGame.Instance.GetKey(Key.D).Down) Instance.CameraLocation.X--;

                tileIndex += (int)CoreGame.Instance.MouseScroll;
                if (tileIndex == 0) tileIndex = TileManager.MaxValue;
                if (tileIndex > TileManager.MaxValue) tileIndex = 1;

                if (CoreGame.Instance.GetMouse(Mouse.Left).Down && (byte)tileIndex != Instance.GetChunk(chunkMouseX, chunkMouseY).GetTile(tileMouseX, tileMouseY)?.Type) {
                    Instance.GetChunk(chunkMouseX, chunkMouseY).SetTile(new Tile((byte)tileIndex), tileMouseX, tileMouseY);
                }
            }

            public override void Draw() {
                base.Draw();

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

                    int tileLocX = (CoreGame.Instance.MouseX / Tile.TileSize) * Tile.TileSize;
                    int tileLocY = (CoreGame.Instance.MouseY / Tile.TileSize) * Tile.TileSize;

                    PixelEngine.Extensions.Transforms.Transform transform = new PixelEngine.Extensions.Transforms.Transform();
                    transform.Translate(tileLocX, tileLocY);
                    PixelEngine.Extensions.Transforms.Transform.DrawSprite(previewTexture, transform);
                }
            }
        }
        #endregion States

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

            List<byte> triggerData = new List<byte>();
            foreach (LevelTrigger trigger in LevelTriggers) {
                byte[] tData = trigger.GetSaveData();
                int tDataLength = tData.Length;
                triggerData.AddRange(BitConverter.GetBytes(tDataLength));
                triggerData.AddRange(tData);
            }
            data.AddRange(BitConverter.GetBytes(LevelTriggers.Count));
            data.AddRange(triggerData);

            return data.ToArray();
        }
        public void LoadSaveData(byte[] data) {
            int location = 0;

            byte nameLength = data[location]; /*                             */ location++;
            string name = Encoding.ASCII.GetString(data, location, nameLength); location += nameLength;
            Name = name;

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
            if (location == data.Length) return;

            int numTriggers = BitConverter.ToInt32(data, location); location += sizeof(int);
            for (int i = 0; i < numTriggers; i++) {
                int triggerLength = BitConverter.ToInt32(data, location); location += sizeof(int);
                LevelTrigger t = new LevelTrigger();
                t.LoadSaveData(data.Skip(location).Take(triggerLength).ToArray());
                LevelTriggers.Add(t);
                location += triggerLength;
            }
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
