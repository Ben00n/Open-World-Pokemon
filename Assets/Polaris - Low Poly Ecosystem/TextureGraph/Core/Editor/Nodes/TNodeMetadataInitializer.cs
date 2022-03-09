using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

namespace Pinwheel.TextureGraph
{
    [InitializeOnLoad]
    public class TNodeMetadataInitializer
    {
        private static Dictionary<Type, TNodeMetadataAttribute> metadataByType;
        private static Dictionary<Type, TNodeMetadataAttribute> MetadataByType
        {
            get
            {
                if (metadataByType == null)
                {
                    metadataByType = new Dictionary<Type, TNodeMetadataAttribute>();
                }
                return metadataByType;
            }
            set
            {
                metadataByType = value;
            }
        }


        [InitializeOnLoadMethod]
        public static void Init()
        {
            List<Type> nodeTypes = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (t.IsSubclassOf(typeof(TAbstractTextureNode)))
                    {
                        nodeTypes.Add(t);
                    }
                }
            }

            MetadataByType = new Dictionary<Type, TNodeMetadataAttribute>();
            foreach (Type t in nodeTypes)
            {
                TNodeMetadataAttribute titleAttribute = t.GetCustomAttribute<TNodeMetadataAttribute>(false);
                if (titleAttribute != null)
                {
                    MetadataByType.Add(t, titleAttribute);
                }
            }
        }

        public static TNodeMetadataAttribute GetMetadata(Type t)
        {
            TNodeMetadataAttribute metadata = null;
            MetadataByType.TryGetValue(t, out metadata);
            return metadata;
        }
    }
}
