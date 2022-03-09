using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TGaussianBlurNode))]
    public class TGaussianBlurNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent radiusGUI = new GUIContent("Radius", "Radius of the blur");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TGaussianBlurNode n = target as TGaussianBlurNode;
            n.Radius = TParamGUI.IntSlider(radiusGUI, n.Radius, 0, 100);
        }
    }
}
