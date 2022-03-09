using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Poseidon;

namespace Pinwheel.Poseidon
{
    public class PHexMeshCreator : IPMeshCreator
    {
        private Vector2[,] grid;
        private PWater water;

        public void Create(PWater water)
        {
            this.water = water;
            Init();
            GenerateGrid();
            UpdateMesh();
        }

        private void Init()
        {
            int resolution = water.MeshResolution;
            int length = resolution + 1;
            grid = new Vector2[length, length];
        }

        private void GenerateGrid()
        {
            int length = grid.GetLength(0);
            int width = grid.GetLength(1);

            Vector2 p = Vector2.zero;
            for (int z = 0; z < length; ++z)
            {
                for (int x = 0; x < width; ++x)
                {
                    p.Set(
                        Mathf.InverseLerp(0, width - 1, x),
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
            for (int z = 0; z < length - 1; ++z)
            {
                for (int x = 0; x < width - 1; ++x)
                {
                    int lastIndex = vertices.Count;
                    triangles.Add(lastIndex + 0);
                    triangles.Add(lastIndex + 1);
                    triangles.Add(lastIndex + 2);
                    triangles.Add(lastIndex + 3);
                    triangles.Add(lastIndex + 4);
                    triangles.Add(lastIndex + 5);

                    bl.Set(Mathf.InverseLerp(0, width - 1, x), 0, Mathf.InverseLerp(0, length - 1, z), 0);
                    tl.Set(Mathf.InverseLerp(0, width - 1, x), 0, Mathf.InverseLerp(0, length - 1, z + 1), 0);
                    tr.Set(Mathf.InverseLerp(0, width - 1, x + 1), 0, Mathf.InverseLerp(0, length - 1, z + 1), 0);
                    br.Set(Mathf.InverseLerp(0, width - 1, x + 1), 0, Mathf.InverseLerp(0, length - 1, z), 0);

                    if (z % 2 == 0)
                    {
                        v0 = bl;
                        v1 = tl + hexOffset;
                        v2 = tr + hexOffset;
                        vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                        vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                        vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);

                        v0 = bl;
                        v1 = tr + hexOffset;
                        v2 = br;
                        vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                        vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                        vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);
                    }
                    else
                    {
                        v0 = bl + hexOffset;
                        v1 = tl;
                        v2 = br + hexOffset;
                        vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                        vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                        vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);

                        v0 = tr;
                        v1 = br + hexOffset;
                        v2 = tl;
                        vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                        vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                        vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);
                    }
                }
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
            bounds.extents = new Vector3(bounds.extents.x, (bounds.extents.x + bounds.extents.z) * 0.5f, bounds.extents.z);
            m.bounds = bounds;
            PUtilities.DoubleMeshBounds(m);
        }
    }
}
