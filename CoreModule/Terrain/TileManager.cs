using System.Collections.Generic;

using PixelEngine;

namespace CoreModule.Terrain {
    public static class TileManager {
        public static Dictionary<byte, List<Sprite>> Graphics { get; } = new Dictionary<byte, List<Sprite>>();
        public static List<byte> Collideable { get; } = new List<byte>();

        public static Sprite GetTexture(byte type) {
            if (!Graphics.ContainsKey(type)) return Graphics[0][0];
            return CoreGame.Instance.Random(Graphics[type]);
        }

        public static void Setup() {
            string[] manifest = System.IO.File.ReadAllLines("Assets/Terrain/manifest.txt");

            Graphics[1] = new List<Sprite>() {
            null,};
            Graphics[0] = new List<Sprite>();

            foreach (string s in manifest) {
                string[] data = s.Split(' ');
                byte key = (byte)int.Parse(data[0]);
                string name;
                for (int i = 1; i <= int.Parse(data[3]); i++) {
                    if (int.Parse(data[3]) > 1)

                        name = $"Assets/Terrain/{data[1]}_{i}.png"; 
                    else
                        name = $"Assets/Terrain/{data[1]}.png";
                    bool collide = bool.Parse(data[2]);

                    if (!Graphics.ContainsKey(key)) 
                        Graphics[key] = new List<Sprite>();

                    Graphics[key].Add(Sprite.Load(name));
                    if (collide) Collideable.Add(key);
                }
            }
        }

        public static bool IsSolid(Tile t) {
            return t != null && Collideable.Contains(t.Type);
        }
    }
}
