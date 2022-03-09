using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TOutputToRTNode))]
    public class TOutputToRTNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent pathGUI = new GUIContent("Path", "Path to the Custom Render Texture");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TOutputToRTNode node = target as TOutputToRTNode;

            EditorGUILayout.LabelField(pathGUI, new GUIContent(node.FilePath.value, node.FilePath.value));
            TEditorCommon.Separator();
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(TEditorCommon.objectSelectorDragDropHeight));
            CustomRenderTexture tex = TEditorCommon.ObjectSelectorDragDrop<CustomRenderTexture>(r, "Select a Texture", "t:CustomRenderTexture", false);
            if (tex != null && AssetDatabase.Contains(tex))
            {
                string relativePath = AssetDatabase.GetAssetPath(tex);
                node.SetPath(relativePath);
                GUI.changed = true;
            }
        }
    }
}
