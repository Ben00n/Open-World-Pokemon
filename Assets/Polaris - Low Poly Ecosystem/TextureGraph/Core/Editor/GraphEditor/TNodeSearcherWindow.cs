#if TG_SEARCHER
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Searcher;
using UnityEngine.UIElements;
using UnityEditor;
using System;

namespace Pinwheel.TextureGraph
{
    public class TNodeSearcherWindow : ScriptableObject
    {
        public class TNodeSearcherItem : SearcherItem
        {
            public Type Type;

            public TNodeSearcherItem(string name, string help = "", List<SearcherItem> children = null) : base(name, help, children)
            {
            }
        }

        public EditorWindow Host { get; set; }

        public List<SearcherItem> GetItems()
        {
            TMenuTree<Type> trees = TNodeCreationMenu.MenuTree;
            Stack<TMenuEntryGroup<Type>> s0 = new Stack<TMenuEntryGroup<Type>>();
            for (int i = 0; i < trees.RootEntry.SubEntries.Count; ++i)
            {
                s0.Push(trees.RootEntry.SubEntries[i]);
            }

            List<SearcherItem> items = new List<SearcherItem>();
            Stack<SearcherItem> s1 = new Stack<SearcherItem>();
            for (int i = 0; i < trees.RootEntry.SubEntries.Count; ++i)
            {
                TMenuEntryGroup<Type> entry = trees.RootEntry.SubEntries[i];
                TNodeSearcherItem childItems = new TNodeSearcherItem(entry.Text);
                childItems.Type = entry.Data;
                items.Add(childItems);
                s1.Push(childItems);
            }

            while (s0.Count > 0)
            {
                TMenuEntryGroup<Type> treeEntry = s0.Pop();
                SearcherItem item = s1.Pop();

                for (int i = 0; i < treeEntry.SubEntries.Count; ++i)
                {
                    TNodeSearcherItem childItem = new TNodeSearcherItem(treeEntry.SubEntries[i].Text);
                    childItem.Type = treeEntry.SubEntries[i].Data;
                    item.AddChild(childItem);
                    s0.Push(treeEntry.SubEntries[i]);
                    s1.Push(childItem);
                }
            }

            return items;
        }
    }
}
#endif