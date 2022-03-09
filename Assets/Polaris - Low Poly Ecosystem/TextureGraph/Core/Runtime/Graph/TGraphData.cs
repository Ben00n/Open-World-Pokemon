using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Pinwheel.TextureGraph
{
    [System.Serializable]
    public class TGraphData : ITGraphSerializeCallbackReceiver
    {
        private TGraph owner;
        public TGraph Owner
        {
            get
            {
                return owner;
            }
            internal set
            {
                owner = value;
            }
        }

        #region Node data
        private List<TAbstractTextureNode> nodes;
        public List<TAbstractTextureNode> Nodes
        {
            get
            {
                if (nodes == null)
                {
                    nodes = new List<TAbstractTextureNode>();
                }
                return nodes;
            }
            private set
            {
                nodes = value;
            }
        }

        [SerializeField]
        private List<TGraphSerializer.TSerializedElement> nodesSerialized;

        private Dictionary<Guid, TAbstractTextureNode> nodesByGuid;
        private Dictionary<Guid, TAbstractTextureNode> NodesByGuid
        {
            get
            {
                if (nodesByGuid == null)
                {
                    nodesByGuid = new Dictionary<Guid, TAbstractTextureNode>();
                }
                return nodesByGuid;
            }
        }
        #endregion

        #region Edge data
        private List<ITEdge> edges;
        public List<ITEdge> Edges
        {
            get
            {
                if (edges == null)
                {
                    edges = new List<ITEdge>();
                }
                return edges;
            }
            private set
            {
                edges = value;
            }
        }

        [SerializeField]
        private List<TGraphSerializer.TSerializedElement> edgesSerialized;

        private Dictionary<Guid, ITEdge> edgesByGuid;
        private Dictionary<Guid, ITEdge> EdgesByGuid
        {
            get
            {
                if (edgesByGuid == null)
                {
                    edgesByGuid = new Dictionary<Guid, ITEdge>();
                }
                return edgesByGuid;
            }
        }
        #endregion

        public TGraphData(TGraph owner)
        {
            this.owner = owner;
        }

        public void OnBeforeSerialize()
        {
            if (nodes != null)
            {
                foreach (TAbstractTextureNode n in nodes)
                {
                    n.OnBeforeSerialize();
                }
                nodesSerialized = TGraphSerializer.Serialize<TAbstractTextureNode>(nodes);
            }
            else
            {
                nodesSerialized = new List<TGraphSerializer.TSerializedElement>();
            }

            if (edges != null)
            {
                foreach (ITEdge e in edges)
                {
                    e.OnBeforeSerialize();
                }
                edgesSerialized = TGraphSerializer.Serialize<ITEdge>(edges);
            }
            else
            {
                edgesSerialized = new List<TGraphSerializer.TSerializedElement>();
            }
        }

        public void OnAfterDeserialize()
        {
            if (nodesSerialized != null)
            {
                nodes = TGraphSerializer.Deserialize<TAbstractTextureNode>(nodesSerialized);
                foreach (TAbstractTextureNode n in nodes)
                {
                    n.OnAfterDeserialize();
                    NodesByGuid.Add(n.GUID, n);
                }
            }

            if (edgesSerialized != null)
            {
                edges = TGraphSerializer.Deserialize<ITEdge>(edgesSerialized);
                foreach (ITEdge e in edges)
                {
                    e.OnAfterDeserialize();
                    EdgesByGuid.Add(e.GUID, e);
                }
            }

            Validate();
        }

        public void AddNode(TAbstractTextureNode n)
        {
            if (NodesByGuid.ContainsKey(n.GUID))
            {
                throw new ArgumentException("Attempting to add new a node with existing GUID.");
            }
            Nodes.Add(n);
            NodesByGuid.Add(n.GUID, n);
        }

        public void AddEdge(ITEdge e)
        {
            if (EdgesByGuid.ContainsKey(e.GUID))
            {
                throw new ArgumentException("Attempting to add new a edge with existing GUID.");
            }
            Edges.Add(e);
            EdgesByGuid.Add(e.GUID, e);
        }

        public TAbstractTextureNode GetNodeByGUID(Guid guid)
        {
            TAbstractTextureNode n;
            NodesByGuid.TryGetValue(guid, out n);
            return n;
        }

        public ITEdge GetEdgeByGUID(Guid guid)
        {
            ITEdge e;
            EdgesByGuid.TryGetValue(guid, out e);
            return e;
        }

        public void MoveNodes(IEnumerable<Guid> nodeGuids, Vector2 moveDelta)
        {
            foreach (Guid id in nodeGuids)
            {
                TAbstractTextureNode n = GetNodeByGUID(id);
                if (n != null)
                {
                    TNodeDrawState drawState = n.DrawState;
                    drawState.position.position += moveDelta;
                    n.DrawState = drawState;
                }
            }
        }

        public void RemoveEdges(IEnumerable<Guid> edgeGuids)
        {
            foreach (Guid id in edgeGuids)
            {
                ITEdge e = GetEdgeByGUID(id);
                if (e != null)
                {
                    EdgesByGuid.Remove(id);
                    Edges.Remove(e);
                }
            }
        }

        public void RemoveNodes(IEnumerable<Guid> nodeGuids)
        {
            foreach (Guid id in nodeGuids)
            {
                TAbstractTextureNode n = GetNodeByGUID(id);
                if (n != null)
                {
                    n.Dispose();
                    NodesByGuid.Remove(id);
                    Nodes.Remove(n);
                    Owner.CleanUp(n.GUID);
                }
            }
        }

        public TSlot FindSlot(TSlotReference slotRef)
        {
            TAbstractTextureNode node = GetNodeByGUID(slotRef.NodeGuid);
            if (node == null)
            {
                return null;
            }

            TSlot slot = node.GetSlotById(slotRef.SlotId);
            return slot;
        }

        public void Validate()
        {
            TGraphContext context = TGraphContext.Create(this.Owner);
            //Remove duplicated nodes
            Nodes = Nodes.Distinct(new TAbstractTextureNode.GuidComparer()).ToList();
            foreach (TAbstractTextureNode n in Nodes)
            {
                n.Validate();
            }

            //Remove duplicated edges
            Edges = Edges.Distinct().ToList();

            //Remove all edges connected to nothing.
            Edges.RemoveAll(e =>
            {
                TSlot outputSlot = FindSlot(e.OutputSlot);
                if (outputSlot == null)
                    return true;
                TSlot inputSlot = FindSlot(e.InputSlot);
                if (inputSlot == null)
                    return true;
                return false;
            });

        }

        public List<T> GetNodeOfType<T>() where T : TAbstractTextureNode
        {
            List<T> nodes = new List<T>();
            foreach (TAbstractTextureNode n in Nodes)
            {
                if (n.GetType().Equals(typeof(T)))
                {
                    nodes.Add(n as T);
                }
            }
            return nodes;
        }
    }
}
