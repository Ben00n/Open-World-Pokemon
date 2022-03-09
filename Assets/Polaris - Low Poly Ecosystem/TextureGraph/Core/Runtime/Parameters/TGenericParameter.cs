using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    [Serializable]
    public abstract class TGenericParameter
    {

    }

    [Serializable]
    public class TGenericParameter<T> : TGenericParameter, IEquatable<TGenericParameter<T>>
    {
        [SerializeField]
        public T value;

        public bool Equals(TGenericParameter<T> other)
        {
            if (this.value == null)
            {
                return other.value == null;
            }
            else
            {
                return this.value.Equals(other.value);
            }
        }
    }
}
