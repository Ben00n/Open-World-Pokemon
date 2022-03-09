using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor;
using System.Reflection;
using System;
using GraphicsFormat = UnityEngine.Experimental.Rendering.GraphicsFormat;

namespace Pinwheel.TextureGraph
{
    public class TNodeViewCreator
    {
        private static class TConst
        {
            public static readonly Color portColorRGBA = new Color(1f, 0.5f, 0f, 1f);
            public static readonly Color portColorGray = new Color(0.75f, 0.75f, 0.75f, 1f);
        }

        private static string GetNodeTitle(Type t)
        {
            TNodeMetadataAttribute meta = TNodeMetadataInitializer.GetMetadata(t);
            if (meta != null && !string.IsNullOrEmpty(meta.Title))
            {
                return meta.Title;
            }
            else
            {
                return ObjectNames.NicifyVariableName(t.Name);
            }
        }

        public static TNodeView Create(TAbstractTextureNode n, TGraph parentGraph)
        {
            TNodeView nodeView = new TNodeView();
            nodeView.NodeDataGuid = n.GUID;
            nodeView.Graph = parentGraph;

            nodeView.title = GetNodeTitle(n.GetType());
            nodeView.SetPosition(n.DrawState.position);

            List<TSlot> inputSlots = n.GetInputSlots();
            for (int i = 0; i < inputSlots.Count; ++i)
            {
                Port p = nodeView.InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, null);
                p.portName = null;
                TSlotReference slotRef = TSlotReference.Create(n.GUID, inputSlots[i].Id);
                p.userData = slotRef;
                p.tooltip = inputSlots[i].Name;
                SetPortCustomStyles(p, inputSlots[i]);
                nodeView.inputContainer.Add(p);
            }

            List<TSlot> outputSlots = n.GetOutputSlots();
            for (int i = 0; i < outputSlots.Count; ++i)
            {
                Port p = nodeView.InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, null);
                p.portName = null;
                TSlotReference slotRef = TSlotReference.Create(n.GUID, outputSlots[i].Id);
                p.userData = slotRef;
                p.tooltip = outputSlots[i].Name;
                nodeView.outputContainer.Add(p);
                SetPortCustomStyles(p, outputSlots[i]);
            }

            IMGUIContainer previewIMGUI = new IMGUIContainer() { name = "previewIMGUI" };
            previewIMGUI.style.width = new StyleLength(TextureGraph.TConst.NODE_PREVIEW_SIZE.x);
            previewIMGUI.style.height = new StyleLength(TextureGraph.TConst.NODE_PREVIEW_SIZE.y);
            previewIMGUI.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("TextureGraph/Textures/Checkerboard"));
            previewIMGUI.onGUIHandler = () =>
            {
                nodeView.DrawPreviewCallback();
            };
            previewIMGUI.pickingMode = PickingMode.Ignore;

            VisualElement previewContainer = new VisualElement() { name = "preview" };
            previewContainer.Add(previewIMGUI);
            nodeView.topContainer.Clear(); 
            nodeView.topContainer.Add(previewContainer);
            nodeView.topContainer.Add(nodeView.inputContainer);
            nodeView.topContainer.Add(nodeView.outputContainer);

            StyleColor inputOutputBG = new StyleColor(Color.clear);
            nodeView.inputContainer.style.backgroundColor = inputOutputBG;
            nodeView.outputContainer.style.backgroundColor = inputOutputBG;

            nodeView.userData = n.GUID;
            nodeView.RefreshPorts();
            nodeView.RefreshExpandedState();

            nodeView.Q("node-border").style.overflow = new StyleEnum<Overflow>(Overflow.Visible);
            nodeView.Q("selection-border").SendToBack();

            VisualElement metaContainer = new VisualElement() { name = "metaContainer" };
            Label metaLabel = new Label() { name = "metaLabel" };
            metaContainer.Add(metaLabel);
            nodeView.Add(metaContainer);

            StyleSheet styles = Resources.Load<StyleSheet>("TextureGraph/USS/NodeStyles");
            nodeView.styleSheets.Add(styles);

            return nodeView;
        }

        private static void SetPortCustomStyles(Port p, TSlot slot)
        {
            VisualElement connector = p.Q("connector");
            StyleLength size = new StyleLength(12f);
            connector.style.width = size;
            connector.style.height = size;

            StyleLength margin = new StyleLength(0f);
            connector.style.marginLeft = margin;
            connector.style.marginTop = margin;
            connector.style.marginRight = margin;
            connector.style.marginBottom = margin;

            StyleFloat borderWidth = new StyleFloat(2);
            connector.style.borderLeftWidth = borderWidth;
            connector.style.borderTopWidth = borderWidth;
            connector.style.borderRightWidth = borderWidth;
            connector.style.borderBottomWidth = borderWidth;

            StyleLength capSize = new StyleLength(4f);
            VisualElement cap = p.Q("cap");
            cap.style.width = capSize;
            cap.style.height = capSize;

            p.portColor = slot.DataType == TSlotDataType.RGBA ? TConst.portColorRGBA : TConst.portColorGray;
        }
    }
}
