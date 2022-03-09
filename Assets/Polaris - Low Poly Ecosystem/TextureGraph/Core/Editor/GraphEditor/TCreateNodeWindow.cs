using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using System.Reflection;

namespace Pinwheel.TextureGraph
{
    public class TCreateNodeWindow : ScriptableObject, ISearchWindowProvider
    {
        public TGraphEditor GraphWindow { get; internal set; }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchEntries = new List<SearchTreeEntry>();
            List<TMenuEntryGroup<Type>> flattenedMenu = TNodeCreationMenu.GetFlattenMenu();
            for (int i = 0; i < flattenedMenu.Count; ++i)
            {
                TMenuEntryGroup<Type> entry = flattenedMenu[i];
                if (entry.Data == null)
                {
                    searchEntries.Add(new SearchTreeGroupEntry(new GUIContent(entry.Text), entry.Level));
                }
                else
                {
                    Texture icon = Texture2D.blackTexture;
                    if (!string.IsNullOrEmpty(entry.Icon))
                    {
                        Texture t = Resources.Load<Texture>(entry.Icon);
                        if (t != null)
                        {
                            icon = t;
                        }
                    }
                    searchEntries.Add(new SearchTreeEntry(new GUIContent(entry.Text, icon))
                    {
                        userData = entry.Data,
                        level = entry.Level
                    });
                }
            }

            return searchEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry entry, SearchWindowContext context)
        {
            Type type = entry.userData as Type;
            if (GraphWindow != null)
            {
                GraphWindow.CreateNodeOfType(type, context.screenMousePosition);
                return true;
            }

            return false;
        }
    }
}
