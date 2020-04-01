using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using PixelEngine;
using CoreModule.Terrain;
using CoreModule.Scenes;

using Chunk = CoreModule.Terrain.Chunk;

namespace CoreModule.Saving {
    public interface ISerializable<T> {
        byte[] GetSaveData();
        void LoadSaveData(byte[] data);
    }

    public static class LevelIO {
        public static void SaveLevel(Level level, string path) {
            Stream stream = File.Open(path, FileMode.Create);
            byte[] data = level.GetSaveData();
            stream.Write(data, 0, data.Length);
            stream.Close();
        }

        public static Level LoadLevel(string path) {
            Level level = new Level();
            level.LoadSaveData(File.ReadAllBytes(path));
            return level;
        }
    }
}