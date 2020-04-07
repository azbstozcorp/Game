using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace CoreModule.Saving {
    public static partial class Saver {
        /// <summary>
        /// Delegate for a method which takes an object and converts it to an array of bytes
        /// </summary>
        public delegate byte[] Serializer(object obj);
        /// <summary>
        /// Delegate for a method which takes an array of bytes and converts it to an object
        /// </summary>
        public delegate object Deserializer(byte[] data);

        /// <summary>
        /// Saves an object decorated with <see cref="Saveable"/>
        /// </summary>
        /// <param name="obj">the object</param>
        /// <param name="path">folder to save to</param>
        /// <param name="name">name of save file</param>
        public static void Save(object obj, string path, string name) {
            Type type = obj.GetType(); // Get the type of the object so attributes and members can be found

            #region Path Housekeeping
            if (!path.EndsWith("/")) path += "/"; // Append slash if it doesn't exist
            string oldPath = path; // Store the original parameter (with slash)
            path = $"{AppDomain.CurrentDomain.BaseDirectory}/{path}"; // Get global path to resource
            string root = $"{path}{name}/"; // Get global path to root of save
            string final = $"{path}{name}.save"; // Get global path to the final save file
            if (!Directory.Exists(root)) Directory.CreateDirectory(root); // Create a directory for working files to be written to
            #endregion Path Housekeeping

            #region Check if the object is saveable
            object[] attributes = type.GetCustomAttributes(true); // Get attributes of the object
            bool saveable = false;

            foreach (object attribute in attributes) // Loop through attributes of the type looking for a Saveable attribute
                if (attribute is Saveable) saveable = true;
            if (!saveable) throw new ArgumentException("Object is not saveable"); // Can't save the object if it isn't Saveable
            #endregion Check if the object is saveable

            #region Object is saveable
            File.WriteAllBytes(root + "type", Encoding.ASCII.GetBytes(type.ToString())); // Write type of object to file

            #region Find save structure
            Dictionary<string, List<MemberInfo>> folders = new Dictionary<string, List<MemberInfo>>(); // Store which members go where

            foreach (MemberInfo member in type.GetMembers()) {
                if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property) // Check if member is data container
                    continue; // Can only serialize objects which contain data

                foreach (Attribute attribute in member.GetCustomAttributes()) {
                    if (attribute is At saveto) { // Only save member if it has a location in the save file
                        Type t = null;
                        object value = null;
                        if (member is PropertyInfo p) {
                            t = p.PropertyType;
                            value = p.GetValue(obj);
                        }
                        if (member is FieldInfo f) {
                            t = f.FieldType;
                            value = f.GetValue(obj);
                        }

                        var memberTypeAttributes = t.GetCustomAttributes();
                        bool memberIsSaveable = false;
                        foreach (Attribute a in memberTypeAttributes) {
                            if (a is Saveable) {
                                memberIsSaveable = true;
                                break;
                            }
                        }

                        if (memberIsSaveable) {
                            if (value != null)
                                Save(value, $"{oldPath}{name}/{(saveto.Folder == "root" ? "" : saveto.Folder)}", member.Name);
                        }
                        else {
                            if (!folders.ContainsKey(saveto.Folder)) folders[saveto.Folder] = new List<MemberInfo>();
                            folders[saveto.Folder].Add(member);
                        }
                    }
                }
            }
            #endregion Find save structure

            foreach (string s in folders.Keys) {
                string subfolderPath = $"{root}{s}/";
                if (s == "root") subfolderPath = root;
                if (!Directory.Exists(subfolderPath)) Directory.CreateDirectory(subfolderPath);
                foreach (MemberInfo info in folders[s]) {
                    object v = null;
                    Type t = null;

                    if (info is PropertyInfo p) {
                        v = p.GetValue(obj);
                        t = p.PropertyType;
                    }
                    else if (info is FieldInfo f) {
                        v = f.GetValue(obj);
                        t = f.FieldType;
                    }

                    if (Serializers.ContainsKey(t)) {
                        string pathToMember = $"{subfolderPath}{info.Name}";
                        Directory.CreateDirectory(pathToMember);

                        try {
                            File.WriteAllBytes(pathToMember + "/type", Encoding.ASCII.GetBytes(t.ToString()));
                            File.WriteAllBytes(pathToMember + "/data", Serializers[t](v));
                        }
                        catch { }
                    }
                    else throw new ArgumentException($"Cannot serialize type {t}");
                }
            }

            if (File.Exists($"{final}.backup")) File.Delete($"{final}.backup");
            if (File.Exists(final)) File.Move(final, $"{final}.backup");
            ZipFile.CreateFromDirectory(root, final);
            Directory.Delete(root, true);
            #endregion Object is saveable
        }

        public static object Load(string path, string name) {
            if (!path.EndsWith("/")) path += "/";
            string oldPath = path;
            path = $"{AppDomain.CurrentDomain.BaseDirectory}/{path}/";
            string root = $"{path}{name}";

            if (Directory.Exists(root)) Directory.Delete(root, true);
            ZipFile.ExtractToDirectory(root + ".save", root);

            Type type = Type.GetType(File.ReadAllText($"{root}/type"));
            object obj = Activator.CreateInstance(type);
            object[] attributes = type.GetCustomAttributes(true);

            bool loadable = false;
            foreach (object attribute in attributes)
                if (attribute is Saveable) loadable = true;
            if (!loadable) throw new ArgumentException("Object is not saveable");

            Dictionary<string, List<MemberInfo>> folders = new Dictionary<string, List<MemberInfo>>();
            foreach (MemberInfo member in type.GetMembers()) {
                if (member.MemberType != MemberTypes.Field && member.MemberType != MemberTypes.Property) continue;
                foreach (Attribute attribute in member.GetCustomAttributes()) {
                    if (attribute is At saveto) {
                        Type t = null;
                        if (member is PropertyInfo p) {
                            t = p.PropertyType;
                        }
                        if (member is FieldInfo f) {
                            t = f.FieldType;
                        }

                        var memberTypeAttributes = t.GetCustomAttributes();
                        bool memberIsSaveable = false;
                        foreach (Attribute a in memberTypeAttributes) {
                            if (a is Saveable) {
                                memberIsSaveable = true;
                                break;
                            }
                        }

                        if (memberIsSaveable) {
                            if (member is PropertyInfo propertyInfo) {
                                if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/{oldPath}{name}/{(saveto.Folder == "root" ? "" : saveto.Folder)}/{member.Name}.save"))
                                    propertyInfo.SetValue(obj, Load(oldPath + name + $"/{(saveto.Folder == "root" ? "" : saveto.Folder)}", member.Name));
                            }
                            if (member is FieldInfo fieldInfo) {
                                if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/{oldPath}{name}/{(saveto.Folder == "root" ? "" : saveto.Folder)}/{member.Name}.save"))
                                    fieldInfo.SetValue(obj, Load(oldPath + name + $"/{(saveto.Folder == "root" ? "" : saveto.Folder)}", member.Name));
                            }
                        }
                        else {
                            if (!folders.ContainsKey(saveto.Folder)) folders[saveto.Folder] = new List<MemberInfo>();
                            folders[saveto.Folder].Add(member);
                        }
                    }
                }
            }

            foreach (string s in folders.Keys) {
                string subfolderPath = $"{root}/{s}/";
                if (s == "root") subfolderPath = root;
                foreach (MemberInfo info in folders[s]) {
                    string pathToMember = $"{subfolderPath}/{info.Name}";

                    Type t = Type.GetType(File.ReadAllText(pathToMember + "/type"));

                    if (Deserializers.ContainsKey(t)) {
                        try {
                            if (info is PropertyInfo p) {
                                p.SetValue(obj, Deserializers[t](File.ReadAllBytes(pathToMember + "/data")));
                            }
                            if (info is FieldInfo f) {
                                f.SetValue(obj, Deserializers[t](File.ReadAllBytes(pathToMember + "/data")));
                            }
                        }
                        catch { }
                    }
                    else throw new ArgumentException($"Cannot deserialize type {t}");
                }
            }
            Directory.Delete(root, true);

            return obj;
        }
    }
}
