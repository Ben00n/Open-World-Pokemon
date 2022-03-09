using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TBlendNode))]
    public class TBlendNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent blendModeGUI = new GUIContent("Blend Mode", "Blend function to apply to RGB channels");
        private static readonly GUIContent alphaBlendModeGUI = new GUIContent("Alpha Blend Mode", "Blend function to apply to A channel");
        private static readonly GUIContent opacityGUI = new GUIContent("Opacity", "Strength of the blend operation. This will be used to interpolate between the background and the blend result.");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TBlendNode n = target as TBlendNode;
            n.ColorBlendMode.value = (TBlendNode.TBlendMode)EditorGUILayout.EnumPopup(blendModeGUI, n.ColorBlendMode.value);
            n.AlphaBlendMode.value = (TBlendNode.TAlphaBlendMode)EditorGUILayout.EnumPopup(alphaBlendModeGUI, n.AlphaBlendMode.value);
            n.Opacity = TParamGUI.FloatSlider(opacityGUI, n.Opacity, 0f, 1f);
        }
    }
}
