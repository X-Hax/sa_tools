using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SASave
{
    public static class IniFile
    {
        public static Dictionary<string, Dictionary<string, string>> Load(string filename)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> curent = new Dictionary<string, string>();
            result.Add(string.Empty, curent);
            string curgroup = string.Empty;
            string[] fc = System.IO.File.ReadAllLines(filename);
            for (int i = 0; i < fc.Length; i++)
            {
                string line = fc[i];
                StringBuilder sb = new StringBuilder(line.Length);
                bool startswithbracket = false;
                bool endswithbracket = false;
                int firstequals = -1;
                for (int c = 0; c < line.Length; c++)
                    switch (line[c])
                    {
                        case '\\': // escape character
                            if (c + 1 == line.Length) goto default;
                            c++;
                            switch (line[c])
                            {
                                case 'n': // line feed
                                    sb.Append('\n');
                                    break;
                                case 'r': // carriage return
                                    sb.Append('\r');
                                    break;
                                default: // literal character
                                    sb.Append(line[c]);
                                    break;
                            }
                            break;
                        case '=':
                            if (firstequals == -1)
                                firstequals = c;
                            goto default;
                        case '[':
                            if (c == 0)
                                startswithbracket = true;
                            goto default;
                        case ']':
                            if (c == line.Length - 1)
                                endswithbracket = true;
                            goto default;
                        case ';': // comment character, stop processing this line
                            c = line.Length;
                            break;
                        default:
                            sb.Append(line[c]);
                            break;
                    }
                line = sb.ToString();
                if (startswithbracket & endswithbracket)
                {
                    curgroup = line.Substring(1, line.Length - 2);
                    curent = new Dictionary<string, string>();
                    try
                    {
                        result.Add(curgroup, curent);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new Exception("INI File error: Group \"" + curgroup + "\" already exists.\n" + filename + ":line " + i, ex);
                    }
                }
                else if (!IsNullOrWhiteSpace(line))
                {
                    string key;
                    string value = string.Empty;
                    if (firstequals > -1)
                    {
                        key = line.Substring(0, firstequals);
                        value = line.Substring(firstequals + 1);
                    }
                    else
                        key = line;
                    try
                    {
                        curent.Add(key, value);
                    }
                    catch (ArgumentException ex)
                    {
                        throw new Exception("INI File error: Value \"" + key + "\" already exists in group \"" + curgroup + "\".\n" + filename + ":line " + i, ex);
                    }
                }
            }
            return result;
        }

        public static void Save(Dictionary<string, Dictionary<string, string>> INI, string filename)
        {
            List<string> result = new List<string>();
            foreach (KeyValuePair<string, Dictionary<string, string>> group in INI)
            {
                if (!string.IsNullOrEmpty(group.Key))
                    result.Add("[" + group.Key.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r").Replace(";", @"\;") + "]");
                foreach (KeyValuePair<string, string> value in group.Value)
                    result.Add(value.Key.Replace(@"\", @"\\").Replace("[", @"\[").Replace("=", @"\=").Replace("\n", @"\n").Replace("\r", @"\r").Replace(";", @"\;") + "=" + value.Value.Replace(@"\", @"\\").Replace("\n", @"\n").Replace("\r", @"\r").Replace(";", @"\;"));
            }
            System.IO.File.WriteAllLines(filename, result.ToArray());
        }

        public static void Serialize(object Object, string Filename)
        {
            Save(Serialize(Object), Filename);
        }

        public static Dictionary<string, Dictionary<string, string>> Serialize(object Object)
        {
            Dictionary<string, Dictionary<string, string>> ini = new Dictionary<string, Dictionary<string, string>>() { { string.Empty, new Dictionary<string, string>() } };
            SerializeInternal("value", Object, ini, string.Empty, true, false);
            return ini;
        }

        private static void SerializeInternal(string name, object value, Dictionary<string, Dictionary<string, string>> ini, string groupName, bool rootObject, bool usecollectionname)
        {
            Dictionary<string, string> group = ini[groupName];
            switch (Type.GetTypeCode(value.GetType()))
            {
                case TypeCode.Object:
                    if (value is IList)
                    {
                        int i = 0;
                        foreach (object item in (IList)value)
                            SerializeInternal(usecollectionname ? name + "[" + i++ + "]" : (i++).ToString(), item, ini, groupName, false, false);
                        break;
                    }
                    else if (value is IDictionary)
                    {
                        foreach (DictionaryEntry item in (IDictionary)value)
                            if (!item.Key.GetType().IsComplexType())
                                SerializeInternal(usecollectionname ? name + "[" + item.Key.ConvertToString() + "]" : item.Key.ConvertToString(), item.Value, ini, groupName, false, true);
                        break;
                    }
                    if (!value.GetType().IsComplexType())
                        group.Add(name, value.ConvertToString());
                    string newgroup = groupName;
                    if (!rootObject)
                    {
                        if (!string.IsNullOrEmpty(newgroup))
                            newgroup += '.';
                        newgroup += name;
                        ini.Add(newgroup, new Dictionary<string, string>());
                    }
                    foreach (MemberInfo member in value.GetType().GetMembers(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (Attribute.GetCustomAttribute(member, typeof(IniIgnoreAttribute), true) != null)
                            continue;
                        string membername = member.Name;
                        if (Attribute.GetCustomAttribute(member, typeof(IniNameAttribute), true) != null)
                            membername = ((IniNameAttribute)Attribute.GetCustomAttribute(member, typeof(IniNameAttribute), true)).Name;
                        object item;
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                item = ((FieldInfo)member).GetValue(value);
                                break;
                            case MemberTypes.Property:
                                PropertyInfo property = (PropertyInfo)member;
                                if (property.GetIndexParameters().Length > 0) continue;
                                MethodInfo getmethod = property.GetGetMethod();
                                if (getmethod == null) continue;
                                item = getmethod.Invoke(value, null);
                                break;
                            default:
                                continue;
                        }
                        SerializeInternal(membername, item, ini, newgroup, false, Attribute.GetCustomAttribute(member, typeof(IniCollectionAttribute)) == null);
                    }
                    break;
                case TypeCode.Empty:
                case TypeCode.DBNull:
                    break;
                default:
                    group.Add(name, value.ConvertToString());
                    break;
            }
        }

        public static T Deserialize<T>(string Filename)
        {
            return Deserialize<T>(Load(Filename));
        }

        public static object Deserialize(string Filename, Type type)
        {
            return Deserialize(Load(Filename), type);
        }

        public static T Deserialize<T>(Dictionary<string, Dictionary<string, string>> INI)
        {
            return (T)Deserialize(INI, typeof(T));
        }

        public static object Deserialize(Dictionary<string, Dictionary<string, string>> INI, Type type)
        {
            return DeserializeInternal("value", type, type.GetDefaultValue(), INI, string.Empty, true, false);
        }

        private static object DeserializeInternal(string name, Type type, object defaultvalue, Dictionary<string, Dictionary<string, string>> ini, string groupName, bool rootObject, bool usecollectionname)
        {
            string fullname = groupName;
            if (!rootObject)
            {
                if (!string.IsNullOrEmpty(fullname))
                    fullname += '.';
                fullname += name;
            }
            if (!ini.ContainsKey(groupName)) return defaultvalue;
            Dictionary<string, string> group = ini[groupName];
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    Type generictype;
                    if (type.IsArray)
                    {
                        Type valuetype = type.GetElementType();
                        int maxind = 0;
                        if (!IsComplexType(valuetype))
                        {
                            foreach (KeyValuePair<string, string> item in group)
                                if (!usecollectionname)
                                {
									if (int.TryParse(item.Key, out int key))
										maxind = Math.Max(key, maxind);
								}
                                else if (item.Key.StartsWith(name + "["))
                                {
                                    int key = int.Parse(item.Key.Substring(name.Length + 1, item.Key.Length - (name.Length + 2)));
                                    maxind = Math.Max(key, maxind);
                                }
                        }
                        else
                            foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
                                if (!usecollectionname)
                                {
									if (int.TryParse(item.Key, out int key))
										maxind = Math.Max(key, maxind);
								}
                                else if (item.Key.StartsWith(fullname + "["))
                                {
                                    int key = int.Parse(item.Key.Substring(fullname.Length + 1, item.Key.Length - (fullname.Length + 2)));
                                    maxind = Math.Max(key, maxind);
                                }
                        Array obj = Array.CreateInstance(valuetype, maxind);
                        if (!IsComplexType(valuetype))
                            for (int i = 0; i < maxind; i++)
                                if (group.ContainsKey(usecollectionname ? name + "[" + i + "]" : i.ToString()))
                                {
                                    obj.SetValue(ConvertFromString(valuetype, group[usecollectionname ? name + "[" + i + "]" : i.ToString()]), i);
                                    group.Remove(usecollectionname ? name + "[" + i + "]" : i.ToString());
                                }
                                else
                                    obj.SetValue(valuetype.GetDefaultValue(), i);
                        else
                            for (int i = 0; i < maxind; i++)
                                obj.SetValue(DeserializeInternal("value", valuetype, valuetype.GetDefaultValue(), ini, usecollectionname ? fullname + "[" + i + "]" : i.ToString(), true, true), i);
                        return obj;
                    }
                    else if (ImplementsGenericDefinition(type, typeof(IList<>), out generictype))
                    {
                        IList obj = (IList)Activator.CreateInstance(type);
                        Type valuetype = generictype.GetGenericArguments()[0];
                        if (!IsComplexType(valuetype))
                        {
                            int maxind = 0;
                            foreach (KeyValuePair<string, string> item in group)
                                if (!usecollectionname)
                                {
									if (int.TryParse(item.Key, out int key))
										maxind = Math.Max(key, maxind);
								}
                                else if (item.Key.StartsWith(name + "["))
                                {
                                    int key = int.Parse(item.Key.Substring(name.Length + 1, item.Key.Length - (name.Length + 2)));
                                    maxind = Math.Max(key, maxind);
                                }
                            for (int i = 0; i < maxind; i++)
                                if (group.ContainsKey(usecollectionname ? name + "[" + i + "]" : i.ToString()))
                                {
                                    obj.Add(ConvertFromString(valuetype, group[usecollectionname ? name + "[" + i + "]" : i.ToString()]));
                                    group.Remove(usecollectionname ? name + "[" + i + "]" : i.ToString());
                                }
                                else
                                    obj.Add(valuetype.GetDefaultValue());
                        }
                        else
                        {
                            int maxind = 0;
                            foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
                                if (!usecollectionname)
                                {
									if (int.TryParse(item.Key, out int key))
										maxind = Math.Max(key, maxind);
								}
                                else if (item.Key.StartsWith(fullname + "["))
                                {
                                    int key = int.Parse(item.Key.Substring(fullname.Length + 1, item.Key.Length - (fullname.Length + 2)));
                                    maxind = Math.Max(key, maxind);
                                }
                            for (int i = 0; i < maxind; i++)
                                obj.Add(DeserializeInternal("value", valuetype, valuetype.GetDefaultValue(), ini, usecollectionname ? fullname + "[" + i + "]" : i.ToString(), true, true));
                        }
                        return obj;
                    }
                    else if (type.ImplementsGenericDefinition(typeof(IDictionary<,>), out generictype))
                    {
                        IDictionary obj = (IDictionary)Activator.CreateInstance(type);
                        Type keytype = generictype.GetGenericArguments()[0];
                        Type valuetype = generictype.GetGenericArguments()[1];
                        if (keytype.IsComplexType()) return obj;
                        if (!valuetype.IsComplexType())
                        {
                            List<string> items = new List<string>();
                            foreach (KeyValuePair<string, string> item in group)
                                if (!usecollectionname)
                                    items.Add(item.Key);
                                else if (item.Key.StartsWith(name + "["))
                                    items.Add(item.Key.Substring(name.Length + 1, item.Key.Length - (name.Length + 2)));
                            foreach (string item in items)
                            {
                                obj.Add(keytype.ConvertFromString(item), valuetype.ConvertFromString(group[usecollectionname ? name + "[" + item + "]" : item]));
                                group.Remove(usecollectionname ? name + "[" + item + "]" : item);
                            }
                        }
                        else
                        {
                            List<string> items = new List<string>();
                            foreach (KeyValuePair<string, Dictionary<string, string>> item in ini)
                                if (!usecollectionname & item.Key != groupName)
                                    items.Add(item.Key);
                                else if (item.Key.StartsWith(fullname + "["))
                                    items.Add(item.Key.Substring(fullname.Length + 1, item.Key.Length - (fullname.Length + 2)));
                            foreach (string item in items)
                                obj.Add(keytype.ConvertFromString(item), DeserializeInternal("value", valuetype, valuetype.GetDefaultValue(), ini, usecollectionname ? name + "[" + item + "]" : item, true, true));
                        }
                        return obj;
                    }
                    if (!type.IsComplexType())
                    {
                        if (group.ContainsKey(name))
                        {
                            object converted = type.ConvertFromString(group[name]);
                            group.Remove(name);
                            if (converted != null)
                                return converted;
                        }
                        return defaultvalue;
                    }
                    object result = Activator.CreateInstance(type);
                    MemberInfo collection = null;
                    foreach (MemberInfo member in type.GetMembers(BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (Attribute.GetCustomAttribute(member, typeof(IniIgnoreAttribute), true) != null)
                            continue;
                        string membername = member.Name;
                        if (Attribute.GetCustomAttribute(member, typeof(IniNameAttribute), true) != null)
                            membername = ((IniNameAttribute)Attribute.GetCustomAttribute(member, typeof(IniNameAttribute), true)).Name;
                        bool collectionattr = Attribute.GetCustomAttribute(member, typeof(IniCollectionAttribute), true) != null;
                        switch (member.MemberType)
                        {
                            case MemberTypes.Field:
                                FieldInfo field = (FieldInfo)member;
                                if (collectionattr && field.FieldType.Implements(typeof(ICollection)))
                                {
                                    if (collection != null) throw new Exception("IniCollectionAttribute cannot be used on multiple members of a Type.");
                                    collection = member;
                                    continue;
                                }
                                object defval = field.FieldType.GetDefaultValue();
                                DefaultValueAttribute defattr = (DefaultValueAttribute)Attribute.GetCustomAttribute(member, typeof(DefaultValueAttribute), true);
                                if (defattr != null)
                                    defval = defattr.Value;
                                field.SetValue(result, DeserializeInternal(membername, field.FieldType, defval, ini, fullname, false, true));
                                break;
                            case MemberTypes.Property:
                                PropertyInfo property = (PropertyInfo)member;
                                if (property.GetIndexParameters().Length > 0) continue;
                                if (collectionattr && property.PropertyType.Implements(typeof(ICollection)))
                                {
                                    if (collection != null) throw new Exception("IniCollectionAttribute cannot be used on multiple members of a Type.");
                                    collection = member;
                                    continue;
                                }
                                defval = property.PropertyType.GetDefaultValue();
                                defattr = (DefaultValueAttribute)Attribute.GetCustomAttribute(member, typeof(DefaultValueAttribute), true);
                                if (defattr != null)
                                    defval = defattr.Value;
                                object propval = DeserializeInternal(membername, property.PropertyType, defval, ini, fullname, false, true);
                                MethodInfo setmethod = property.GetSetMethod();
                                if (setmethod == null) continue;
                                setmethod.Invoke(result, new object[] { propval });
                                break;
                        }
                    }
                    if (collection != null)
                        switch (collection.MemberType)
                        {
                            case MemberTypes.Field:
                                FieldInfo field = (FieldInfo)collection;
                                field.SetValue(result, DeserializeInternal(collection.Name, field.FieldType, field.FieldType.GetDefaultValue(), ini, fullname, false, false));
                                break;
                            case MemberTypes.Property:
                                PropertyInfo property = (PropertyInfo)collection;
                                object propval = DeserializeInternal(collection.Name, property.PropertyType, property.PropertyType.GetDefaultValue(), ini, fullname, false, false);
                                MethodInfo setmethod = property.GetSetMethod();
                                if (setmethod == null) break;
                                setmethod.Invoke(result, new object[] { propval });
                                break;
                        }
                    ini.Remove(rootObject ? string.Empty : name);
                    return result;
                default:
                    if (!group.ContainsKey(name)) return defaultvalue;
                    result = type.ConvertFromString(group[name]);
                    group.Remove(name);
                    return result;
            }
        }

        private static object GetDefaultValue(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Boolean:
                    return default(bool);
                case TypeCode.Byte:
                    return default(byte);
                case TypeCode.Char:
                    return default(char);
                case TypeCode.DBNull:
                    return default(DBNull);
                case TypeCode.DateTime:
                    return default(DateTime);
                case TypeCode.Decimal:
                    return default(decimal);
                case TypeCode.Double:
                    return default(double);
                case TypeCode.Empty:
                    return null;
                case TypeCode.Int16:
                    return default(short);
                case TypeCode.Int32:
                    return default(int);
                case TypeCode.Int64:
                    return default(long);
                case TypeCode.SByte:
                    return default(sbyte);
                case TypeCode.Single:
                    return default(float);
                case TypeCode.String:
                    return default(string);
                case TypeCode.UInt16:
                    return default(ushort);
                case TypeCode.UInt32:
                    return default(uint);
                case TypeCode.UInt64:
                    return default(ulong);
                default:
                    if (type.IsValueType)
                        return Activator.CreateInstance(type);
                    return null;
            }
        }

        private static bool IsComplexType(this Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Object:
                    TypeConverter converter = TypeDescriptor.GetConverter(type);
                    if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter))
                        if (converter.CanConvertTo(typeof(string)))
                            return false;
                    if (type.GetType() == typeof(Type))
                        return false;
                    return true;
                default:
                    return false;
            }
        }

        private static string ConvertToString(this object @object)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(@object);
            if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter))
                if (converter.CanConvertTo(typeof(string)))
                    return converter.ConvertToInvariantString(@object);
            if (@object is Type)
                return ((Type)@object).AssemblyQualifiedName;
            return null;
        }

        private static object ConvertFromString(this Type type, string value)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter != null && !(converter is ComponentConverter) && converter.GetType() != typeof(TypeConverter))
                if (converter.CanConvertFrom(typeof(string)))
                    return converter.ConvertFromInvariantString(value);
            if (type == typeof(Type))
                return Type.GetType(value);
            return type.GetDefaultValue();
        }

        private static bool Implements(this Type type, Type Interface)
        {
            if (type == Interface) return true;
            foreach (Type i in type.GetInterfaces())
                if (i == Interface)
                    return true;
            return false;
        }

        private static bool ImplementsGenericDefinition(this Type type, Type genericInterfaceDefinition, out Type implementingType)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType)
                {
                    Type interfaceDefinition = type.GetGenericTypeDefinition();
                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = type;
                        return true;
                    }
                }
            }
            foreach (Type i in type.GetInterfaces())
            {
                if (i.IsGenericType)
                {
                    Type interfaceDefinition = i.GetGenericTypeDefinition();

                    if (genericInterfaceDefinition == interfaceDefinition)
                    {
                        implementingType = i;
                        return true;
                    }
                }
            }
            implementingType = null;
            return false;
        }

        public static bool IsNullOrWhiteSpace(string value)
        {
            if (string.IsNullOrEmpty(value))
                return true;
            for (int i = 0; i < value.Length; i++)
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            return true;
        }
    }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    sealed class IniIgnoreAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    sealed class IniCollectionAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    sealed class IniNameAttribute : Attribute
    {
        // See the attribute guidelines at 
        //  http://go.microsoft.com/fwlink/?LinkId=85236

        public IniNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }
}