using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Globalization;
using System.Reflection;

namespace Pinwheel.TextureGraph
{
    public static class TGraphSerializer
    {
        [Serializable]
        public struct TTypeInfo
        {
            [SerializeField]
            public string fullName;

            public bool IsValid
            {
                get
                {
                    return !string.IsNullOrEmpty(fullName);
                }
            }
        }

        [Serializable]
        public struct TSerializedElement : IEquatable<TSerializedElement>
        {
            [SerializeField]
            public TTypeInfo typeInfo;

            [SerializeField]
            public string jsonData;

            public bool Equals(TSerializedElement other)
            {
                return String.Compare(typeInfo.fullName, other.typeInfo.fullName) == 0 &&
                        String.Compare(jsonData, other.jsonData) == 0;
            }

            public override string ToString()
            {
                return $"{typeInfo.fullName}: {{ {jsonData} }}";
            }
        }

        public static TSerializedElement NullElement
        {
            get
            {
                return new TSerializedElement()
                {
                    typeInfo = new TTypeInfo(),
                    jsonData = null
                };
            }
        }

        public static TTypeInfo GetTypeAsSerializedData(Type type)
        {
            return new TTypeInfo
            {
                fullName = type.FullName
            };
        }

        public static Type GetTypeFromSerializedData(TTypeInfo typeInfo)
        {
            if (!typeInfo.IsValid)
                return null;

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type type = assembly.GetType(typeInfo.fullName);
                if (type != null)
                    return type;
            }

            return null;
        }

        public static TSerializedElement Serialize<T>(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("item", "Cannot serialize null item");
            }

            TTypeInfo typeInfo = GetTypeAsSerializedData(item.GetType());
            string jsonData = JsonUtility.ToJson(item);

            TSerializedElement serializedElement = new TSerializedElement()
            {
                typeInfo = typeInfo,
                jsonData = jsonData
            };
            return serializedElement;
        }

        public static T Deserialize<T>(TSerializedElement item, params object[] constructorArgs) where T : class
        {
            if (!item.typeInfo.IsValid)
            {
                throw new ArgumentException("Cannot deserialize the item, object type is invalid.");
            }

            Type type = GetTypeFromSerializedData(item.typeInfo);
            if (type == null)
            {
                throw new ArgumentException("Cannot deserialize the item, object type not found.");
            }

            T instance;
            try
            {
                CultureInfo culture = CultureInfo.CurrentCulture;
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                instance = Activator.CreateInstance(type, flags, null, constructorArgs, culture) as T;
            }
            catch
            {
                throw;
            }

            if (instance != null && !string.IsNullOrEmpty(item.jsonData))
            {
                JsonUtility.FromJsonOverwrite(item.jsonData, instance);
            }
            return instance;
        }

        public static List<TSerializedElement> Serialize<T>(IEnumerable<T> items)
        {
            List<TSerializedElement> serializedItems = new List<TSerializedElement>();
            if (items == null)
            {
                return serializedItems;
            }

            foreach (T i in items)
            {
                try
                {
                    serializedItems.Add(Serialize(i));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return serializedItems;
        }

        public static List<T> Deserialize<T>(IEnumerable<TSerializedElement> items, params object[] constructorArgs) where T : class
        {
            List<T> deserializedItems = new List<T>();
            if (items == null)
            {
                return deserializedItems;
            }

            foreach (TSerializedElement i in items)
            {
                try
                {
                    deserializedItems.Add(Deserialize<T>(i, constructorArgs));
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
            return deserializedItems;
        }
    }
}
