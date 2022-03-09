using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TContrastBrightnessNode))]
    public class TContrastBrightnessNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent contrastGUI = new GUIContent("Contrast", "Adjust the distinction of the image color");
        private static readonly GUIContent brightnessGUI = new GUIContent("Brightness", "Adjust the image light level");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TContrastBrightnessNode n = target as TContrastBrightnessNode;
            n.Contrast = TParamGUI.FloatSlider(contrastGUI, n.Contrast, -1, 1);
            n.Brightness = TParamGUI.FloatSlider(brightnessGUI, n.Brightness, -1, 1);
        }
    }
}
