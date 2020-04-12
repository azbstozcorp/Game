using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoreModule.Terrain;

namespace CoreModule.Saving {
    public static partial class Saver {
        public static Dictionary<Type, Serializer> Serializers { get; } = new Dictionary<Type, Serializer>() {
            {typeof(string), (object s) => Encoding.ASCII.GetBytes((string)s) },
            {typeof(int), (object i) => BitConverter.GetBytes((int)i) },
            {typeof(float), (object i) => BitConverter.GetBytes((float)i) },

            {typeof(Tile[,]), (object a) => {
                Tile[,] array = (Tile[,])a; // Cast object into tile array, will throw if not 2D tile array
                List<byte> bytes = new List<byte>(); // Data stream

                int width = array.GetLength(0);  // Get width
                int height = array.GetLength(1); // Get height

                bytes.AddRange(BitConverter.GetBytes(width));  // Write width
                bytes.AddRange(BitConverter.GetBytes(height)); // Write height

                // Write tiles
                // Flattened in columns
                // 0  4  8  12
                // 1  5  9  13
                // 2  6  10 14
                // 3  7  11 15
                // Is flattened to
                // 0  1  2  3  4  5  6  7  8  9  10 11 12 13 14 15
                for(int x = 0; x < width; x++) {
                    for(int y = 0; y < height; y++) {
                        Tile current = array[x,y];
                        bytes.Add(current.Type);
                    }
                }

                return bytes.ToArray();
            } },

        };
    }
}
