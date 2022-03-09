using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TNormalNode))]
    public class TNormalNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent strengthGUI = new GUIContent("Strength", "Strength of the normal map");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TNormalNode n = target as TNormalNode;
            n.Strength = TParamGUI.FloatField(strengthGUI, n.Strength);
        }
    }
}
