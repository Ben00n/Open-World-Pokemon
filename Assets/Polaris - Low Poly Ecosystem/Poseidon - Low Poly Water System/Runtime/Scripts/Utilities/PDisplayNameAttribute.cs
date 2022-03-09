using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.Poseidon
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class PDisplayName : Attribute
    {
        public string DisplayName { get; set; }

        public PDisplayName(string name)
        {
            DisplayName = name;
        }
    }
}
