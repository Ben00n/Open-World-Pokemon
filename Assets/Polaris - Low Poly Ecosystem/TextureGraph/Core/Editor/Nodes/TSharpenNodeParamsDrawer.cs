using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TSharpenNode))]
    public class TSharpenNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent intensityGUI = new GUIContent("Intensity", "Intensity of the effect");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TSharpenNode n = target as TSharpenNode;
            n.Intensity = TParamGUI.FloatField(intensityGUI, n.Intensity);
        }
    }
}
