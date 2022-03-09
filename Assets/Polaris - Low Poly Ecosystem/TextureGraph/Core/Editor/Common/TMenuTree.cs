using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    public class TMenuTree<T>
    {
        public TMenuEntryGroup<T> RootEntry { get; set; }

        public TMenuTree(string rootIcon, string rootText, T rootData)
        {
            RootEntry = new TMenuEntryGroup<T>();
            RootEntry.Level = 0;
            RootEntry.Icon = rootIcon;
            RootEntry.Path = rootText;
            RootEntry.Text = rootText;
            RootEntry.Data = rootData;
        }

        public void AddEntry(string icon, string menuPath, T data)
        {
            string[] paths = menuPath.Split(new string[] { "/" }, System.StringSplitOptions.RemoveEmptyEntries);
            TMenuEntryGroup<T> parentEntry = RootEntry;

            for (int i = 0; i < paths.Length; ++i)
            {
                TMenuEntryGroup<T> currentEntry = parentEntry.SubEntries.Find(e => string.Compare(paths[i], e.Text) == 0);
                if (currentEntry != null)
                {
                    parentEntry = currentEntry;
                }
                else
                {
                    currentEntry = new TMenuEntryGroup<T>();
                    currentEntry.Level = i + 1;
                    currentEntry.Text = paths[i];

                    parentEntry.SubEntries.Add(currentEntry);

                    parentEntry = currentEntry;
                }

                if (i == paths.Length - 1)
                {
                    currentEntry.Icon = icon;
                    currentEntry.Data = data;
                    currentEntry.Path = menuPath;
                }
            }
        }

        public List<TMenuEntryGroup<T>> ToList()
        {
            List<TMenuEntryGroup<T>> list = new List<TMenuEntryGroup<T>>();
            Stack<TMenuEntryGroup<T>> entryStack = new Stack<TMenuEntryGroup<T>>();
            entryStack.Push(RootEntry);
            while (entryStack.Count > 0)
            {
                TMenuEntryGroup<T> entry = entryStack.Pop();
                List<TMenuEntryGroup<T>> subEntries = entry.SubEntries;
                for (int i = subEntries.Count - 1; i >= 0; --i)
                {
                    entryStack.Push(subEntries[i]);
                }
                list.Add(entry);
            }
            return list;
        }
    }

    public class TMenuEntryGroup<T>
    {
        public int Level { get; internal set; }
        public string Icon { get; set; }
        public string Text { get; set; }
        public string Path { get; set; }
        public T Data { get; set; }

        private List<TMenuEntryGroup<T>> subEntries;
        public List<TMenuEntryGroup<T>> SubEntries
        {
            get
            {
                if (subEntries == null)
                {
                    subEntries = new List<TMenuEntryGroup<T>>();
                }
                return subEntries;
            }
            set
            {
                subEntries = value;
            }
        }
    }
}
