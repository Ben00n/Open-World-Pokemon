using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using TSerializedElement = Pinwheel.TextureGraph.TGraphSerializer.TSerializedElement;

namespace Pinwheel.TextureGraph
{
    public class TDebugWindow : EditorWindow
    {
        //[MenuItem("Window/Texture Graph/Internal/Debug")]
        public static void ShowWindow()
        {
            TDebugWindow window = GetWindow<TDebugWindow>();
            window.titleContent = new GUIContent("TDebug");            
            window.Show();
        }
        
        public void OnGUI()
        {
            List<TSerializedElement> nodes = new List<TSerializedElement>();
            List<TSerializedElement> edges = new List<TSerializedElement>();
            TClipboard.GetData(nodes, edges);

            TEditorCommon.Header($"NODES: {nodes.Count}");
            foreach(var d in nodes)
            {
                EditorGUILayout.LabelField(d.ToString(), TEditorCommon.WordWrapLeftLabel);
                TEditorCommon.Separator();
            }
            TEditorCommon.Header($"EDGES: {edges.Count}");
            foreach (var d in edges)
            {
                EditorGUILayout.LabelField(d.ToString(), TEditorCommon.WordWrapLeftLabel);
                TEditorCommon.Separator();
            }
        }
    }
}
