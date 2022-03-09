using UnityEngine;
using System;

namespace Pinwheel.Poseidon
{
    [System.Serializable]
    public class PSplineSegment : IDisposable
    {
        [SerializeField]
        private int startIndex;
        public int StartIndex
        {
            get
            {
                return startIndex;
            }
            set
            {
                startIndex = value;
            }
        }

        [SerializeField]
        private int endIndex;
        public int EndIndex
        {
            get
            {
                return endIndex;
            }
            set
            {
                endIndex = value;
            }
        }

        [SerializeField]
        private Vector3 startTangent;
        public Vector3 StartTangent
        {
            get
            {
                return startTangent;
            }
            set
            {
                startTangent = value;
            }
        }

        [SerializeField]
        private Vector3 endTangent;
        public Vector3 EndTangent
        {
            get
            {
                return endTangent;
            }
            set
            {
                endTangent = value;
            }
        }

        [SerializeField]
        private float resolutionMultiplierY = 1;
        public float ResolutionMultiplierY
        {
            get
            {
                resolutionMultiplierY = Mathf.Clamp(resolutionMultiplierY, 0f, 2f);
                return resolutionMultiplierY;
            }
            set
            {
                resolutionMultiplierY = Mathf.Clamp(value, 0f, 2f);
            }
        }

        [SerializeField]
        private Mesh mesh;
        public Mesh Mesh
        {
            get
            {
                if (mesh == null)
                {
                    mesh = new Mesh();
                    mesh.name = "Spline Segment";
                    mesh.MarkDynamic();
                }
                return mesh;
            }
        }

        public void Dispose()
        {
            if (mesh != null)
            {
                PUtilities.DestroyObject(mesh);
            }
        }

#if GRIFFIN_2020
        public static explicit operator PSplineSegment(Pinwheel.Griffin.SplineTool.GSplineSegment s)
        {
            PSplineSegment segment = new PSplineSegment();
            segment.StartIndex = s.StartIndex;
            segment.EndIndex = s.EndIndex;
            segment.StartTangent = s.StartTangent;
            segment.EndTangent = s.EndTangent;
            segment.ResolutionMultiplierY = 1;
            return segment;
        }
#endif
    }
}
