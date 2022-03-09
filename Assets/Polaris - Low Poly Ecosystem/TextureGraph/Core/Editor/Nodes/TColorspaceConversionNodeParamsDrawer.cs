using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TColorspaceConversionNode))]
    public class TColorspaceConversionNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent conversionModeGUI = new GUIContent("Mode", "Colorspace conversion mode");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TColorspaceConversionNode n = target as TColorspaceConversionNode;
            n.ConversionMode.value = (TColorspaceConversionNode.TConversionMode)EditorGUILayout.EnumPopup(conversionModeGUI, n.ConversionMode.value);
        }
    }
}
