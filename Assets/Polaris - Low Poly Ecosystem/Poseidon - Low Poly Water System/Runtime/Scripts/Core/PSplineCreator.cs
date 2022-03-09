using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Poseidon
{
    [System.Serializable]
    [ExecuteInEditMode]
    public class PSplineCreator : MonoBehaviour
    {
        [SerializeField]
        private int groupId;
        public int GroupId
        {
            get
            {
                return groupId;
            }
            set
            {
                groupId = value;
            }
        }

        [SerializeField]
        private Vector3 positionOffset;
        public Vector3 PositionOffset
        {
            get
            {
                return positionOffset;
            }
            set
            {
                positionOffset = value;
            }
        }

        [SerializeField]
        private Quaternion initialRotation;
        public Quaternion InitialRotation
        {
            get
            {
                return initialRotation;
            }
            set
            {
                initialRotation = value;
            }
        }

        [SerializeField]
        private Vector3 initialScale;
        public Vector3 InitialScale
        {
            get
            {
                return initialScale;
            }
            set
            {
                initialScale = value;
            }
        }

        [SerializeField]
        private int smoothness;
        public int Smoothness
        {
            get
            {
                return smoothness;
            }
            set
            {
                smoothness = Mathf.Max(2, value);
            }
        }

        [SerializeField]
        private float width;
        public float Width
        {
            get
            {
                return width;
            }
            set
            {
                width = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private PSpline spline;
        public PSpline Spline
        {
            get
            {
                if (spline == null)
                {
                    spline = new PSpline();
                }
                return spline;
            }
            set
            {
                spline = value;
            }
        }

        public void Reset()
        {
            PositionOffset = Vector3.zero;
            InitialRotation = Quaternion.identity;
            InitialScale = Vector3.one;
            Smoothness = 20;
            Width = 10;
        }
    }
}
