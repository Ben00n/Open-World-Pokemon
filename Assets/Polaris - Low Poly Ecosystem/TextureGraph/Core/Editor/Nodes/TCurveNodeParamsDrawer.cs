using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TCurveNode))]
    public class TCurveNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent curveRGBGUI = new GUIContent("RGB", "The curve to apply to RGB channel");
        private static readonly GUIContent curveRGUI = new GUIContent("R", "The curve to apply to R channel");
        private static readonly GUIContent curveGGUI = new GUIContent("G", "The curve to apply to G channel");
        private static readonly GUIContent curveBGUI = new GUIContent("B", "The curve to apply to B channel");
        private static readonly GUIContent curveAGUI = new GUIContent("A", "The curve to apply to A channel");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TCurveNode n = target as TCurveNode;
            n.CurveRGB = TParamGUI.CurveField(curveRGBGUI, n.CurveRGB, Color.white, new Rect(0, 0, 1, 1));
            n.CurveR = TParamGUI.CurveField(curveRGUI, n.CurveR, Color.red, new Rect(0, 0, 1, 1));
            n.CurveG = TParamGUI.CurveField(curveGGUI, n.CurveG, Color.green, new Rect(0, 0, 1, 1));
            n.CurveB = TParamGUI.CurveField(curveBGUI, n.CurveB, Color.blue, new Rect(0, 0, 1, 1));
            n.CurveA = TParamGUI.CurveField(curveAGUI, n.CurveA, Color.gray, new Rect(0, 0, 1, 1));
        }
    }
}
