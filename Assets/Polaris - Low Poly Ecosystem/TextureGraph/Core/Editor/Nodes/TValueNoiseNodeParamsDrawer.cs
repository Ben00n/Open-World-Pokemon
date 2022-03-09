using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TValueNoiseNode))]
    public class TValueNoiseNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "Scale/Density of the noise map");
        private static readonly GUIContent seedGUI = new GUIContent("Seed", "Value to randomize the noise");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TValueNoiseNode n = target as TValueNoiseNode;
            n.Scale = TParamGUI.IntField(scaleGUI, n.Scale);
            n.Seed = TParamGUI.IntField(seedGUI, n.Seed);
        }
    }
}
