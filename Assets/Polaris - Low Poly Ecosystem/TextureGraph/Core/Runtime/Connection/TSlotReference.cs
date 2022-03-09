using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    [Serializable]
    public struct TSlotReference : ITGraphSerializeCallbackReceiver, IEquatable<TSlotReference>
    {
        [NonSerialized]
        private Guid nodeGuid;
        public Guid NodeGuid
        {
            get
            {
                return nodeGuid;
            }
        }

        [SerializeField]
        private string nodeGuidSerialized;

        [SerializeField]
        private int slotId;
        public int SlotId
        {
            get
            {
                return slotId;
            }
        }

        public static TSlotReference Create(Guid nodeGuid, int slotId)
        {
            TSlotReference slotRef = new TSlotReference();
            slotRef.nodeGuid = nodeGuid;
            slotRef.slotId = slotId;
            return slotRef;
        }

        public void OnBeforeSerialize()
        {
            nodeGuidSerialized = nodeGuid.ToString();
        }

        public void OnAfterDeserialize()
        {
            nodeGuid = new Guid(nodeGuidSerialized);
        }

        public bool Equals(TSlotReference other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return slotId == other.slotId && nodeGuid.Equals(other.nodeGuid);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((TSlotReference)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (slotId * 397) ^ nodeGuid.GetHashCode();
            }
        }

        public override string ToString()
        {
            return NodeGuid.ToString() + "\n" + SlotId;
        }
    }
}
