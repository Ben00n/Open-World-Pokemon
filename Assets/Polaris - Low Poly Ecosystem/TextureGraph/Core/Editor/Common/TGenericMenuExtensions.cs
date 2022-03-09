using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public static class TGenericMenuExtensions
    {
        public static void AddMenuItem(this GenericMenu menu, bool enabled, string text, GenericMenu.MenuFunction callback)
        {
            if (!enabled)
            {
                menu.AddDisabledItem(new GUIContent(text));
            }
            else
            {
                menu.AddItem(new GUIContent(text), false, callback);
            }
        }
    }
}
