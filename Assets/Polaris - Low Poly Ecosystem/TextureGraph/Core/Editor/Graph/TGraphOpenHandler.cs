using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public class TGraphOpenHandler
    {
        [OnOpenAssetAttribute(0)]
        public static bool HandleOpenGraphAsset(int instanceID, int line)
        {
            Object asset = EditorUtility.InstanceIDToObject(instanceID);
            if (asset is TGraph)
            {
                TGraph graph = asset as TGraph;
                TGraphEditor.OpenGraph(graph);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
