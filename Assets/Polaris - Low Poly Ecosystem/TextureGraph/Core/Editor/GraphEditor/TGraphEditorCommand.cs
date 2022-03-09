using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using TSerializedElement = Pinwheel.TextureGraph.TGraphSerializer.TSerializedElement;

namespace Pinwheel.TextureGraph
{
    public partial class TGraphEditor : EditorWindow
    {
        private void HandleShortcutKeys(KeyCode k, bool isControl, bool isShift, bool isAlt)
        {
            k = Event.current.keyCode;
            if (isControl && k == KeyCode.S)
            {
                if (SourceGraph != null && ClonedGraph != null)
                {
                    HandleSave();
                }
            }
            else if (isControl && k == KeyCode.X)
            {
                if (ClonedGraph != null)
                {
                    HandleCut();
                }
            }
            else if (isControl && k == KeyCode.C)
            {
                if (ClonedGraph != null)
                {
                    HandleCopy();
                }
            }
            else if (isControl && k == KeyCode.V)
            {
                if (ClonedGraph != null)
                {
                    HandlePaste();
                }
            }
            else if (isControl && k == KeyCode.D)
            {
                if (ClonedGraph != null)
                {
                    HandleCopy();
                    HandlePaste();
                }
            }
            else if (isControl && k == KeyCode.Z)
            {
                if (ClonedGraph != null)
                {
                    HandleUndo();
                }
            }
            else if (isControl && k == KeyCode.Y)
            {
                if (ClonedGraph != null)
                {
                    HandleRedo();
                }
            }
        }

        private void HandleShortcutKeysOnGraphView(KeyDownEvent e)
        {
            HandleShortcutKeys(e.keyCode, e.ctrlKey, e.shiftKey, e.altKey);
        }

        public void HandleSave()
        {
            TGraphSaver.Save(ClonedGraph, SourceGraph);
        }

        public void HandleCut()
        {
            if (!HasSelection())
                return;

            HandleCopy(true);
            GraphView.DeleteSelection();
        }

        public void HandleCopy(bool isCut = false)
        {
            if (!HasSelection())
                return;

            List<ITGraphSerializeCallbackReceiver> serializedElements = new List<ITGraphSerializeCallbackReceiver>();
            foreach (ISelectable s in GraphView.selection)
            {
                if (s is Node n)
                {
                    Guid guid = (Guid)n.userData;
                    TAbstractTextureNode node = ClonedGraph.GraphData.GetNodeByGUID(guid);
                    if (node != null)
                    {
                        serializedElements.Add(node);
                    }
                }
                if (s is Edge e)
                {
                    Guid guid = (Guid)e.userData;
                    ITEdge edge = ClonedGraph.GraphData.GetEdgeByGUID(guid);
                    if (edge != null)
                    {
                        serializedElements.Add(edge);
                    }
                }
            }
            TClipboard.SetData(serializedElements, isCut);
        }

