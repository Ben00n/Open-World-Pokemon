using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TLoadTextureNode))]
    public class TLoadTextureNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent pathGUI = new GUIContent("Path", "Path to the texture");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TLoadTextureNode node = target as TLoadTextureNode;

            EditorGUILayout.LabelField(pathGUI, new GUIContent(node.FilePath.value, node.FilePath.value));
            TEditorCommon.Separator();
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(TEditorCommon.objectSelectorDragDropHeight));
            Texture tex = TEditorCommon.ObjectSelectorDragDrop<Texture>(r, "Select a Texture", "t:Texture", false);
            if (tex != null && AssetDatabase.Contains(tex))
            {
                string relativePath = AssetDatabase.GetAssetPath(tex);
                node.SetPath(relativePath);
                GUI.changed = true;
            }
        }
    }
}
