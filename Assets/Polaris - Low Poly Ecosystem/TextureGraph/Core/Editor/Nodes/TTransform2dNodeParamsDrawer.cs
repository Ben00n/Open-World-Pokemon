using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TTransform2dNode))]
    public class TTransform2dNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent tilingModeGUI = new GUIContent("Tiling Mode", "How to tile the image on X and Y axis");
        private static readonly GUIContent offsetGUI = new GUIContent("Offset", "Offset the image from its origin (percentage)");
        private static readonly GUIContent rotationGUI = new GUIContent("Rotation", "Rotate the image (degree)");
        private static readonly GUIContent scaleGUI = new GUIContent("Scale", "Scale the image (percentage)");
        private static readonly GUIContent backgroundColorGUI = new GUIContent("Background Color", "Color to replace the pixel if its position get pass the boundaries, only if Tiling is not enabled on an axis.");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TTransform2dNode n = target as TTransform2dNode;
            n.TilingMode.value = (TTilingMode)EditorGUILayout.EnumPopup(tilingModeGUI, n.TilingMode.value);
            n.Offset = TParamGUI.Vector2Field(offsetGUI, n.Offset);
            n.Rotation = TParamGUI.FloatField(rotationGUI, n.Rotation);
            n.Scale = TParamGUI.Vector2Field(scaleGUI, n.Scale);
            if (n.TilingMode.value != TTilingMode.TileXY)
            {
                n.BackgroundColor = TParamGUI.ColorField(backgroundColorGUI, n.BackgroundColor);
            }
        }
    }
}
