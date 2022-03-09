using UnityEngine;

namespace Pinwheel.Poseidon
{
    [System.Serializable]
    public class PSplineAnchor
    {
        [SerializeField]
        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        [SerializeField]
        private Quaternion rotation;
        public Quaternion Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        [SerializeField]
        private Vector3 scale;
        public Vector3 Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        public PSplineAnchor()
        {
            position = Vector3.zero;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

        public PSplineAnchor(Vector3 pos)
        {
            position = pos;
            rotation = Quaternion.identity;
            scale = Vector3.one;
        }

#if GRIFFIN_2020
        public static explicit operator PSplineAnchor(Pinwheel.Griffin.SplineTool.GSplineAnchor a)
        {
            PSplineAnchor anchor = new PSplineAnchor();
            anchor.Position = a.Position;
            anchor.Rotation = a.Rotation;
            anchor.Scale = a.Scale;
            return anchor;
        }
#endif
    }
}
