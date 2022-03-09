using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    [Serializable]
    public class TEdge : ITEdge
    {
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
        protected string guidSerialized;

        [SerializeField]
        protected TSlotReference outputSlot;
        public TSlotReference OutputSlot
        {
            get
            {
                return outputSlot;
            }
        }

        [SerializeField]
        protected TSlotReference inputSlot;
        public TSlotReference InputSlot
        {
            get
            {
                return inputSlot;
            }
        }

        public TEdge()
        { }

        public TEdge(TSlotReference outputSlot, TSlotReference inputSlot)
        {
            this.guid = Guid.NewGuid();
            this.outputSlot = outputSlot;
            this.inputSlot = inputSlot;
        }

        public void OnBeforeSerialize()
        {
            guidSerialized = guid.ToString();
            outputSlot.OnBeforeSerialize();
            inputSlot.OnBeforeSerialize();
        }

        public void OnAfterDeserialize()
        {
            guid = new Guid(guidSerialized);
            outputSlot.OnAfterDeserialize();
            inputSlot.OnAfterDeserialize();
        }

        public bool Equals(ITEdge other)
        {
            return this.OutputSlot.Equals(other.OutputSlot) && this.InputSlot.Equals(other.InputSlot);
        }

        public override int GetHashCode()
        {
            return OutputSlot.GetHashCode() ^ InputSlot.GetHashCode();
        }
    }
}
