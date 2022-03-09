using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Poseidon;

namespace Pinwheel.Poseidon
{
    public class PAreaMeshCreator : IPMeshCreator
    {
        private const int MASK_RESOLUTION = 100;

        private PWater water;
        private float[] mask;

        private float minX;
        private float maxX;
        private float minY;
        private float maxY;

        private Vector2[,] grid;

        public void Create(PWater water)
        {
            if (water.AreaMeshAnchors.Count < 3)
                return;

            this.water = water;
            CalculateBoundaries();
            GenerateMask();
            GenerateGrid();
            UpdateMesh();
        }

        private void GenerateMask()
        {
            mask = new float[MASK_RESOLUTION * MASK_RESOLUTION];

            List<Vector2> segments = new List<Vector2>();
            GetSegments(segments);

            float lineY = 0;
            List<float> intersectX = new List<float>();
            for (int i = 0; i < MASK_RESOLUTION; ++i)
            {
                intersectX.Clear();
                lineY = i * 1.0f / MASK_RESOLUTION;
                FindIntersects(segments, intersectX, lineY);
                FillLine(i, intersectX);
            }

            //DrawDebug();
        }

        private void CalculateBoundaries()
        {
            List<Vector3> anchors = water.AreaMeshAnchors;
            minX = float.MaxValue;
            maxX = float.MinValue;
            minY = float.MaxValue;
            maxY = float.MinValue;
            for (int i = 0; i < anchors.Count; ++i)
            {
                minX = Mathf.Min(minX, anchors[i].x);
                maxX = Mathf.Max(maxX, anchors[i].x);
                minY = Mathf.Min(minY, anchors[i].z);
                maxY = Mathf.Max(maxY, anchors[i].z);
            }
        }

        private void GetSegments(List<Vector2> segments)
        {
            List<Vector3> anchors = water.AreaMeshAnchors;
            for (int i = 0; i < anchors.Count; ++i)
            {
                segments.Add(new Vector2(
                    Mathf.InverseLerp(minX, maxX, anchors[i].x),
                    Mathf.InverseLerp(minY, maxY, anchors[i].z)));
            }
            segments.Add(new Vector2(
                    Mathf.InverseLerp(minX, maxX, anchors[0].x),
                    Mathf.InverseLerp(minY, maxY, anchors[0].z)));
        }

        private void FindIntersects(List<Vector2> segments, List<float> intersectX, float lineY)
        {
            for (int i = 0; i < segments.Count - 1; ++i)
            {
                Vector2 s0 = segments[i];
                Vector2 s1 = segments[i + 1];
                Vector2 inter;
                if (PGeometryUtilities.IsIntersectHorizontalLine(
                    s0.x, s0.y,
                    s1.x, s1.y,
                    lineY,
                    out inter))
                {
                    intersectX.Add(inter.x);
                }
            }
        }

        private void FillLine(int lineIndex, List<float> intersectX)
        {
            intersectX.Sort();

            List<int> columnIndices = new List<int>();
            for (int i = 0; i < intersectX.Count; ++i)
            {
                columnIndices.Add((int)(intersectX[i] * MASK_RESOLUTION));
            }

            int pairCount = columnIndices.Count / 2;
            for (int p = 0; p < pairCount; ++p)
            {
                int c0 = columnIndices[p * 2 + 0];
                int c1 = columnIndices[p * 2 + 1];
                for (int c = c0; c <= c1; ++c)
                {
                    mask[PUtilities.To1DIndex(c, lineIndex, MASK_RESOLUTION)] = 1.0f;
                }
            }
        }

        //private void DrawDebug()
        //{
        //    water.debugTexture = new Texture2D(MASK_RESOLUTION, MASK_RESOLUTION);
        //    Color[] colors = new Color[MASK_RESOLUTION * MASK_RESOLUTION];
        //    for (int i = 0; i < mask.Length; ++i)
        //    {
        //        colors[i] = mask[i] == 1 ? Color.white : Color.black;
        //    }
        //    water.debugTexture.SetPixels(colors);
        //    water.debugTexture.Apply();
        //}

        private void GenerateGrid()
        {
            int resolution = water.MeshResolution;
            int length = resolution + 1;
            grid = new Vector2[length, length];

            Vector2 p = Vector2.zero;
            for (int z = 0; z < length; ++z)
            {
                for (int x = 0; x < length; ++x)
                {
                    p.Set(
                        Mathf.InverseLerp(0, length - 1, x),
                        Mathf.InverseLerp(0, length - 1, z));
                    grid[z, x] = p;
                }
            }
        }

