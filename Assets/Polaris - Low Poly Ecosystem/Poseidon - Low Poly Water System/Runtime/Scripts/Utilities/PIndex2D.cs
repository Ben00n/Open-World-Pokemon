using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Poseidon
{
    /// <summary>
    /// Indicate an index in 2D grid with 2 component X, Z
    /// </summary>
    [System.Serializable]
    public struct PIndex2D
    {
        [SerializeField]
        private int x;
        public int X
        {
            get
            {
                return x;
            }
            set
            {
                x = value;
            }
        }

        [SerializeField]
        private int z;
        public int Z
        {
            get
            {
                return z;
            }
            set
            {
                z = value;
            }
        }

        public PIndex2D(int x, int z)
        {
            this.x = x;
            this.z = z;
        }

        public static PIndex2D operator +(PIndex2D i1, PIndex2D i2)
        {
            return new PIndex2D(i1.x + i2.x, i1.z + i2.z);
        }

        public static PIndex2D operator -(PIndex2D i1, PIndex2D i2)
        {
            return new PIndex2D(i1.x - i2.x, i1.z - i2.z);
        }

        public static bool operator ==(PIndex2D i1, PIndex2D i2)
        {
            return i1.x == i2.x && i1.z == i2.z;
        }

        public static bool operator !=(PIndex2D i1, PIndex2D i2)
        {
            return i1.x != i2.x || i1.z != i2.z;
        }

        public override string ToString()
        {
            return string.Format("({0}, {1})", x, z);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PIndex2D))
            {
                return false;
            }

            var d = (PIndex2D)obj;
            return x == d.x &&
                   z == d.z;
        }

        public override int GetHashCode()
        {
            var hashCode = 1553271884;
            hashCode = hashCode * -1521134295 + x.GetHashCode();
            hashCode = hashCode * -1521134295 + z.GetHashCode();
            return hashCode;
        }
    }
}