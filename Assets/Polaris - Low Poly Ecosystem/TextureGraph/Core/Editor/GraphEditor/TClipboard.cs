using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TSerializedElement = Pinwheel.TextureGraph.TGraphSerializer.TSerializedElement;

namespace Pinwheel.TextureGraph
{
    public static class TClipboard
    {
        private static List<TSerializedElement> nodeData;
        private static List<TSerializedElement> NodeData
        {
            get
            {
                if (nodeData == null)
                {
                    nodeData = new List<TSerializedElement>();
                }
                return nodeData;
            }
        }

        private static List<TSerializedElement> edgeData;
        private static List<TSerializedElement> EdgeData
        {
            get
            {
                if (edgeData == null)
                {
                    edgeData = new List<TSerializedElement>();
                }
                return edgeData;
            }
        }

        public static bool IsCut { get; private set; }

        public static bool HasData()
        {
            return NodeData.Count > 0 || EdgeData.Count > 0;
        }

        public static void SetData(IEnumerable<ITGraphSerializeCallbackReceiver> element, bool isCut = false)
        {
            Clear();
            foreach (ITGraphSerializeCallbackReceiver e in element)
            {
                e.OnBeforeSerialize();
                TSerializedElement serializedData = TGraphSerializer.Serialize(e);
                if (e is TAbstractTextureNode)
                {
                    NodeData.Add(serializedData);
                }
                else if (e is ITEdge)
                {
                    EdgeData.Add(serializedData);
                }
            }
            IsCut = isCut;
        }

        public static void Clear()
        {
            NodeData.Clear();
            EdgeData.Clear();
        }

        public static void GetData(List<TSerializedElement> nodes = null, List<TSerializedElement> edges = null)
        {
            if (nodes != null)
            {
                nodes.Clear();
                nodes.AddRange(NodeData);
            }
            if (edges != null)
            {
                edges.Clear();
                edges.AddRange(EdgeData);
            }
        }
    }
}