        public void HandlePaste()
        {
            if (!CanPaste())
                return;

            List<TSerializedElement> nodeData = new List<TSerializedElement>();
            List<TSerializedElement> edgeData = new List<TSerializedElement>();
            TClipboard.GetData(nodeData, edgeData);
            if (TClipboard.IsCut)
            {
                TClipboard.Clear();
            }

            List<TAbstractTextureNode> nodes = new List<TAbstractTextureNode>();
            List<ITEdge> edges = new List<ITEdge>();

            Dictionary<Guid, Guid> guidRemap = new Dictionary<Guid, Guid>();

            Vector2 avgNodePos = Vector2.zero;
            foreach (TSerializedElement data in nodeData)
            {
                TAbstractTextureNode n = TGraphSerializer.Deserialize<TAbstractTextureNode>(data);
                n.OnAfterDeserialize();
                Guid oldGuid = n.GUID;
                Guid newGuid = Guid.NewGuid();
                guidRemap.Add(oldGuid, newGuid);
                TGuidSetter.Set(n, newGuid);
                nodes.Add(n);

                avgNodePos += n.DrawState.position.position + TConst.NODE_CREATION_POSITION_OFFSET;
            }

            avgNodePos = avgNodePos / nodes.Count;
            Vector2 mousePosInGraphView = GraphView.LocalMousePosition;
            Vector2 worldMousePos = GraphView.LocalToWorld(mousePosInGraphView);
            Vector2 newCenterPoint = GraphView.contentViewContainer.WorldToLocal(worldMousePos);
            Vector2 offset = newCenterPoint - avgNodePos;
            foreach (TAbstractTextureNode n in nodes)
            {
                TNodeDrawState state = n.DrawState;
                state.position.position += offset;
                n.DrawState = state;
            }

            foreach (TSerializedElement data in edgeData)
            {
                ITEdge e = TGraphSerializer.Deserialize<ITEdge>(data);
                e.OnAfterDeserialize();
                Guid newGuid = Guid.NewGuid();
                TGuidSetter.Set(e, newGuid);

                TSlotReference inputRef = e.InputSlot;
                Guid inputGUID = inputRef.NodeGuid;
                if (!guidRemap.TryGetValue(inputGUID, out inputGUID))
                {
                    continue;
                }

                TSlotReference outputRef = e.OutputSlot;
                Guid outputGUID = outputRef.NodeGuid;
                if (!guidRemap.TryGetValue(outputGUID, out outputGUID))
                {
                    continue;
                }

                TEdge newEdge = new TEdge(TSlotReference.Create(outputGUID, outputRef.SlotId), TSlotReference.Create(inputGUID, inputRef.SlotId));
                edges.Add(newEdge);
            }

            foreach (TAbstractTextureNode n in nodes)
            {
                ClonedGraph.GraphData.AddNode(n);
            }
            foreach (ITEdge e in edges)
            {
                ClonedGraph.GraphData.AddEdge(e);
            }

            SetGraphDirty(SourceGraph, true);

            int nodeStartIndex = ClonedGraph.GraphData.Nodes.Count - nodes.Count;
            int edgeStartIndex = ClonedGraph.GraphData.Edges.Count - edges.Count;
            int[] nodeIndices = TUtilities.GetIndicesArray(nodeStartIndex, ClonedGraph.GraphData.Nodes.Count - 1);
            int[] edgeIndices = TUtilities.GetIndicesArray(edgeStartIndex, ClonedGraph.GraphData.Edges.Count - 1);
            List<GraphElement> newViews = GraphView.CreateViews(ClonedGraph, nodeIndices, edgeIndices);
            GraphView.ClearSelection();
            foreach (ISelectable v in newViews)
            {
                GraphView.AddToSelection(v);
            }
        }

        public bool HasSelection()
        {
            return GraphView.selection.Count > 0;
        }

        public bool CanPaste()
        {
            return TClipboard.HasData();
        }

        public void HandleUndo()
        {
            Undo.PerformUndo();
        }

        public void HandleRedo()
        {
            Undo.PerformRedo();
        }

        public void OpenGraphSettings()
        {
            if (settingsWindow == null)
            {
                settingsWindow = TGraphSettingsWindow.ShowWindow(this);
            }
            else
            {
                settingsWindow.Focus();
            }
        }

        public void OpenExportWindow()
        {
            if (exportWindow == null)
            {
                exportWindow = TExportWindow.ShowWindow(this);
            }
            else
            {
                exportWindow.Focus();
            }
        }

        public void OpenDocumentation(TNodeView nv)
        {
            Guid nodeGuid = (Guid)nv.userData;
            TAbstractTextureNode n = ClonedGraph.GraphData.GetNodeByGUID(nodeGuid);
            if (n != null)
            {
                TNodeMetadataAttribute meta = TNodeMetadataInitializer.GetMetadata(n.GetType());
                if (!string.IsNullOrEmpty(meta.Documentation))
                {
                    Application.OpenURL(meta.Documentation);
                }
            }
        }
    }
}
