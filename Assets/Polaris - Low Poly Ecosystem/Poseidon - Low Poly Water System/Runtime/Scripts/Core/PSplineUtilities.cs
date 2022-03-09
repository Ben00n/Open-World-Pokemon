using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon
{
    public static class PSplineUtilities
    {
        public static void WaterPivotToSplineCenter(PWater water)
        {
            PSpline spline = water.Spline;
            List<PSplineAnchor> anchors = spline.Anchors;
            if (anchors.Count == 0)
                return;

            Vector3 splineCenterLocal = Vector3.zero;
            for (int i = 0; i < anchors.Count; ++i)
            {
                splineCenterLocal += anchors[i].Position;
            }

            splineCenterLocal = splineCenterLocal / anchors.Count;
            for (int i = 0; i < anchors.Count; ++i)
            {
                anchors[i].Position -= splineCenterLocal;
            }

            List<PSplineSegment> segments = spline.Segments;
            for (int i = 0; i < segments.Count; ++i)
            {
                segments[i].StartTangent -= splineCenterLocal;
                segments[i].EndTangent -= splineCenterLocal;
            }
            Vector3 splineCenterWorld = water.transform.TransformPoint(splineCenterLocal);
            water.transform.position = splineCenterWorld;
        }
    }
}
