using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TPowNode))]
    public class TPowNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent powerGUI = new GUIContent("Power", "Power value");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TPowNode n = target as TPowNode;
            n.Power = EditorGUILayout.FloatField(powerGUI, n.Power);
        }
    }
}
