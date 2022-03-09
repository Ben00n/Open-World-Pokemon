using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TCustomParametersDrawerAttribute : Attribute
    {
        public Type type { get; set; }

        public TCustomParametersDrawerAttribute(Type t)
        {
            if (t == null)
                throw new ArgumentException("Target type cannot be null.");
            type = t;
        }
    }
}
