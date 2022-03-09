using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public static class TViewManager
    {
        public const string KEY_VISIBLE = "visible";

        public static bool IsViewVisible(string key)
        {
            return EditorPrefs.GetBool(key + KEY_VISIBLE, false);
        }

        public static void ToggleViewVisibility(ITManagedView view, string key)
        {
            if (IsViewVisible(key))
            {
                HideView(view, key);
            }
            else
            {
                ShowView(view, key);
            }
        }

        public static void ShowView(ITManagedView view, string key)
        {
            view.Show();
            EditorPrefs.SetBool(key + KEY_VISIBLE, true);
        }

        public static void HideView(ITManagedView view, string key)
        {
            view.Hide();
            EditorPrefs.SetBool(key + KEY_VISIBLE, false);
        }
    }
}
