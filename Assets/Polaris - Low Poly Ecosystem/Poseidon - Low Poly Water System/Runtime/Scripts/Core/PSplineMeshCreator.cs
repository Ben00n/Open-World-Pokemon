using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Poseidon;

namespace Pinwheel.Poseidon
{
    public class PSplineMeshCreator : IPMeshCreator
    {
        private PWater water;

        public void Create(PWater water)
        {
            List<PSplineSegment> segments = water.Spline.Segments;
            for (int i = 0; i < segments.Count; ++i)
            {
                Create(water, i);
            }
        }

        public void Create(PWater water, int segmentIndex)
        {
            this.water = water;

            List<Vector3> vertices = new List<Vector3>();
            List<Vector4> uvs0 = new List<Vector4>();
            List<Vector4> uvs1 = new List<Vector4>();
            List<Color> colors = new List<Color>();
            List<int> triangles = new List<int>();

            PSpline spline = water.Spline;
            PSplineSegment s = water.Spline.Segments[segmentIndex];

            Vector3 pos0, pos1;
            Quaternion rotation0, rotation1;
            Vector3 scale0, scale1;
            Matrix4x4 matrix0, matrix1;

            Vector4 bl, tl, tr, br;
            Vector4 v0, v1, v2, v3;
            Vector4 flow0, flow1;
            int currentVertexCount;

            float halfWidth = water.SplineWidth * 0.5f;
            int resY = Mathf.RoundToInt(water.SplineResolutionY * s.ResolutionMultiplierY);
            resY = Mathf.Clamp(resY, 2, 100);
            if (resY % 2 == 1)
            {
                resY -= 1;
            }
            int resX = water.SplineResolutionX;
            float xStep = 1.0f / resX;
            float yStep = 1.0f / resY;
            float x0, x1;
            float y0, y1;
            for (int yIndex = 0; yIndex < resY; ++yIndex)
            {
                y0 = yIndex * yStep;
                pos0 = spline.EvaluatePosition(segmentIndex, y0);
                rotation0 = spline.EvaluateRotation(segmentIndex, y0);
                scale0 = spline.EvaluateScale(segmentIndex, y0);
                matrix0 = Matrix4x4.TRS(pos0, rotation0, scale0);

                y1 = (yIndex + 1) * yStep;
                pos1 = spline.EvaluatePosition(segmentIndex, y1);
                rotation1 = spline.EvaluateRotation(segmentIndex, y1);
                scale1 = spline.EvaluateScale(segmentIndex, y1);
                matrix1 = Matrix4x4.TRS(pos1, rotation1, scale1);

                bl = matrix0.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                tl = matrix1.MultiplyPoint(new Vector3(-halfWidth, 0, 0));
                tr = matrix1.MultiplyPoint(new Vector3(halfWidth, 0, 0));
                br = matrix0.MultiplyPoint(new Vector3(halfWidth, 0, 0));

                for (int xIndex = 0; xIndex < resX; ++xIndex)
                {
                    x0 = xIndex * xStep;
                    x1 = (xIndex + 1) * xStep;

                    v0 = Vector4.Lerp(bl, br, x0);
                    v1 = Vector4.Lerp(tl, tr, x0);
                    v2 = Vector4.Lerp(tl, tr, x1);
                    v3 = Vector4.Lerp(bl, br, x1);

                    currentVertexCount = vertices.Count;
                    triangles.Add(currentVertexCount + 0);
                    triangles.Add(currentVertexCount + 1);
                    triangles.Add(currentVertexCount + 2);
                    triangles.Add(currentVertexCount + 3);
                    triangles.Add(currentVertexCount + 4);
                    triangles.Add(currentVertexCount + 5);

                    flow0 = matrix0.MultiplyVector(Vector3.forward*2);
                    flow1 = matrix1.MultiplyVector(Vector3.forward*2);

                    if ((xIndex + yIndex) % 2 == 0)
                    {
                        BakeData(vertices, uvs0, colors, uvs1, v0, v1, v2, flow0, flow1, flow1);
                        BakeData(vertices, uvs0, colors, uvs1, v1, v2, v0, flow1, flow1, flow0);
                        BakeData(vertices, uvs0, colors, uvs1, v2, v0, v1, flow1, flow0, flow1);

                        BakeData(vertices, uvs0, colors, uvs1, v2, v3, v0, flow1, flow0, flow0);
                        BakeData(vertices, uvs0, colors, uvs1, v3, v0, v2, flow0, flow0, flow1);
                        BakeData(vertices, uvs0, colors, uvs1, v0, v2, v3, flow0, flow1, flow0);
                    }
                    else
                    {
                        BakeData(vertices, uvs0, colors, uvs1, v0, v1, v3, flow0, flow1, flow0);
                        BakeData(vertices, uvs0, colors, uvs1, v1, v3, v0, flow1, flow0, flow0);
                        BakeData(vertices, uvs0, colors, uvs1, v3, v0, v1, flow0, flow0, flow1);

                        BakeData(vertices, uvs0, colors, uvs1, v2, v3, v1, flow1, flow0, flow1);
                        BakeData(vertices, uvs0, colors, uvs1, v3, v1, v2, flow0, flow1, flow1);
                        BakeData(vertices, uvs0, colors, uvs1, v1, v2, v3, flow1, flow1, flow0);
                    }
                }
            }

            Mesh m = s.Mesh;
            m.Clear();
            m.SetVertices(vertices);
            m.SetUVs(0, uvs0);
            m.SetUVs(1, uvs1);
            m.SetColors(colors);
            m.SetTriangles(triangles, 0);
            m.RecalculateBounds();
            PUtilities.DoubleMeshBounds(m);
        }

        private void BakeData(
            List<Vector3> vertices, List<Vector4> uvs0, List<Color> colors, List<Vector4> uvs1,
            Vector4 v0, Vector4 v1, Vector4 v2, 
            Vector4 flow0, Vector4 flow1, Vector4 flow2)
        {
            Vector3 vertex = new Vector3(v0.x, v0.y, v0.z);
            Vector4 uv0 = new Vector4(v1.x, v1.y, v1.z, flow0.x);
            Color color = new Color(v2.x, v2.y, v2.z, flow0.z);
            Vector4 uv1 = new Vector4(flow1.x, flow1.z, flow2.x, flow2.z);

            vertices.Add(vertex);
            uvs0.Add(uv0);
            colors.Add(color);
            uvs1.Add(uv1);            
        }
    }
}
