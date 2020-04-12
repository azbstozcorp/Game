using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Saving;

namespace CoreModule.Terrain {
    public class ChunkSet {
        public Chunk this[int x, int y] {
            get => GetChunk(x, y);
            set => SetChunk(value, x, y);
        }
        Dictionary<(int x, int y), Chunk> chunks { get; } = new Dictionary<(int x, int y), Chunk>();

        public int MinX { get; private set; }
        public int MinY { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public Chunk GetChunk(int x, int y) {
            var position = (x, y);
            if (chunks.ContainsKey(position)) return chunks[position];
            else return null;
        }
        public void SetChunk(Chunk chunk, int x, int y) {
            var position = (x, y);
            chunks[position] = chunk;

            if (x > Width) Width = x;
            if (y > Height) Height = y;

            if (x < MinX) MinX = x;
            if (y < MinY) MinY = y;
        }

        public void Draw() {
            for (int x = MinX; x < Width; x++) {
                for (int y = MinX; y < Height; y++) {
                    Chunk current = GetChunk(x, y);
                    current?.Draw();
                }
            }
        }

        public void SaveTo(string path) {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            for (int x = MinX; x < Width; x++) {
                for (int y = MinX; y < Height; y++) {
                    Chunk current = GetChunk(x, y);
                    if (current == null) continue;

                    Saver.Save(current, path, $"{x},{y}");
                }
            }
        }
        public void LoadFrom(string path) {
            if (!Directory.Exists(path)) return;
            string[] files = Directory.GetFiles(path, "*.save");

            foreach (string chunk in files) {
                string[] chunkPositionString = chunk.Split(',');
                int chunkX = int.Parse(chunkPositionString[0]);
                int chunkY = int.Parse(chunkPositionString[1]);

                SetChunk((Chunk)Saver.Load(path, chunk), chunkX, chunkY);
            }
        }
    }
}
