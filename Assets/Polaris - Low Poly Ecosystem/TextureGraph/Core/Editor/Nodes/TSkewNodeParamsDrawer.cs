using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TSkewNode))]
    public class TSkewNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent axisGUI = new GUIContent("Axis", "The skew axis");
        private static readonly GUIContent skewAmountGUI = new GUIContent("Amount", "The skew amount");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TSkewNode n = target as TSkewNode;
            n.SkewAxis.value = (TAxis)EditorGUILayout.EnumPopup(axisGUI, n.SkewAxis.value);
            n.SkewAmount = TParamGUI.FloatSlider(skewAmountGUI, n.SkewAmount, -1f, 1f);
        }
    }
}
