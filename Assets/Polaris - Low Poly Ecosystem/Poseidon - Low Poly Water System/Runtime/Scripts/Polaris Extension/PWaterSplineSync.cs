#if GRIFFIN && GRIFFIN_2020
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Griffin.SplineTool;
using Pinwheel.Griffin;

namespace Pinwheel.Poseidon
{
    [GDisplayName("Poseidon/Water Spline Sync")]
    [ExecuteInEditMode]
    public class PWaterSplineSync : GSplineModifier
    {
        [SerializeField]
        private PWater water;
        public PWater Water
        {
            get
            {
                return water;
            }
            set
            {
                water = value;
            }
        }

        [SerializeField]
        private float heightOffset;
        public float HeightOffset
        {
            get
            {
                return heightOffset;
            }
            set
            {
                heightOffset = value;
            }
        }

        private void OnEnable()
        {
            GSplineCreator.Editor_SplineChanged += OnSplineChanged;
        }

        private void OnDisable()
        {
            GSplineCreator.Editor_SplineChanged -= OnSplineChanged;
        }

        private void OnValidate()
        {
            Apply();
        }

        private void OnSplineChanged(GSplineCreator creator)
        {
            if (creator == this.SplineCreator)
            {
                Apply();
            }
        }

        public override void Apply()
        {
            if (SplineCreator == null && Water == null)
                return;
            if (Water.MeshType != PWaterMeshType.Spline)
                return;

            GSpline gSpline = SplineCreator.Spline;
            PSpline pSpline = Water.Spline;

            pSpline.Dispose();
            pSpline.Anchors.Clear();
            pSpline.Segments.Clear();

            for (int i = 0; i < gSpline.Anchors.Count; ++i)
            {
                PSplineAnchor a = (PSplineAnchor)gSpline.Anchors[i];
                a.Position += Vector3.up * HeightOffset;
                pSpline.Anchors.Add(a);
            }

            for (int i = 0; i < gSpline.Segments.Count; ++i)
            {
                PSplineSegment s = (PSplineSegment)gSpline.Segments[i];
                s.StartTangent += Vector3.up * HeightOffset;
                s.EndTangent += Vector3.up * HeightOffset;
                pSpline.Segments.Add(s);
            }

            Water.transform.position = transform.position;
            Water.transform.rotation = transform.rotation;
            Water.transform.localScale = transform.localScale;
            Water.SplineWidth = SplineCreator.Width + SplineCreator.FalloffWidth * 2;
            Water.GenerateSplineMesh();
            Water.ReCalculateBounds();
        }
    }
}
#endif