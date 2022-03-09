using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TGradientLinearNode))]
    public class TGradientLinearNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent rotationGUI = new GUIContent("Rotation", "Pre-defined rotation for the gradient");
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "How many times the gradient should repeat");
        private static readonly GUIContent midPointGUI = new GUIContent("Mid Point", "The point in each strip where the gradient reflected");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TGradientLinearNode n = target as TGradientLinearNode;
            n.Rotation.value = (TGradientLinearNode.TRotation)EditorGUILayout.EnumPopup(rotationGUI, n.Rotation.value);
            n.Scale = TParamGUI.IntField(scaleGUI, n.Scale);
            n.MidPoint = TParamGUI.FloatSlider(midPointGUI, n.MidPoint, 0f, 1f);
        }
    }
}
