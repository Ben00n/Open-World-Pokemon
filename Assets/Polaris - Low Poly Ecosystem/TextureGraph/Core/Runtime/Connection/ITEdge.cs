using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    public interface ITEdge : ITGraphSerializeCallbackReceiver, IEquatable<ITEdge>
    {
        Guid GUID { get; }
        TSlotReference OutputSlot { get; }
        TSlotReference InputSlot { get; }
    }
}
