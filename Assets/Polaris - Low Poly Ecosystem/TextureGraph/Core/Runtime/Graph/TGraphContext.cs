using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    public struct TGraphContext
    {
        public Vector2Int baseResolution;
        public bool useHighPrecision;

        private TGraph graph;
        private Dictionary<TSlotReference, TSlotReference> inputLinks;
        private Dictionary<TSlotReference, List<TSlotReference>> outputLinks;
        private HashSet<Guid> dirtyState;

        public static TGraphContext Create(TGraph graph)
        {
            TGraphContext context = new TGraphContext();
            context.graph = graph;
            context.baseResolution = graph.BaseResolution;
            context.useHighPrecision = graph.UseHighPrecision;
            context.dirtyState = new HashSet<Guid>();
            context.BuildInputLinks();
            context.BuildOutputLinks();
            return context;
        }

        private void BuildInputLinks()
        {
            inputLinks = new Dictionary<TSlotReference, TSlotReference>();

            List<ITEdge> edges = graph.GraphData.Edges;
            foreach (ITEdge e in edges)
            {
                TSlot inputSlot = graph.GraphData.FindSlot(e.InputSlot);
                if (inputSlot == null)
                    continue;

                TSlot outputSlot = graph.GraphData.FindSlot(e.OutputSlot);
                if (outputSlot == null)
                    continue;

                inputLinks.Add(e.InputSlot, e.OutputSlot);
            }
        }

        private void BuildOutputLinks()
        {
            outputLinks = new Dictionary<TSlotReference, List<TSlotReference>>();
            List<ITEdge> edges = graph.GraphData.Edges;
            foreach (ITEdge e in edges)
            {
                TSlot inputSlot = graph.GraphData.FindSlot(e.InputSlot);
                if (inputSlot == null)
                    continue;

                TSlot outputSlot = graph.GraphData.FindSlot(e.OutputSlot);
                if (outputSlot == null)
                    continue;

                List<TSlotReference> inputRefs;
                if (!outputLinks.ContainsKey(e.OutputSlot))
                {
                    inputRefs = new List<TSlotReference>();
                    outputLinks.Add(e.OutputSlot, inputRefs);
                }

                inputRefs = outputLinks[e.OutputSlot];
                inputRefs.Add(e.InputSlot);
            }
        }

        /// <summary>
        /// Return the outputSlot that connected to this inputSlot
        /// </summary>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public TSlotReference GetInputLink(TSlotReference inputSlot)
        {
            TSlotReference outputSlot = default;
            inputLinks.TryGetValue(inputSlot, out outputSlot);
            return outputSlot;
        }

        public List<TSlotReference> GetOutputLinks(TSlotReference outputSlot)
        {
            List<TSlotReference> inputSlots = null;
            outputLinks.TryGetValue(outputSlot, out inputSlots);

            if (inputSlots == null)
            {
                inputSlots = new List<TSlotReference>();
            }
            return inputSlots;
        }

        /// <summary>
        /// Return the texture for an input slot from the other node it connected to.
        /// </summary>
        /// <param name="inputSlot"></param>
        /// <returns></returns>
        public Texture GetInputTexture(TSlotReference inputSlot)
        {
            TSlotReference outputSlot = GetInputLink(inputSlot);
            TAbstractTextureNode node = graph.GraphData.GetNodeByGUID(outputSlot.NodeGuid);
            if (node == null)
                return null;

            if (IsNodeDirty(node.GUID))
            {
                node.ExecuteWithCallback(this);
                SetNodeDirtyState(node.GUID, false);
            }

            return graph.GetRT(outputSlot);
        }

        public TSlot GetSlot(TSlotReference slotRef)
        {
            return graph.GraphData.FindSlot(slotRef);
        }

        public void MarkAllNodesDirty()
        {
            List<TAbstractTextureNode> nodes = graph.GraphData.Nodes;
            foreach (TAbstractTextureNode n in nodes)
            {
                dirtyState.Add(n.GUID);
            }
        }

        public void MarkNodeDirtyRecursive(Guid startNodeGuid)
        {
            HashSet<Guid> flags = new HashSet<Guid>();
            Stack<Guid> stack = new Stack<Guid>();
            stack.Push(startNodeGuid);
            flags.Add(startNodeGuid);
            while (stack.Count > 0)
            {
                Guid guid = stack.Pop();
                TAbstractTextureNode node = graph.GraphData.GetNodeByGUID(guid);
                if (node == null)
                    continue;
                dirtyState.Add(node.GUID);

                List<TSlot> outputSlots = node.GetOutputSlots();
                foreach (TSlot slot in outputSlots)
                {
                    TSlotReference slotRef = TSlotReference.Create(node.GUID, slot.Id);
                    List<TSlotReference> links = GetOutputLinks(slotRef);
                    foreach (TSlotReference sr in links)
                    {
                        if (!flags.Contains(sr.NodeGuid))
                        {
                            stack.Push(sr.NodeGuid);
                            flags.Add(sr.NodeGuid);
                        }
                    }
                }
            }
        }

        public void SetNodeDirtyState(Guid guid, bool isDirty)
        {
            if (isDirty)
            {
                dirtyState.Add(guid);
            }
            else
            {
                dirtyState.Remove(guid);
            }
        }

        public bool IsNodeDirty(Guid guid)
        {
            return dirtyState.Contains(guid);
        }

        public bool HasConnectionLoop()
        {
            List<TAbstractTextureNode> nodes = graph.GraphData.Nodes;
            foreach (TAbstractTextureNode n in nodes)
            {
                if (HasConnectionLoop(n.GUID))
                    return true;
            }

            return false;
        }

        public bool HasConnectionLoop(Guid nodeGuid)
        {
            HashSet<Guid> flags = new HashSet<Guid>();
            Stack<Guid> stack = new Stack<Guid>();
            stack.Push(nodeGuid);
            flags.Add(nodeGuid);
            while (stack.Count > 0)
            {
                Guid guid = stack.Pop();
                TAbstractTextureNode node = graph.GraphData.GetNodeByGUID(guid);
                if (node == null)
                    continue;
                List<TSlot> outputSlots = node.GetOutputSlots();
                foreach (TSlot slot in outputSlots)
                {
                    TSlotReference slotRef = TSlotReference.Create(node.GUID, slot.Id);
                    List<TSlotReference> links = GetOutputLinks(slotRef);
                    foreach (TSlotReference sr in links)
                    {
                        if (sr.NodeGuid.Equals(nodeGuid))
                            return true;

                        if (!flags.Contains(sr.NodeGuid))
                        {
                            stack.Push(sr.NodeGuid);
                            flags.Add(sr.NodeGuid);
                        }
                    }
                }
            }

            return false;
        }

        public RenderTexture RequestTargetRT(TSlotReference slot, TRenderTextureRequest request)
        {
            return graph.RequestTargetRT(slot, request);
        }

        public RenderTexture GetRT(TSlotReference slot)
        {
            return graph.GetRT(slot);
        }
    }
}
