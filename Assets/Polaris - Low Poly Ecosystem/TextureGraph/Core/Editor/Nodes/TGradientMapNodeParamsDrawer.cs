using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TGradientMapNode))]
    public class TGradientMapNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent gradientGUI = new GUIContent("Gradient", "The gradient to apply");
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "How many times to repeat the gradient");
        private static readonly GUIContent wrapModeGUI = new GUIContent("Wrap Mode", "How to repeat the gradient");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TGradientMapNode n = target as TGradientMapNode;
            n.Gradient = TParamGUI.GradientField(gradientGUI, n.Gradient,true);
            n.Scale = TParamGUI.FloatField(scaleGUI, n.Scale);
            n.WrapMode.value = (TextureWrapMode)EditorGUILayout.EnumPopup(wrapModeGUI, n.WrapMode.value);
        }
    }
}
