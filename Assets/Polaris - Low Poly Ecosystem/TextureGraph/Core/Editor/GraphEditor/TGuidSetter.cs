using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace Pinwheel.TextureGraph
{
    public static class TGuidSetter
    {
        public static bool Set(object o, Guid guid)
        {
            Type t = o.GetType();
            FieldInfo guidField = t.GetField("guid", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (guidField == null)
            {
                return false;
            }
            else
            {
                guidField.SetValue(o, guid);
                return true;
            }
        }
    }
}
