using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    [CreateAssetMenu(menuName = "Texture Graph/New Graph", fileName = "New Texture Graph")]
    public class TGraph : ScriptableObject, ISerializationCallbackReceiver
    {
        [NonSerialized]
        private TGraphData graphData;
        public TGraphData GraphData
        {
            get
            {
                if (graphData.Owner == null)
                {
                    graphData.Owner = this;
                }
                return graphData;
            }
        }

        [SerializeField]
        private TGraphSerializer.TSerializedElement graphDataSerialized;

        [SerializeField]
        private Vector2Int baseResolution;
        public Vector2Int BaseResolution
        {
            get
            {
                return baseResolution;
            }
            set
            {
                baseResolution = new Vector2Int(
                    Mathf.Clamp(value.x, TConst.TEXTURE_SIZE_MIN.x, TConst.TEXTURE_SIZE_MAX.x),
                    Mathf.Clamp(value.y, TConst.TEXTURE_SIZE_MIN.x, TConst.TEXTURE_SIZE_MAX.x));
            }
        }

        [SerializeField]
        private bool useHighPrecision;
        public bool UseHighPrecision
        {
            get
            {
                return useHighPrecision;
            }
            set
            {
                useHighPrecision = value;
            }
        }

        private Dictionary<TSlotReference, RenderTexture> rtPool;
        public Dictionary<TSlotReference, RenderTexture> RtPool
        {
            get
            {
                if (rtPool == null)
                {
                    rtPool = new Dictionary<TSlotReference, RenderTexture>();
                }
                return rtPool;
            }
        }

#if UNITY_EDITOR
        [SerializeField]
        private TView3DEnvironmentSettings view3dEnvironmentSettings;
        public TView3DEnvironmentSettings View3DEnvironmentSettings
        {
            get
            {
                return view3dEnvironmentSettings;
            }
            set
            {
                view3dEnvironmentSettings = value;
            }
        }
#endif

        public Action<TGraph> OnAfterExecuting { get; set; }

        public void Reset()
        {
            graphData = new TGraphData(this);
            baseResolution = new Vector2Int(512, 512);
            useHighPrecision = true;

#if UNITY_EDITOR
            view3dEnvironmentSettings = new TView3DEnvironmentSettings();
#endif
        }

        private void OnDisable()
        {
            CleanUp();
        }

        public void OnBeforeSerialize()
        {
            if (graphData != null)
            {
                graphData.OnBeforeSerialize();
                graphDataSerialized = TGraphSerializer.Serialize(graphData);
            }
            else
            {
                graphDataSerialized = TGraphSerializer.NullElement;
            }
        }

        public void OnAfterDeserialize()
        {
            if (!graphDataSerialized.Equals(TGraphSerializer.NullElement))
            {
                graphData = TGraphSerializer.Deserialize<TGraphData>(graphDataSerialized, this);
                graphData.OnAfterDeserialize();
            }
            else
            {
                graphData = new TGraphData(this);
            }
        }

        public void CopySerializeDataTo(TGraph otherGraph)
        {
            OnBeforeSerialize();
            otherGraph.graphDataSerialized = this.graphDataSerialized;
            otherGraph.baseResolution = this.baseResolution;
            otherGraph.useHighPrecision = this.useHighPrecision;
#if UNITY_EDITOR
            otherGraph.view3dEnvironmentSettings = this.view3dEnvironmentSettings;
#endif
            otherGraph.OnAfterDeserialize();
        }

        public void Execute()
        {
            TGraphContext context = TGraphContext.Create(this);
            if (context.HasConnectionLoop())
            {
                throw new Exception("The graph has connection loop. Abort execution!");
            }

            context.MarkAllNodesDirty();
            foreach (TAbstractTextureNode n in GraphData.Nodes)
            {
                if (context.IsNodeDirty(n.GUID))
                {
                    n.ExecuteWithCallback(context);
                    context.SetNodeDirtyState(n.GUID, false);
                }
            }

            if (OnAfterExecuting != null)
            {
                OnAfterExecuting.Invoke(this);
            }
        }

        public void ExecuteAt(Guid nodeGuid)
        {
            TAbstractTextureNode startNode = GraphData.GetNodeByGUID(nodeGuid);
            if (startNode == null)
                return;
            TGraphContext context = TGraphContext.Create(this);
            if (context.HasConnectionLoop(startNode.GUID))
            {
                throw new Exception(string.Format("The graph has connection loop at {0}. Abort execution!", startNode.GetType().Name));
            }

            context.MarkNodeDirtyRecursive(startNode.GUID);
            foreach (TAbstractTextureNode n in GraphData.Nodes)
            {
                if (context.IsNodeDirty(n.GUID))
                {
                    n.ExecuteWithCallback(context);
                    context.SetNodeDirtyState(n.GUID, false);
                }
            }

            if (OnAfterExecuting != null)
            {
                OnAfterExecuting.Invoke(this);
            }
        }

        public void ExecuteAt(IEnumerable<Guid> nodeGuids)
        {
            TGraphContext context = TGraphContext.Create(this);
            foreach (Guid id in nodeGuids)
            {
                TAbstractTextureNode startNode = GraphData.GetNodeByGUID(id);
                if (startNode == null)
                    return;
                if (context.HasConnectionLoop(startNode.GUID))
                {
                    throw new Exception(string.Format("The graph has connection loop at {0}. Abort execution!", startNode.GetType().Name));
                }

                context.MarkNodeDirtyRecursive(startNode.GUID);
            }

            foreach (TAbstractTextureNode n in GraphData.Nodes)
            {
                if (context.IsNodeDirty(n.GUID))
                {
                    n.ExecuteWithCallback(context);
                    context.SetNodeDirtyState(n.GUID, false);
                }
            }

            if (OnAfterExecuting != null)
            {
                OnAfterExecuting.Invoke(this);
            }
        }

        public RenderTexture RequestTargetRT(TSlotReference slot, TRenderTextureRequest request)
        {
            bool isGrayscale = request.DataType == TSlotDataType.Gray;
            RenderTextureFormat format = UseHighPrecision ?
                (isGrayscale ? RenderTextureFormat.RFloat : RenderTextureFormat.ARGBFloat) :
                (isGrayscale ? RenderTextureFormat.R8 : RenderTextureFormat.ARGB32);
            int width = BaseResolution.x;
            int height = BaseResolution.y;

            RenderTexture rt = null;
            if (RtPool.TryGetValue(slot, out rt) && rt != null)
            {
                if (rt.width != width ||
                    rt.height != height ||
                    rt.format != format)
                {
                    rt.Release();
                    TUtilities.DestroyObject(rt);
                    rt = null;
                    RtPool.Remove(slot);
                }
            }

            if (rt == null)
            {
                rt = new RenderTexture(width, height, 0, format, RenderTextureReadWrite.Linear);
                rt.wrapMode = TextureWrapMode.Clamp;
                rt.name = slot.ToString();
                RtPool[slot] = rt;
            }

            return rt;
        }

        public RenderTexture GetRT(TSlotReference slot)
        {
            RenderTexture rt;
            RtPool.TryGetValue(slot, out rt);
            return rt;
        }

        public void CleanUp(Guid nodeGuid)
        {
            foreach (var entry in RtPool)
            {
                if (entry.Key.NodeGuid.Equals(nodeGuid))
                {
                    RenderTexture rt = entry.Value;
                    if (rt != null)
                    {
                        rt.Release();
                        TUtilities.DestroyObject(rt);
                    }
                }
            }
        }

        public void CleanUp()
        {
            foreach (var entry in RtPool)
            {
                RenderTexture rt = entry.Value;
                if (rt != null)
                {
                    rt.Release();
                    TUtilities.DestroyObject(rt);
                }
            }
        }

        public RenderTexture GetMainRT(Guid nodeGuid)
        {
            TAbstractTextureNode n = GraphData.GetNodeByGUID(nodeGuid);
            if (n == null)
            {
                return null;
            }
            else
            {
                TSlot mainSlot = n.GetMainOutputSlot();
                return GetRT(TSlotReference.Create(n.GUID, mainSlot.Id));
            }
        }
    }
}
