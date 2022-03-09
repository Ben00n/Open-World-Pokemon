using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(THslNode))]
    public class THslNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent hueGUI = new GUIContent("Hue", "Shift the image color");
        private static readonly GUIContent saturationGUI = new GUIContent("Saturation", "Adjust color intensity");
        private static readonly GUIContent lightnessGUI = new GUIContent("Lightness", "Adjust color lightness");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            THslNode n = target as THslNode;
            n.Hue = TParamGUI.IntSlider(hueGUI, n.Hue, -180, 180);
            n.Saturation = TParamGUI.IntSlider(saturationGUI, n.Saturation, -100, 100);
            n.Lightness = TParamGUI.IntSlider(lightnessGUI, n.Lightness, -100, 100);
        }
    }
}
