using System.Collections.Generic;

using PixelEngine;

namespace CoreModule.Terrain {
    public static class TileManager {
        public static Dictionary<byte, Sprite> Graphics { get; } = new Dictionary<byte, Sprite>();
        public static Dictionary<byte, string> Names { get; } = new Dictionary<byte, string>();
        public static Dictionary<string, byte> IDs { get; } = new Dictionary<string, byte>();
        public static HashSet<byte> Collideable { get; } = new HashSet<byte>();
        public static byte MaxValue = 0;

        public static Sprite GetTexture(byte type) {
            if (type == 1) return null;
            if (!Graphics.ContainsKey(type)) return Graphics[0];
            return Graphics[type];
        }
        public static string GetName(byte type) {
            if (!Names.ContainsKey(type)) return null;
            return Names[type];
        }
        public static byte GetTypeFromName(string name) {
            if (!IDs.ContainsKey(name)) return 0;
            return IDs[name];
        }
        public static bool IsSolid(Tile t) {
            return t != null && Collideable.Contains(t.Type);
        }
        public static bool IsSolid(byte type) => Collideable.Contains(type);

        public static void AddToManifest(byte type, string name, bool collide) {
            string path = $"Assets/Terrain/{name}.png";

            if (type > MaxValue) MaxValue = type;

            Graphics[type] = Sprite.Load(path);
            Names[type] = name;
            IDs[name] = type;
            if (collide) Collideable.Add(type);
            else if (Collideable.Contains(type)) Collideable.Remove(type);
        }
        public static void SaveManifest() {
            List<string> manifest = new List<string>();

            foreach (byte id in Graphics.Keys) {
                if (id == 1) continue;
                string name = GetName(id);
                bool collide = IsSolid(new Tile(id));
                manifest.Add($"{id} {name} {collide}");
            }

            System.IO.File.WriteAllLines("Assets/Terrain/manifest.txt", manifest.ToArray());
        }

        public static void Setup() {
            string[] manifest = System.IO.File.ReadAllLines("Assets/Terrain/manifest.txt");
            foreach (string s in manifest) {
                string[] data = s.Split(' ');
                byte key = (byte)int.Parse(data[0]);
                string name = data[1];
                bool collide = bool.Parse(data[2]);

                AddToManifest(key, name, collide);
            }
        }
    }
}
