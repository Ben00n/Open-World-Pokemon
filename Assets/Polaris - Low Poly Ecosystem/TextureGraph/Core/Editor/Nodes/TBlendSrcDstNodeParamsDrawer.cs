using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEngine.Rendering;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TBlendSrcDstNode))]
    public class TBlendSrcDstNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent srcColorGUI = new GUIContent("Source Color", "Blend factor for foreground color");
        private static readonly GUIContent dstColorGUI = new GUIContent("Dest. Color", "Blend factor for background color");
        private static readonly GUIContent srcAlphaGUI = new GUIContent("Source Alpha", "Blend factor for foreground alpha");
        private static readonly GUIContent dstAlphaGUI = new GUIContent("Dest. Alpha", "Blend factor for background alpha");
        private static readonly GUIContent opsGUI = new GUIContent("Color Ops", "Blend operation for color");
        private static readonly GUIContent opsAlphaGUI = new GUIContent("Alpha Ops", "Blend operation for alpha");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TBlendSrcDstNode n = target as TBlendSrcDstNode;
            TEditorCommon.Header("Color");
            n.SrcColor.value = (BlendMode)EditorGUILayout.EnumPopup(srcColorGUI, n.SrcColor.value);
            n.DstColor.value = (BlendMode)EditorGUILayout.EnumPopup(dstColorGUI, n.DstColor.value);
            n.ColorOps.value = (BlendOp)EditorGUILayout.EnumPopup(opsGUI, n.ColorOps.value);

            TEditorCommon.Header("Alpha");
            n.SrcAlpha.value = (BlendMode)EditorGUILayout.EnumPopup(srcAlphaGUI, n.SrcAlpha.value);
            n.DstAlpha.value = (BlendMode)EditorGUILayout.EnumPopup(dstAlphaGUI, n.DstAlpha.value);
            n.AlphaOps.value = (BlendOp)EditorGUILayout.EnumPopup(opsAlphaGUI, n.AlphaOps.value);
        }
    }
}
