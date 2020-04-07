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
                        #region Find member type and value
                        Type t = null; // MemberInfo does not contain type or value directly
                        object value = null;

                        if (member is PropertyInfo p) {
                            t = p.PropertyType;
                            value = p.GetValue(obj);
                        }
                        if (member is FieldInfo f) {
                            t = f.FieldType;
                            value = f.GetValue(obj);
                        }
                        #endregion Find member type and value

                        #region Work with member
                        #region Check if member is Saveable
                        var memberTypeAttributes = t.GetCustomAttributes();
                        bool memberIsSaveable = false;

                        foreach (Attribute a in memberTypeAttributes) {
                            if (a is Saveable) {
                                memberIsSaveable = true;
                                break;
                            }
                        }
                        #endregion Check if member is Saveable

                        if (memberIsSaveable) { // If the member itself is a Saveable type, save it
                            if (value != null)
                                Save(value, $"{oldPath}{name}/{(saveto.Folder == "root" ? "" : saveto.Folder)}", member.Name);
                        }
                        else { // Otherwise, add the member to the dictionary of members to save to files
                            if (!folders.ContainsKey(saveto.Folder)) folders[saveto.Folder] = new List<MemberInfo>();
                            folders[saveto.Folder].Add(member);
                        }
                        #endregion Work with member
                    }
                }
            }
            #endregion Find save structure

            #region Write non-Saveable members to files
            foreach (string s in folders.Keys) {
                string subfolderPath = $"{root}{s}/"; // Get the global path to the subfolder within the save the member resides in
                if (s == "root") subfolderPath = root; // If the member is saved At("root"), the subfolder is the object's root folder
                if (!Directory.Exists(subfolderPath)) Directory.CreateDirectory(subfolderPath);

                foreach (MemberInfo info in folders[s]) {
                    #region Get member type and value
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
                    #endregion Get member type and value

                    #region Write member to file
                    if (Serializers.ContainsKey(t)) { // If the saver knows how to serialize the member
                        string pathToMember = $"{subfolderPath}{info.Name}"; // Get global path to the name folder
                        Directory.CreateDirectory(pathToMember); // Create folder for member data

                        try {
                            File.WriteAllBytes(pathToMember + "/type", Encoding.ASCII.GetBytes(t.ToString())); // Write type of member to file
                            File.WriteAllBytes(pathToMember + "/data", Serializers[t](v)); // Write data of member to file
                        }
                        catch { }
                    }
                    else throw new ArgumentException($"Cannot serialize type {t}"); // Otherwise, can't serialize
                    #endregion Write member to file
                }
            }
            #endregion Write non-Saveable members to file

            #region Clean up files
            if (File.Exists($"{final}.backup")) File.Delete($"{final}.backup"); // If a backup exists, delete it
            if (File.Exists(final)) File.Move(final, $"{final}.backup"); // Create new backup
            ZipFile.CreateFromDirectory(root, final); // Move working directory to a .save file
            Directory.Delete(root, true); // Delete the working directory
            #endregion Clean up files
            #endregion Object is saveable
        }

        /// <summary>
        /// Loads a Saveable from a .save
        /// </summary>
        /// <param name="path">path to directory</param>
        /// <param name="name">name of .save</param>
        public static object Load(string path, string name) {
            #region Path Housekeeping
            if (!path.EndsWith("/")) path += "/"; // Make sure path ends with /
            string oldPath = path; // store the local path
            path = $"{AppDomain.CurrentDomain.BaseDirectory}/{path}/"; // Get the global path
            string root = $"{path}{name}"; // Get the working directory
            #endregion Path Housekeeping

            if (Directory.Exists(root)) Directory.Delete(root, true); // If a working directory exists, delete it
            ZipFile.ExtractToDirectory(root + ".save", root); // Extract the .save to the working directory

            #region Get type to load
            Type type = Type.GetType(File.ReadAllText($"{root}/type")); // Parse object type from it's type file
            object obj = Activator.CreateInstance(type); // Create an instance of an object of that type
            object[] attributes = type.GetCustomAttributes(true); // Get the attributes of the object

            #region Check if type is Saveable
            bool loadable = false;

            foreach (object attribute in attributes)
                if (attribute is Saveable) loadable = true;
            if (!loadable) throw new ArgumentException("Object is not loadable"); // Type in .save typefile isn't Saveable, something went wrong
            #endregion Check if type is Saveable
            #endregion Get type to load

            #region Load members
            Dictionary<string, List<MemberInfo>> folders = new Dictionary<string, List<MemberInfo>>();

            #region Get members to load
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

                        if (memberIsSaveable) { // If the member type is Saveable, load it from it's own embedded .save
                            if (member is PropertyInfo propertyInfo) {
                                if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/{oldPath}{name}/{(saveto.Folder == "root" ? "" : saveto.Folder)}/{member.Name}.save"))
                                    propertyInfo.SetValue(obj, Load(oldPath + name + $"/{(saveto.Folder == "root" ? "" : saveto.Folder)}", member.Name));
                            }
                            if (member is FieldInfo fieldInfo) {
                                if (File.Exists($"{AppDomain.CurrentDomain.BaseDirectory}/{oldPath}{name}/{(saveto.Folder == "root" ? "" : saveto.Folder)}/{member.Name}.save"))
                                    fieldInfo.SetValue(obj, Load(oldPath + name + $"/{(saveto.Folder == "root" ? "" : saveto.Folder)}", member.Name));
                            }
                        }
                        else { // Otherwise, add it to the list of members to load from file
                            if (!folders.ContainsKey(saveto.Folder)) folders[saveto.Folder] = new List<MemberInfo>();
                            folders[saveto.Folder].Add(member);
                        }
                    }
                }
            }
            #endregion Get members to load

            #region Load non-Saveable members
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
            #endregion load non-Saveable members
            #endregion Load members

            Directory.Delete(root, true); // Remove working directory
            return obj; // Return loaded object
        }
    }
}
