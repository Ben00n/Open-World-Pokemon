using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;

namespace Pinwheel.TextureGraph
{
    public static class TGraphEditorUtilities
    {
        public static bool SlotRefEqual(Port p0, Port p1)
        {
            TSlotReference id0 = (TSlotReference)p0.userData;
            TSlotReference id1 = (TSlotReference)p1.userData;
            return id0.Equals(id1);
        }
    }
}
