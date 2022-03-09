using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TShapeNode))]
    public class TShapeNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent shapeGUI = new GUIContent("Shape", "Shape type to generate");
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "Scale the whole shape");
        private static readonly GUIContent innerSizeGUI = new GUIContent("Inner Size", "Size of the inner part of some shape");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TShapeNode n = target as TShapeNode;
            n.Shape.value = (TShapeNode.TShape)EditorGUILayout.EnumPopup(shapeGUI, n.Shape.value);
            n.Scale = TParamGUI.Vector2Slider(scaleGUI, n.Scale, 0f, 1f);
            if (n.Shape.value == TShapeNode.TShape.Torus ||
                n.Shape.value == TShapeNode.TShape.Brick)
            {
                n.InnerSize = TParamGUI.FloatSlider(innerSizeGUI, n.InnerSize, 0f, 1f);
            }
        }
    }
}