        private void UpdateMesh()
        {
            //vertices
            int length = grid.GetLength(0);
            int width = grid.GetLength(1);
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector4> uvs0 = new List<Vector4>(); //contain neighbor vertex position, for normal re-construction
            List<Color> colors = new List<Color>(); //contain neighbor vertex position, for normal re-construction

            Vector4 bl = Vector4.zero;
            Vector4 tl = Vector4.zero;
            Vector4 tr = Vector4.zero;
            Vector4 br = Vector4.zero;
            Vector4 v0 = Vector4.zero;
            Vector4 v1 = Vector4.zero;
            Vector4 v2 = Vector4.zero;
            Vector4 hexOffset = new Vector4(-0.5f / width, 0, 0, 0);
            int lastIndex = 0;
            for (int z = 0; z < length - 1; ++z)
            {
                for (int x = 0; x < width - 1; ++x)
                {
                    bl.Set(Mathf.InverseLerp(0, width - 1, x), 0, Mathf.InverseLerp(0, length - 1, z), 0);
                    tl.Set(Mathf.InverseLerp(0, width - 1, x), 0, Mathf.InverseLerp(0, length - 1, z + 1), 0);
                    tr.Set(Mathf.InverseLerp(0, width - 1, x + 1), 0, Mathf.InverseLerp(0, length - 1, z + 1), 0);
                    br.Set(Mathf.InverseLerp(0, width - 1, x + 1), 0, Mathf.InverseLerp(0, length - 1, z), 0);

                    if (z % 2 == 0)
                    {
                        v0 = bl;
                        v1 = tl + hexOffset;
                        v2 = tr + hexOffset;
                        if (!Clip(v0, v1, v2))
                        {
                            lastIndex = vertices.Count;
                            triangles.Add(lastIndex + 0);
                            triangles.Add(lastIndex + 1);
                            triangles.Add(lastIndex + 2);
                            vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                            vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                            vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);
                        }

                        v0 = bl;
                        v1 = tr + hexOffset;
                        v2 = br;
                        if (!Clip(v0, v1, v2))
                        {
                            lastIndex = vertices.Count;
                            triangles.Add(lastIndex + 0);
                            triangles.Add(lastIndex + 1);
                            triangles.Add(lastIndex + 2);
                            vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                            vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                            vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);
                        }
                    }
                    else
                    {
                        v0 = bl + hexOffset;
                        v1 = tl;
                        v2 = br + hexOffset;
                        if (!Clip(v0, v1, v2))
                        {
                            lastIndex = vertices.Count;
                            triangles.Add(lastIndex + 0);
                            triangles.Add(lastIndex + 1);
                            triangles.Add(lastIndex + 2);
                            vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                            vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                            vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);
                        }

                        v0 = tr;
                        v1 = br + hexOffset;
                        v2 = tl;
                        if (!Clip(v0, v1, v2))
                        {
                            lastIndex = vertices.Count;
                            triangles.Add(lastIndex + 0);
                            triangles.Add(lastIndex + 1);
                            triangles.Add(lastIndex + 2);
                            vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                            vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                            vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);
                        }
                    }
                }
            }

            for (int i=0;i<vertices.Count;++i)
            {
                vertices[i] = Remap(vertices[i]);
                uvs0[i] = Remap(uvs0[i]);
                colors[i] = Remap(colors[i]);
            }

            Mesh m = water.Mesh;
            m.Clear();
            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);
            m.SetUVs(0, uvs0);
            m.SetColors(colors);

            m.RecalculateBounds();
            m.RecalculateNormals();
            //m.RecalculateTangents();
            m.name = "Water Mesh"; 
            
            Bounds bounds = m.bounds;
            bounds.extents = new Vector3(bounds.extents.x, 1, bounds.extents.z);
            m.bounds = bounds;

            PUtilities.DoubleMeshBounds(m);
        }

        private bool Clip(Vector4 v0, Vector4 v1, Vector4 v2)
        {
            float mask0 = PUtilities.GetValueBilinear(mask, MASK_RESOLUTION, MASK_RESOLUTION, GetMaskUV(v0));
            float mask1 = PUtilities.GetValueBilinear(mask, MASK_RESOLUTION, MASK_RESOLUTION, GetMaskUV(v1));
            float mask2 = PUtilities.GetValueBilinear(mask, MASK_RESOLUTION, MASK_RESOLUTION, GetMaskUV(v2));
            return mask0 == 0 && mask1 == 0 && mask2 == 0;
        }

        private Vector2 GetMaskUV(Vector4 v)
        {
            return new Vector2(Mathf.Clamp01(v.x), Mathf.Clamp01(v.z));
        }

        private Vector4 Remap(Vector4 v)
        {
            return new Vector4(
                Mathf.Lerp(minX, maxX, v.x),
                0,
                Mathf.Lerp(minY, maxY, v.z),
                0);
        }
    }
}
