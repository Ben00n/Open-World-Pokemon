using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TSolidColorNode))]
    public class TSolidColorNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent colorGUI = new GUIContent("Color", "Output color");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TSolidColorNode n = target as TSolidColorNode;
            n.Color = TParamGUI.ColorField(colorGUI, n.Color);
        }
    }
}
