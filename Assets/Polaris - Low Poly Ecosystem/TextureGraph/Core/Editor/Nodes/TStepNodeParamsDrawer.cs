using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TStepNode))]
    public class TStepNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent stepGUI = new GUIContent("Step", "Step count");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TStepNode n = target as TStepNode;
            n.Step = TParamGUI.IntField(stepGUI, n.Step);
        }
    }
}
