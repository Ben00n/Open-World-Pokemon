using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TThresholdNode))]
    public class TThresholdNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent lowGUI = new GUIContent("Low", "Minimum value where it will turn full black");
        private static readonly GUIContent highGUI = new GUIContent("High", "Maximum value where it will turn full white");
        private static readonly GUIContent modeGUI = new GUIContent("Mode", "Comparison mode");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TThresholdNode n = target as TThresholdNode;
            n.ThresholdLow = TParamGUI.FloatSlider(lowGUI, n.ThresholdLow, 0f, 1f);
            n.ThresholdHigh = TParamGUI.FloatSlider(highGUI, n.ThresholdHigh, 0f, 1f);
            n.Mode.value = (TThresholdNode.TCompareMode)EditorGUILayout.EnumPopup(modeGUI, n.Mode.value);
        }
    }
}
