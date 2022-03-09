using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TBlurNode))]
    public class TBlurNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent radiusGUI = new GUIContent("Radius", "Radius of the blur");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TBlurNode n = target as TBlurNode;
            n.Radius = TParamGUI.IntSlider(radiusGUI, n.Radius, 0, 100);
        }
    }
}
