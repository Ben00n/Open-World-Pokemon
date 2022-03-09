using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TClampNode))]
    public class TClampNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent minGUI = new GUIContent("Min", "Minimum value");
        private static readonly GUIContent maxGUI = new GUIContent("Max", "Maximum value");
        private static readonly GUIContent applyAlphaGUI = new GUIContent("Apply To Alpha", "Should it apply to the A channel?");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TClampNode n = target as TClampNode;
            n.Min = TParamGUI.FloatSlider(minGUI, n.Min, 0f, 1f);
            n.Max = TParamGUI.FloatSlider(maxGUI, n.Max, 0f, 1f);
            n.ApplyToAlpha = TParamGUI.Toggle(applyAlphaGUI, n.ApplyToAlpha);
        }
    }
}
