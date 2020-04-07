using System;

namespace CoreModule.Saving {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    class Saveable : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    class At : Attribute {
        public string Folder { get; }
        public At(string folder) { Folder = folder; }
    }
}
