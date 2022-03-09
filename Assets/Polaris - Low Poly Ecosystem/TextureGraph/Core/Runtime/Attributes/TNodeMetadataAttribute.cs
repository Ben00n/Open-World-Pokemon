using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.TextureGraph
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TNodeMetadataAttribute : Attribute
    {
        public string Title { get; set; }
        public string CreationMenu { get; set; }
        public string Icon { get; set; }
        public string Documentation { get; set; }
    }
}
