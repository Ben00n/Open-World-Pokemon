using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TGradientAxialNode))]
    public class TGradientAxialNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent startPointGUI = new GUIContent("Start Point", "The point where the gradient begins");
        private static readonly GUIContent endPointGUI = new GUIContent("End Point", "The point where the gradient ends");
        private static readonly GUIContent reflectedGUI = new GUIContent("Reflected", "Make the gradient reflected at the center of the line");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TGradientAxialNode n = target as TGradientAxialNode;
            n.StartPoint = TParamGUI.Vector2Field(startPointGUI, n.StartPoint);
            n.EndPoint = TParamGUI.Vector2Field(endPointGUI, n.EndPoint);
            n.Reflected = TParamGUI.Toggle(reflectedGUI, n.Reflected);

            Texture preview = null;
            TSlot previewSlot = n.GetMainOutputSlot();
            if (previewSlot != null)
            {
                preview = Graph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
            }
            TPointsDragger.TArguments canvasArgument = TPointsDragger.TArguments.Create();
            canvasArgument.background = preview;
            canvasArgument.backgroundMaterial = TEditorCommon.PreviewRedToGrayMaterial;
            canvasArgument.drawConnectorFunction = (points) => { Handles.DrawLine(points[0], points[1]); };
            TPointsDragger.DrawCanvas(canvasArgument, n.StartPoint, n.EndPoint);
        }
    }
}
