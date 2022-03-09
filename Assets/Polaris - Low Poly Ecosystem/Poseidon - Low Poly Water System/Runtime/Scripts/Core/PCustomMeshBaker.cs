using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.Poseidon;

namespace Pinwheel.Poseidon
{
    public class PCustomMeshBaker : IPMeshCreator
    {
        private PWater water;
        private Vector3[] verts;
        private int[] tris;
        private Bounds meshBound;

        public void Create(PWater profile)
        {
            this.water = profile;
            Init();
            Bake();
        }

        private void Init()
        {
            Mesh srcMesh = water.SourceMesh;
            verts = srcMesh.vertices;
            tris = srcMesh.triangles;
            srcMesh.RecalculateBounds();
            meshBound = srcMesh.bounds;
        }

        private void Bake()
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector4> uvs0 = new List<Vector4>(); //contain neighbor vertex position, for normal re-construction
            List<Color> colors = new List<Color>(); //contain neighbor vertex position, for normal re-construction
            List<Vector3> normals = new List<Vector3>();

            Vector4 v0 = Vector4.zero;
            Vector4 v1 = Vector4.zero;
            Vector4 v2 = Vector4.zero;

            int i0, i1, i2;

            int trisCount = tris.Length / 3;
            for (int i = 0; i < trisCount; ++i)
            {
                i0 = i * 3 + 0;
                i1 = i * 3 + 1;
                i2 = i * 3 + 2;

                v0 = RemapVertex(ref verts[tris[i0]], ref meshBound);
                v1 = RemapVertex(ref verts[tris[i1]], ref meshBound);
                v2 = RemapVertex(ref verts[tris[i2]], ref meshBound);

                vertices.Add(v0); uvs0.Add(v1); colors.Add(v2);
                vertices.Add(v1); uvs0.Add(v2); colors.Add(v0);
                vertices.Add(v2); uvs0.Add(v0); colors.Add(v1);

                triangles.Add(i0);
                triangles.Add(i1);
                triangles.Add(i2);
            }

            Mesh m = water.Mesh;
            m.Clear();
            m.SetVertices(vertices);
            m.SetTriangles(triangles, 0);
            m.SetUVs(0, uvs0);
            m.SetColors(colors);
            m.name = "Water Mesh";
            m.RecalculateBounds();

            PUtilities.DoubleMeshBounds(m);
        }

        private Vector3 RemapVertex(ref Vector3 v, ref Bounds bounds)
        {
            Vector3 n = Vector3.zero;
            n.Set(
                Mathf.InverseLerp(bounds.min.x, bounds.max.x, v.x) - 0.5f,
                Mathf.InverseLerp(bounds.min.y, bounds.max.y, v.y) - 0.5f,
                Mathf.InverseLerp(bounds.min.z, bounds.max.z, v.z) - 0.5f);
            return n;
        }
    }
}
