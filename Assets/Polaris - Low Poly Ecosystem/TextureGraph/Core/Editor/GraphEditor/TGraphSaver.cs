using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public static class TGraphSaver
    {
        public static void Save(TGraph clonedGraph, TGraph sourceGraph)
        {
            clonedGraph.CopySerializeDataTo(sourceGraph);
            EditorUtility.SetDirty(sourceGraph);
            AssetDatabase.SaveAssets();
            TGraphEditor.SetGraphDirty(sourceGraph, false);
        }

    }
}
