using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TCheckerboardNode))]
    public class TCheckerboardNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "Scale/Density of the pattern");
        private static readonly GUIContent color0GUI = new GUIContent("Color 0", "Color of the pattern");
        private static readonly GUIContent color1GUI = new GUIContent("Color 1", "Color of the pattern");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TCheckerboardNode n = target as TCheckerboardNode;
            n.Scale = TParamGUI.IntField(scaleGUI, n.Scale);
            n.Color0 = TParamGUI.ColorField(color0GUI, n.Color0);
            n.Color1 = TParamGUI.ColorField(color1GUI, n.Color1);
        }
    }
}
