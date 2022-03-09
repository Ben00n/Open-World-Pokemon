using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace Pinwheel.TextureGraph
{
    public class TGraphView : GraphView
    {
        public delegate void NodeToInspectChangedHandler(TGraphView graphView, Node node);
        public static event NodeToInspectChangedHandler NodeToInspectChanged;

        public delegate void NodeDoubleClickedHandler(TGraphView graphView, Node node);
        public static event NodeDoubleClickedHandler NodeDoubleClicked;

        public static readonly Color32 backgroundColor = new Color32(32, 32, 32, 255);

        public TGraphEditor GraphEditor { get; set; }
        public Vector2 LocalMousePosition { get; set; }

        private Node nodeToInspect;

        public TGraphView()
        {
            SetupZoom(0.2f, 10f, ContentZoomer.DefaultScaleStep, ContentZoomer.DefaultReferenceScale);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

            this.style.backgroundColor = new StyleColor(backgroundColor);
        }

        public List<GraphElement> CreateViews(TGraph graph)
        {
            int[] nodeIndices = TUtilities.GetIndicesArray(graph.GraphData.Nodes.Count);
            int[] edgeIndices = TUtilities.GetIndicesArray(graph.GraphData.Edges.Count);
            return CreateViews(graph, nodeIndices, edgeIndices);
        }

        public List<GraphElement> CreateViews(TGraph graph, IEnumerable<int> nodeIndices, IEnumerable<int> edgeIndices)
        {
            graph.GraphData.Validate();
            List<GraphElement> createdViews = new List<GraphElement>();
            List<Guid> nodeGuids = new List<Guid>();

            foreach (int i in nodeIndices)
            {
                TAbstractTextureNode n = graph.GraphData.Nodes[i];
                Node nv = TNodeViewCreator.Create(n, graph);
                AddElement(nv);
                createdViews.Add(nv);
                nodeGuids.Add(n.GUID);
            }

            Dictionary<TSlotReference, Port> slotRefToPortRemap = new Dictionary<TSlotReference, Port>();
            ports.ForEach(p =>
            {
                TSlotReference slotRef = (TSlotReference)p.userData;
                slotRefToPortRemap[slotRef] = p;
            });

            foreach (int i in edgeIndices)
            {
                ITEdge e = graph.GraphData.Edges[i];
                Port outPort = slotRefToPortRemap[e.OutputSlot];
                Port inPort = slotRefToPortRemap[e.InputSlot];
                Edge ev = outPort.ConnectTo(inPort);
                ev.userData = e.GUID;
                AddElement(ev);
                createdViews.Add(ev);
            }

            graph.ExecuteAt(nodeGuids);

            return createdViews;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();
            ports.ForEach(p =>
            {
                if (p != startPort && p.node != startPort.node && p.direction != startPort.direction)
                    compatiblePorts.Add(p);
            });
            return compatiblePorts;
        }

        public override void HandleEvent(EventBase evt)
        {
            base.HandleEvent(evt);
            if (evt is MouseDownEvent || evt is MouseUpEvent)
            {
                HandleNodeToInspect();
            }
            if (evt is MouseDownEvent mde)
            {
                if (mde.clickCount >= 2)
                {
                    HandleNodeDoubleClick();
                }
            }
            if (evt is MouseMoveEvent mme)
            {
                LocalMousePosition = mme.localMousePosition;
            }
        }

        public void HandleNodeToInspect()
        {
            Node n = null;
            if (selection != null)
            {
                if (selection.Count == 1)
                {
                    if (selection[0] is Node selectedNode)
                    {
                        n = selectedNode;
                    }
                }
            }
            if (n != nodeToInspect)
            {
                nodeToInspect = n;
                if (NodeToInspectChanged != null)
                {
                    NodeToInspectChanged.Invoke(this, nodeToInspect);
                }
            }
        }

        private void HandleNodeDoubleClick()
        {
            if (nodeToInspect != null)
            {
                if (NodeDoubleClicked != null)
                {
                    NodeDoubleClicked.Invoke(this, nodeToInspect);
                }
            }
        }

        public TNodeView FindNode(Guid guid)
        {
            TNodeView node = null;
            nodes.ForEach(nv =>
            {
                Guid nodeGuid = (Guid)nv.userData;
                if (nodeGuid.Equals(guid))
                {
                    node = nv as TNodeView;
                    return;
                }
            });
            return node;
        }

        public Edge FindEdge(Guid guid)
        {
            Edge edge = null;
            edges.ForEach(ev =>
            {
                Guid edgeGuid = (Guid)ev.userData;
                if (edgeGuid.Equals(guid))
                {
                    edge = ev;
                    return;
                }
            });
            return edge;
        }

        public void UpdateViewOnUndoRedo(TGraph graph)
        {
            //Remove views that binding data doesn't exist
            List<GraphElement> elementToRemove = new List<GraphElement>();
            nodes.ForEach(nv =>
            {
                Guid nodeGuid = (Guid)nv.userData;
                TAbstractTextureNode n = graph.GraphData.GetNodeByGUID(nodeGuid);
                if (n == null)
                {
                    elementToRemove.Add(nv);
                }
            });
            edges.ForEach(ev =>
            {
                Guid edgeGuid = (Guid)ev.userData;
                ITEdge e = graph.GraphData.GetEdgeByGUID(edgeGuid);
                if (e == null)
                {
                    elementToRemove.Add(ev);
                }
            });
            DeleteElements(elementToRemove);

            //Create views for data that doesn't have view
            List<TAbstractTextureNode> nodeData = graph.GraphData.Nodes;
            List<ITEdge> edgeData = graph.GraphData.Edges;
            List<int> nodeIndices = new List<int>();
            List<int> edgeIndices = new List<int>();

            for (int i = 0; i < nodeData.Count; ++i)
            {
                Node nv = FindNode(nodeData[i].GUID);
                if (nv == null)
                {
                    nodeIndices.Add(i);
                }
            }
            for (int i = 0; i < edgeData.Count; ++i)
            {
                Edge ev = FindEdge(edgeData[i].GUID);
                if (ev == null)
                {
                    edgeIndices.Add(i);
                }
            }

            if (nodeIndices.Count > 0 || edgeIndices.Count > 0)
            {
                CreateViews(graph, nodeIndices, edgeIndices);
            }

            //Update elements position
            nodes.ForEach(nv =>
            {
                Guid nodeGuid = (Guid)nv.userData;
                TAbstractTextureNode n = graph.GraphData.GetNodeByGUID(nodeGuid);
                if (n != null)
                {
                    nv.SetPosition(n.DrawState.position);
                }
            });

            ClearSelection();
            HandleNodeToInspect();
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            if (evt.target is TGraphView && nodeCreationRequest != null)
            {
                evt.menu.AppendAction(
                    "Create Node",
                    (a) =>
                    {
                        Vector2 mousePos = a.eventInfo.localMousePosition + GraphEditor.position.position;
                        NodeCreationContext context = new NodeCreationContext() { screenMousePosition = mousePos };
                        nodeCreationRequest.Invoke(context);

                    });
            }
            evt.menu.AppendSeparator();

            if (evt.target is TGraphView || evt.target is TNodeView)
            {
                evt.menu.AppendAction(
                    "Cut",
                    (a) =>
                    {
                        GraphEditor.HandleCut();
                    },
                    GraphEditor.HasSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction(
                    "Copy",
                    (a) =>
                    {
                        GraphEditor.HandleCopy();
                    },
                    GraphEditor.HasSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendAction(
                     "Paste",
                     (a) =>
                     {
                         GraphEditor.HandlePaste();
                     },
                     GraphEditor.CanPaste() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
                evt.menu.AppendSeparator(null);
                evt.menu.AppendAction(
                    "Duplicate",
                    (a) =>
                    {
                        GraphEditor.HandleCopy();
                        GraphEditor.HandlePaste();
                        TClipboard.Clear();
                    },
                    GraphEditor.HasSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is TGraphView || evt.target is TNodeView || evt.target is Edge)
            {
                evt.menu.AppendSeparator(null);
                evt.menu.AppendAction(
                    "Delete",
                    (a) =>
                    {
                        DeleteSelection();
                    },
                    GraphEditor.HasSelection() ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled);
            }

            if (evt.target is TNodeView nv)
            {
                evt.menu.AppendSeparator(null);
                evt.menu.AppendAction(
                    "Open Documentation",
                    (a) =>
                    {
                        GraphEditor.OpenDocumentation(nv);
                    });
            }
        }
    }
}
