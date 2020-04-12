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
        public string Name { get; private set; } = "";

        public LevelState CurrentState { get; private set; }
        PlayState PlayingState = new PlayState();

        public List<Entity> Entities { get; } = new List<Entity>();
        public List<LevelTrigger> LevelTriggers { get; } = new List<LevelTrigger>();
        public ParticleManager ParticleManager { get; } = new ParticleManager();

        public Player Player { get; private set; }
        public Point CameraLocation;
        public ChunkSet chunks;

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

            chunks = new ChunkSet();

            SetTile(new Tile(2), 0, 0, 2, 4);
        }
        #endregion Constructors

        #region Chunk Utilities
        /// <summary>
        /// Returns the chunk at a given index into the world, or null if the index is out of range.
        /// </summary>
        public Chunk GetChunk(int worldX, int worldY) {
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
        public void SetTile(Tile t, int worldX, int worldY, int tileX, int tileY) {
            if (GetChunk(worldX, worldY) == null) {
                chunks.SetChunk(new Chunk(), worldX, worldY);
            } 
            GetChunk(worldX, worldY).SetTile(t, tileX, tileY);
        }

        public static Point ScreenToWorld(Point p) => p - Instance.CameraLocation;
        public static Point WorldToScreen(Point p) => p + Instance.CameraLocation;
        #endregion Chunk Utilities

        void Exit() {
            Save();
            CoreGame.Instance.PopScene();
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

            for (int i = 0; i < chunks.Width; i++) {
                for (int j = 0; j < chunks.Height; j++) {
                    GetChunk(i, j)?.Draw();
                }
            }

            CurrentState.Draw();
            foreach (PhysicsEntity e in Entities) e.Draw();
            foreach (LevelTrigger t in LevelTriggers) t.Draw();

            ParticleManager.Draw();
        }

        #region States
        class PlayState : LevelState {
            public override LevelState TryMoveNext() {
                return this;
            }

            public override void Update(float fElapsedTime) {
                base.Update(fElapsedTime);

                //Instance.Player.Update(fElapsedTime);

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

                if (CoreGame.Instance.GetKey(Key.Escape).Pressed) Instance.Exit();
            }

            public override void Draw() {
                base.Draw();

                if (CoreGame.Instance.GetMouse(Mouse.Left).Pressed) {
                    Point from = Instance.Player.Bounds.TopLeft;
                    PointF through = new PointF(CoreGame.Instance.MouseX, CoreGame.Instance.MouseY).ToWorld();

                    Ballistics.Ballistics.Fire(from, through);
                }
            }
        }
        #endregion States

        #region    Saving / Loading

        public void Save() {
            string pathToLevel = $"Assets/Levels/{Name}";
            string pathToChunks = $"{pathToLevel}/chunks";

            chunks.SaveTo(pathToChunks);
        }
        public static Level LoadLevel(string name) {
            string pathToLevel = $"Assets/Levels/{name}";
            string pathToChunks = $"{pathToLevel}/chunks";

            Level l = new Level(name);
            l.chunks.LoadFrom(pathToChunks);

            return l;
        }

        #endregion Saving / Loading
    }
}
