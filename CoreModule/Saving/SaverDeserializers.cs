using System;
using System.Collections.Generic;
using System.Text;

using CoreModule.Terrain;

namespace CoreModule.Saving {
    public static partial class Saver {
        public static Dictionary<Type, Deserializer> Deserializers { get; } = new Dictionary<Type, Deserializer>() {
            {typeof(string), (byte[] data) => Encoding.ASCII.GetString(data) },
            {typeof(int), (byte[] data) => BitConverter.ToInt32(data, 0) },
            {typeof(float), (byte[] data) => BitConverter.ToSingle(data, 0) },

            {typeof(Tile[,]), (byte[] data) => {
                int location = 0; // Location in byte array

                int width = BitConverter.ToInt32(data, location);  // Width of tile array
                location += sizeof(int);                           // Increment location by size of int
                int height = BitConverter.ToInt32(data, location); // Height of tile array
                location += sizeof(int);                           // Increment location by size of int

                Tile[,] loaded = new Tile[width, height]; // Create array to return with loaded width and height

                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        loaded[x, y] = new Tile(data[location]);
                        location++; // Increment location by 1 byte, a tile is stored in 1 byte
                    }
                }

                return loaded;
            } },
        };
    }
}
