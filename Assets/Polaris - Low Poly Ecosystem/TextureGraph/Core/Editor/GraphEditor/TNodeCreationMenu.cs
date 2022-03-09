using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.Reflection;

namespace Pinwheel.TextureGraph
{
    [InitializeOnLoad]
    public class TNodeCreationMenu
    {
        private static TMenuTree<Type> menuTree;
        public static TMenuTree<Type> MenuTree
        {
            get
            {
                if (menuTree == null)
                {
                    menuTree = new TMenuTree<Type>("", "Node", null);
                }
                return menuTree;
            }
            private set
            {
                menuTree = value;
            }
        }

        [InitializeOnLoadMethod]
        //[MenuItem("Window/Texture Graph/Init Node Creation Menu")]
        public static void InitNodeMenus()
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

            MenuTree = new TMenuTree<Type>("", "Node", null);
            foreach (Type t in nodeTypes)
            {
                TNodeMetadataAttribute meta = t.GetCustomAttribute<TNodeMetadataAttribute>(false);
                if (meta != null && !string.IsNullOrEmpty(meta.CreationMenu))
                {
                    MenuTree.AddEntry(meta.Icon, meta.CreationMenu, t);
                }
            }
        }

        public static List<TMenuEntryGroup<Type>> GetFlattenMenu()
        {
            return MenuTree.ToList();
        }
    }
}
