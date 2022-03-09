using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TGradientMapDynamicNode))]
    public class TGradientMapDynamicNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent axisGUI = new GUIContent("Axis", "The axis to sample the gradient map");
        private static readonly GUIContent sliceGUI = new GUIContent("Slice", "Position of the gradient line/column in UV space");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TGradientMapDynamicNode n = target as TGradientMapDynamicNode;
            n.Axis.value = (TAxis)EditorGUILayout.EnumPopup(axisGUI, n.Axis.value);
            n.Slice = TParamGUI.FloatSlider(sliceGUI, n.Slice, 0f, 1f);
        }
    }
}
