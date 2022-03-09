using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TGradientAngularNode))]
    public class TGradientAngularNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent centerPointGUI = new GUIContent("Center Point", "The point where the gradient begins");
        private static readonly GUIContent endPointGUI = new GUIContent("End Point", "The point where the gradient ends");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TGradientAngularNode n = target as TGradientAngularNode;
            n.CenterPoint = TParamGUI.Vector2Field(centerPointGUI, n.CenterPoint);
            n.EndPoint = TParamGUI.Vector2Field(endPointGUI, n.EndPoint);

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
            TPointsDragger.DrawCanvas(canvasArgument, n.CenterPoint, n.EndPoint);
        }
    }
}
