using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreModule.Saving {
   public static partial class Saver {
        public static Dictionary<Type, Serializer> Serializers { get; } = new Dictionary<Type, Serializer>() {
            {typeof(string), (object s) => Encoding.ASCII.GetBytes((string)s) },
            {typeof(int), (object i) => BitConverter.GetBytes((int)i) },
            {typeof(float), (object i) => BitConverter.GetBytes((float)i) },
        };
    }
}
