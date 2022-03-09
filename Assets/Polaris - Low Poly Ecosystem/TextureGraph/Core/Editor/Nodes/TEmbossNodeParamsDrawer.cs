using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TEmbossNode))]
    public class TEmbossNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent lightAngleGUI = new GUIContent("Light Angle", "Angle of the light source (degree)");
        private static readonly GUIContent intensityGUI = new GUIContent("Intensity", "Intensity of the effect");
        private static readonly GUIContent highlightColorGUI = new GUIContent("Highlight Color", "Tint color of the highlighted part");
        private static readonly GUIContent shadowColorGUI = new GUIContent("Shadow Color", "Tint color of the shadowed part");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TEmbossNode n = target as TEmbossNode;
            n.LightAngle = TParamGUI.FloatSlider(lightAngleGUI, n.LightAngle, 0f, 360f);
            n.Intensity = TParamGUI.FloatSlider(intensityGUI, n.Intensity, 0f, 1f);
            n.HighlightColor = TParamGUI.ColorField(highlightColorGUI, n.HighlightColor);
            n.ShadowColor = TParamGUI.ColorField(shadowColorGUI, n.ShadowColor);
        }
    }
}
