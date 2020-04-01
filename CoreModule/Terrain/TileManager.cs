using System.Collections.Generic;

using PixelEngine;
using static CoreModule.Terrain.TerrainType;

namespace CoreModule.Terrain {
    public static class TileManager {
        public static Dictionary<TerrainType, Sprite> Graphics { get; } = new Dictionary<TerrainType, Sprite>();
        public static List<TerrainType> Collideable { get; } = new List<TerrainType>();

        public static Sprite GetTexture(TerrainType type) {
            if (!Graphics.ContainsKey(type)) return Graphics[0];
            return Graphics[type];
        }

        public static void Setup() {
            string[] manifest = System.IO.File.ReadAllLines("Assets/Terrain/manifest.txt");

            Graphics[TT_AIR] = null;
            Graphics[TT_UNDEFINED] = null;

            foreach (string s in manifest) {
                string[] data = s.Split(' ');
                TerrainType key = (TerrainType)int.Parse(data[0]);
                string name;
                for (int i = 0; i < int.Parse(data[3]); i++) {
                    if (int.Parse(data[3]) > 1)

                        name = $"Assets/Terrain/{data[1]}_{i}.png";
                    else
                        name = $"Assets/Terrain/{data[1]}.png";
                    bool collide = bool.Parse(data[2]);

                    Graphics[key] = Sprite.Load(name);
                    if (collide) Collideable.Add(key);
                }
            }
        }

        public static bool IsSolid(Tile t) {
            return t != null && Collideable.Contains(t.Type);
        }
    }
}
