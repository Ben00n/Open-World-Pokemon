#if UNITY_EDITOR && GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon.GriffinExtension
{
    public static class PGriffinExtensionCommon
    {
        public static PWater FindTargetWaterObject()
        {
            PWater[] waters = Object.FindObjectsOfType<PWater>();
            for (int i=0;i<waters.Length;++i)
            {
                if (waters[i].gameObject.name.StartsWith("~"))
                {
                    return waters[i];
                }
            }
            return null;
        }

        public static PWater CreateTargetWaterObject()
        {
            PWater water = PEditorMenus.CreateCalmWaterHQObject(null);
            water.name = "~PoseidonWater";
            return water;
        }
    }
}
#endif
