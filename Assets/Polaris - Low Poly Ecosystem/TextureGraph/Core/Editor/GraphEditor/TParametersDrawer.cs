using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor;
using System;
using System.Reflection;

namespace Pinwheel.TextureGraph
{
    public abstract class TParametersDrawer
    {
        public TGraph Graph { get; set; }
        public abstract void DrawGUI(TAbstractTextureNode target);
    }

    [InitializeOnLoad]
    public class TParameterDrawerInitializer
    {
        private static Dictionary<Type, Type> drawerTypeRemap;
        private static Dictionary<Type, Type> DrawerTypeRemap
        {
            get
            {
                if (drawerTypeRemap == null)
                {
                    drawerTypeRemap = new Dictionary<Type, Type>();
                }
                return drawerTypeRemap;
            }
            set
            {
                drawerTypeRemap = value;
            }
        }

        //[MenuItem("Window/Texture Graph/Init Drawer")]
        [InitializeOnLoadMethod]
        public static void InitDrawers()
        {
            DrawerTypeRemap.Clear();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (!t.IsSubclassOf(typeof(TParametersDrawer)))
                        continue;
                    TCustomParametersDrawerAttribute drawerAttribute = t.GetCustomAttribute<TCustomParametersDrawerAttribute>();
                    if (drawerAttribute == null)
                        continue;
                    DrawerTypeRemap.Add(drawerAttribute.type, t);
                }
            }
        }

        public static Type GetDrawerType(Type nodeType)
        {
            Type t = null;
            DrawerTypeRemap.TryGetValue(nodeType, out t);
            return t;
        }

        public static TParametersDrawer GetDrawer(Type nodeType)
        {
            Type drawerType = GetDrawerType(nodeType);
            if (drawerType == null)
            {
                drawerType = typeof(TDefaultParamsDrawer);
            }

            TParametersDrawer drawer = Activator.CreateInstance(drawerType) as TParametersDrawer;
            return drawer;
        }
    }
}
