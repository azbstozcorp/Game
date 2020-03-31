using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using PixelEngine;
using CoreModule.Terrain;

using Chunk = CoreModule.Terrain.Chunk;

namespace CoreModule.Saving
{
    public static class ChunkIO
    {
        public void SaveChunk(Chunk chunk, string path, int xCoord, int yCoord)
        {
            if (path[path.Length] == '\\')
            {
                path += $"chunk{xCoord}_{yCoord}.bin";
            }
            else
            {
                path += $"\\chunk{xCoord}_{yCoord}.bin";
            }


            Stream stream = File.Open(path, FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, chunk);
            stream.Close();
        }

        public Chunk LoadChunk(string path, int xCoord, int yCoord)
        {
            if (path[path.Length] == '\\')
            {
                path += $"chunk{xCoord}_{yCoord}.bin";
            }
            else
            {
                path += $"\\chunk{xCoord}_{yCoord}.bin";
            }

            Stream stream = File.Open(path, FileMode.Open);
            BinaryFormatter formatter = new BinaryFormatter();

            Chunk returnChunk = (Chunk)formatter.Deserialize(stream);
            stream.Close();      
        }
    }
}