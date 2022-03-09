using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Pinwheel.TextureGraph
{
    [CustomEditor(typeof(TGraph))]
    public class TGraphInspector : Editor
    {
        private TGraph instance;
        private void OnEnable()
        {
            instance = target as TGraph;
        }

        public override VisualElement CreateInspectorGUI()
        {
            VisualElement ve = new VisualElement();
            Button openEditorButton = new Button()
            {
                text = "Open Editor"
            };
            openEditorButton.clicked += OpenEditorButton_clicked;
            ve.Add(openEditorButton);

            IMGUIContainer imgui = new IMGUIContainer(() =>
            {
                List<TAbstractTextureNode> nodes = instance.GraphData.Nodes;
                EditorGUILayout.LabelField("NODES: " + nodes.Count);
                for (int i = 0; i < nodes.Count; ++i)
                {
                    TAbstractTextureNode n = nodes[i];
                    EditorGUILayout.LabelField(n.GetType().Name);
                    List<TSlot> outputSlot = n.GetOutputSlots();
                    foreach (TSlot s in outputSlot)
                    {
                        Texture t = instance.GetRT(TSlotReference.Create(n.GUID, s.Id));
                        if (t != null)
                        {
                            EditorGUILayout.ObjectField(t.GetInstanceID().ToString(), t, typeof(Texture), false);
                        }
                    }
                    EditorGUILayout.Space();
                }

                EditorGUILayout.Space();
                List<ITEdge> edges = instance.GraphData.Edges;
                EditorGUILayout.LabelField("EDGES: " + edges.Count);
                float width = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = 500;
                for (int i = 0; i < edges.Count; ++i)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(edges[i].OutputSlot.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                    EditorGUILayout.LabelField(edges[i].InputSlot.ToString(), GUILayout.Height(EditorGUIUtility.singleLineHeight * 2));
                    EditorGUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
                EditorGUILayout.LabelField("RT POOL");
                foreach (var rt in instance.RtPool.Values)
                {
                    if (rt == null)
                    {
                        EditorGUILayout.LabelField("Null");
                    }
                    else
                    {
                        EditorGUILayout.LabelField(rt.name, TEditorCommon.WordWrapLeftLabel);
                    }
                }

                EditorGUIUtility.labelWidth = width;
            });
            ve.Add(imgui);

            return ve;
        }

        private void OpenEditorButton_clicked()
        {
            TGraphEditor.OpenGraph(instance);
        }
    }
}
