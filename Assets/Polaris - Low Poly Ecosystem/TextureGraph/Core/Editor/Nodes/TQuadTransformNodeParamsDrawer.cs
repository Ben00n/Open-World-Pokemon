using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TQuadTransformNode))]
    public class TQuadTransformNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent[] quadGUI = new GUIContent[4]
        {
            new GUIContent("Point 0", "Position of the bottom-left corner"),
            new GUIContent("Point 1", "Position of the top-left corner"),
            new GUIContent("Point 2", "Position of the top-right corner"),
            new GUIContent("Point 3", "Position of the bottom-right corner")
        };

        private static readonly GUIContent backgroundColorGUI = new GUIContent("Background Color", "Color of the pixels that are not covered by the quad");
        private static readonly GUIContent cullModeGUI = new GUIContent("Cull Mode", "Toggle culling mode");
        private static readonly GUIContent flipOrderGUI = new GUIContent("Flip Order", "Flip the render order of the 2 triangles");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TQuadTransformNode n = target as TQuadTransformNode;
            for (int i = 0; i < 4; ++i)
            {
                n.Quad[i] = TParamGUI.Vector2Field(quadGUI[i], n.Quad[i]);
            }
            n.BackgroundColor = TParamGUI.ColorField(backgroundColorGUI, n.BackgroundColor);
            n.CullMode.value = (UnityEngine.Rendering.CullMode)EditorGUILayout.EnumPopup(cullModeGUI, n.CullMode.value);
            n.FlipOrder = TParamGUI.Toggle(flipOrderGUI, n.FlipOrder);
            
            Texture preview = null;
            TSlot previewSlot = n.GetMainOutputSlot();
            if (previewSlot != null)
            {
                preview = Graph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
            }

            TPointsDragger.TArguments args = TPointsDragger.TArguments.Create();
            args.background = preview;
            args.drawConnectorFunction = (points) =>
            {
                Handles.DrawPolyLine(points[0], points[1], points[2], points[0]);
                Handles.DrawPolyLine(points[0], points[2], points[3], points[0]);
            };
            TPointsDragger.DrawCanvas(args, n.Quad);
        }
    }
}
