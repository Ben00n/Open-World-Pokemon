using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Poseidon;

namespace Pinwheel.Poseidon
{
    //[CreateAssetMenu(menuName = "Poseidon/Spline Tool Config")]
    public class PSplineToolConfig : ScriptableObject
    {
        private static PSplineToolConfig instance;
        public static PSplineToolConfig Instance
        {
            get
            {
                if (instance==null)
                {
                    instance = Resources.Load<PSplineToolConfig>("SplineToolConfig");
                    if (instance==null)
                    {
                        instance = ScriptableObject.CreateInstance<PSplineToolConfig>();
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        private int raycastLayer;
        public int RaycastLayer
        {
            get
            {
                return raycastLayer;
            }
            set
            {
                raycastLayer = value;
            }
        }

        [SerializeField]
        private float yOffset;
        public float YOffset
        {
            get
            {
                return yOffset;
            }
            set
            {
                yOffset = value;
            }
        }

        [SerializeField]
        private bool autoTangent;
        public bool AutoTangent
        {
            get
            {
                return autoTangent;
            }
            set
            {
                autoTangent = value;
            }
        }
    }
}
