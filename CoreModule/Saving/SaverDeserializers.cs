using System;
using System.Collections.Generic;
using System.Text;

namespace CoreModule.Saving {
    public static partial class Saver {
        public static Dictionary<Type, Deserializer> Deserializers { get; } = new Dictionary<Type, Deserializer>() {
            {typeof(string), (byte[] data) => Encoding.ASCII.GetString(data) },
            {typeof(int), (byte[] data) => BitConverter.ToInt32(data, 0) },
            {typeof(float), (byte[] data) => BitConverter.ToSingle(data, 0) },
        };
    }
}
