using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Diagnostics;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;

namespace Pinwheel.TextureGraph
{
    [Serializable]
    public abstract class TAbstractTextureNode : ITGraphSerializeCallbackReceiver, IDisposable
    {
        public delegate void NodeExecutionCallback(TAbstractTextureNode n, TExecutionMetadata metadata);
        public static event NodeExecutionCallback OnBeforeExecuting;
        public static event NodeExecutionCallback OnAfterExecuting;

        public struct TExecutionMetadata
        {
            public float executionTimeMilis;
            public Vector2Int resolution;
            public GraphicsFormat format;
        }

        [NonSerialized]
        protected Guid guid;
        public Guid GUID
        {
            get
            {
                return guid;
            }
        }

        [SerializeField]
        protected string guidString;

        [SerializeField]
        protected TNodeDrawState drawState;
        public TNodeDrawState DrawState
        {
            get
            {
                return drawState;
            }
            set
            {
                drawState = value;
            }
        }

        public TAbstractTextureNode()
        {
            guid = Guid.NewGuid();
            drawState = TNodeDrawState.Create();
        }

        ~TAbstractTextureNode()
        {
            Dispose();
        }

        public virtual void OnBeforeSerialize()
        {
            guidString = guid.ToString();
        }

        public virtual void OnAfterDeserialize()
        {
            guid = new Guid(guidString);
        }

        public virtual List<TSlot> GetInputSlots()
        {
            List<TSlot> slots = new List<TSlot>();
            Type nodeType = GetType();
            FieldInfo[] fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];
                if (f.FieldType == typeof(TSlot))
                {
                    TSlot slot = f.GetValue(this) as TSlot;
                    if (slot.SlotType == TSlotType.Input)
                    {
                        slots.Add(slot);
                    }
                }
            }

            return slots;
        }

        public virtual List<TSlot> GetOutputSlots()
        {
            List<TSlot> slots = new List<TSlot>();
            Type nodeType = GetType();
            FieldInfo[] fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];
                if (f.FieldType == typeof(TSlot))
                {
                    TSlot slot = f.GetValue(this) as TSlot;
                    if (slot.SlotType == TSlotType.Output)
                    {
                        slots.Add(slot);
                    }
                }
            }

            return slots;
        }

        public virtual TSlot GetSlotById(int id)
        {
            Type nodeType = GetType();
            FieldInfo[] fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];
                if (f.FieldType == typeof(TSlot))
                {
                    TSlot slot = f.GetValue(this) as TSlot;
                    if (slot.Id == id)
                    {
                        return slot;
                    }
                }
            }

            return null;
        }

        public virtual void Validate() { }

        public void ExecuteWithCallback(TGraphContext context)
        {
            Validate();
            TExecutionMetadata meta = new TExecutionMetadata();

            if (OnBeforeExecuting != null)
            {
                OnBeforeExecuting.Invoke(this, meta);
            }

            Stopwatch st = new Stopwatch();
            st.Start();
            Execute(context);
            st.Stop();
            meta.executionTimeMilis = st.ElapsedMilliseconds;

            TSlot previewSlot = GetMainOutputSlot();
            if (previewSlot != null)
            {
                RenderTexture rt = context.GetRT(TSlotReference.Create(GUID, previewSlot.Id));
                if (rt != null)
                {
                    meta.resolution = new Vector2Int(rt.width, rt.height);
                    meta.format = rt.graphicsFormat;
                }
            }

            if (OnAfterExecuting != null)
            {
                OnAfterExecuting.Invoke(this, meta);
            }
        }

        public abstract void Execute(TGraphContext context);

        public virtual void Dispose()
        {
            //UnityEngine.Debug.Log($"Dispose {GetType().Name}");

            Type nodeType = GetType();
            FieldInfo[] fields = nodeType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            for (int i = 0; i < fields.Length; ++i)
            {
                FieldInfo f = fields[i];
                if (f.FieldType.IsSubclassOf(typeof(Texture)))
                {
                    Texture tex = f.GetValue(this) as Texture;
                    if (tex != null)
                    {
                        TUtilities.DestroyObject(tex);
                    }
                }
            }
        }

        protected virtual TRenderTextureRequest GetRenderTextureRequest(TSlot slot)
        {
            TRenderTextureRequest r = new TRenderTextureRequest()
            {
                DataType = slot.DataType
            };
            return r;
        }

        public virtual TSlot GetMainOutputSlot()
        {
            return null;
        }

        public struct GuidComparer : IEqualityComparer<TAbstractTextureNode>
        {
            public bool Equals(TAbstractTextureNode x, TAbstractTextureNode y)
            {
                return x.guid.Equals(y.guid);
            }

            public int GetHashCode(TAbstractTextureNode n)
            {
                return n.GetHashCode();
            }
        }
    }
}
