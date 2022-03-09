using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    [System.Serializable]
    public struct TNodeDrawState
    {
        [SerializeField]
        public Rect position;

        public static TNodeDrawState Create()
        {
            TNodeDrawState state = new TNodeDrawState()
            {
                position = new Rect(0, 0, 0, 0)
            };
            return state;
        }
    }
}
