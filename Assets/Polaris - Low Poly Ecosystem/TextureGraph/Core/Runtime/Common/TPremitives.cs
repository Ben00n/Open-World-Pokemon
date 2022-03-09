using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    public static class TPremitives
    {
        [System.Serializable]
        public enum TMeshType
        {
            Plane, Cube, Sphere, Cylinder, Torus, Custom
        }

        public static Mesh Get(TMeshType t)
        {
            if (t == TMeshType.Custom)
            {
                return null;
            }
            else
            {
                Mesh m = null;
                m = Resources.Load<Mesh>("TextureGraph/Meshes/" + t.ToString());
                return m;
            }
        }
    }
}
