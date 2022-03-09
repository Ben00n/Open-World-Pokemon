using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TWarpNode))]
    public class TWarpNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent intensityGUI = new GUIContent("Intensity", "Intensity of the effect");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TWarpNode n = target as TWarpNode;
            n.Intensity = TParamGUI.FloatSlider(intensityGUI, n.Intensity, 0f, 1f);
        }
    }
}
