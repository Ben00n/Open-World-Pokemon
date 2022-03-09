using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TRidgedNoiseNode))]
    public class TRidgedNoiseNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "Scale/Density of the noise map");
        private static readonly GUIContent seedGUI = new GUIContent("Seed", "Value to randomize the noise");
        private static readonly GUIContent variantGUI = new GUIContent("Variant", "Create a slightly different noise");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TRidgedNoiseNode n = target as TRidgedNoiseNode;
            n.Scale = TParamGUI.IntField(scaleGUI, n.Scale);
            n.Seed = TParamGUI.IntField(seedGUI, n.Seed);
            n.Variant = TParamGUI.FloatSlider(variantGUI, n.Variant, 0f, 1f);
        }
    }
}
