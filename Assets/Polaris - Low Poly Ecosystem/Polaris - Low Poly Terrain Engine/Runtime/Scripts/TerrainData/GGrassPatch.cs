#if GRIFFIN
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Pinwheel.Griffin.Rendering;
using Pinwheel.Griffin.Compression;
using System;
using UnityEngine.Serialization;

namespace Pinwheel.Griffin
{
    [System.Serializable]
    public class GGrassPatch
    {
        [SerializeField]
        private GFoliage foliage;
        public GFoliage Foliage
        {
            get
            {
                return foliage;
            }
            private set
            {
                foliage = value;
            }
        }

        [SerializeField]
        private Vector2 index;
        public Vector2 Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

#pragma warning disable 0649
        //Old container that get serialized as raw values and should be retired for now
        //However, it should still be here to prevent data lost when upgrading from lower version
        //This container will be empty after the manual upgrade
        [SerializeField]
        private List<GGrassInstance> instances;
#pragma warning restore 0649

        internal int InstanceCount
        {
            get
            {
                if (instances_NonSerialized == null)
                    return 0;
                else
                    return instances_NonSerialized.Count;
            }
        }

        [NonSerialized]
        private List<GGrassInstance> instances_NonSerialized;
        internal List<GGrassInstance> Instances
        {
            get
            {
                if (instances_NonSerialized == null)
                {
                    instances_NonSerialized = new List<GGrassInstance>();
                }
                return instances_NonSerialized;
            }
            set
            {
                instances_NonSerialized = value;
            }
        }

        private bool requireFullUpdate;
        internal bool RequireFullUpdate
        {
            get
            {
                return requireFullUpdate;
            }
            set
            {
                requireFullUpdate = value;
            }
        }

        [SerializeField]
        internal Bounds bounds;
        public Bounds Bounds
        {
            get
            {
                return bounds;
            }
            private set
            {
                bounds = value;
            }
        }

        [SerializeField]
        private byte[] prototypeIndexSerializeData;
        [SerializeField]
        private byte[] positionSerializeData;
        [SerializeField]
        private byte[] rotationSerializeData;
        [SerializeField]
        private byte[] scaleSerializeData;

        internal GGrassPatch(GFoliage owner, int indexX, int indexY)
        {
            foliage = owner;
            Index = new Vector2(indexX, indexY);
        }

        public void Changed()
        {
            if (Foliage != null && Foliage.TerrainData != null)
            {
                Foliage.TerrainData.InvokeGrassChange(Index);
            }
        }

        public void AddInstances(IEnumerable<GGrassInstance> newInstances)
        {
            Instances.AddRange(newInstances);
            RecalculateBounds();
            Changed();
        }

        public int RemoveInstances(Predicate<GGrassInstance> match)
        {
            int count = Instances.RemoveAll(match);
            if (count > 0)
            {
                RecalculateBounds();
                Changed();
            }
            return count;
        }

        public void ClearInstances()
        {
            Instances.Clear();
            RecalculateBounds();
            Changed();
        }

        public Rect GetUvRange()
        {
            return GCommon.GetUvRange(foliage.PatchGridSize, (int)Index.x, (int)Index.y);
        }

        public void RecalculateBounds()
        {
            if (Instances.Count == 0)
            {
                Rect r = GetUvRange();
                Vector3 center = new Vector3(r.x, 0, r.y);
                Vector3 size = Vector3.zero;
                Bounds b = new Bounds();
                b.center = center;
                b.size = size;
                Bounds = b;
            }
            else
            {
                float minX = float.MaxValue;
                float maxX = float.MinValue;

                float minY = float.MaxValue;
                float maxY = float.MinValue;

                float minZ = float.MaxValue;
                float maxZ = float.MinValue;

                int instanceCount = Instances.Count;

                for (int i = 0; i < instanceCount; ++i)
                {
                    Vector3 pos = Instances[i].Position;

                    minX = Mathf.Min(minX, pos.x);
                    maxX = Mathf.Max(maxX, pos.x);

                    minY = Mathf.Min(minY, pos.y);
                    maxY = Mathf.Max(maxY, pos.y);

                    minZ = Mathf.Min(minZ, pos.z);
                    maxZ = Mathf.Max(maxZ, pos.z);
                }

                Vector3 p = Vector3.Lerp(
                    new Vector3(minX, minY, minZ),
                    new Vector3(maxX, maxY, maxZ),
                    0.5f);
                Vector3 s = new Vector3(
                    maxX - minX,
                    Mathf.Max(0.001f, maxY - minY),
                    maxZ - minZ);
                Bounds = new Bounds(p, s);
            }
        }

        internal void Serialize()
        {
            int[] protoIndices = new int[Instances.Count];
            float[] positions = new float[Instances.Count * 3];
            float[] rotations = new float[Instances.Count * 4];
            float[] scales = new float[Instances.Count * 3];
            for (int i = 0; i < Instances.Count; ++i)
            {
                GGrassInstance grass = Instances[i];
                protoIndices[i] = grass.PrototypeIndex;

                positions[i * 3 + 0] = grass.Position.x;
                positions[i * 3 + 1] = grass.Position.y;
                positions[i * 3 + 2] = grass.Position.z;

                rotations[i * 4 + 0] = grass.Rotation.x;
                rotations[i * 4 + 1] = grass.Rotation.y;
                rotations[i * 4 + 2] = grass.Rotation.z;
                rotations[i * 4 + 3] = grass.Rotation.w;

                scales[i * 3 + 0] = grass.Scale.x;
                scales[i * 3 + 1] = grass.Scale.y;
                scales[i * 3 + 2] = grass.Scale.z;
            }

            prototypeIndexSerializeData = new byte[Buffer.ByteLength(protoIndices)];
            Buffer.BlockCopy(protoIndices, 0, prototypeIndexSerializeData, 0, prototypeIndexSerializeData.Length);
            prototypeIndexSerializeData = GCompressor.Compress(prototypeIndexSerializeData);

            positionSerializeData = new byte[Buffer.ByteLength(positions)];
            Buffer.BlockCopy(positions, 0, positionSerializeData, 0, positionSerializeData.Length);
            positionSerializeData = GCompressor.Compress(positionSerializeData);

            rotationSerializeData = new byte[Buffer.ByteLength(rotations)];
            Buffer.BlockCopy(rotations, 0, rotationSerializeData, 0, rotationSerializeData.Length);
            rotationSerializeData = GCompressor.Compress(rotationSerializeData);

            scaleSerializeData = new byte[Buffer.ByteLength(scales)];
            Buffer.BlockCopy(scales, 0, scaleSerializeData, 0, scaleSerializeData.Length);
            scaleSerializeData = GCompressor.Compress(scaleSerializeData);

            GCompressor.CleanUp();
        }

        internal void Deserialize()
        {
            if (prototypeIndexSerializeData != null &&
                positionSerializeData != null &&
                rotationSerializeData != null &&
                scaleSerializeData != null)
            {
                prototypeIndexSerializeData = GCompressor.Decompress(prototypeIndexSerializeData);
                positionSerializeData = GCompressor.Decompress(positionSerializeData);
                rotationSerializeData = GCompressor.Decompress(rotationSerializeData);
                scaleSerializeData = GCompressor.Decompress(scaleSerializeData);

                int[] indices = new int[prototypeIndexSerializeData.Length / sizeof(int)];
                float[] positions = new float[positionSerializeData.Length / sizeof(float)];
                float[] rotations = new float[rotationSerializeData.Length / sizeof(float)];
                float[] scales = new float[scaleSerializeData.Length / sizeof(float)];

                Buffer.BlockCopy(prototypeIndexSerializeData, 0, indices, 0, prototypeIndexSerializeData.Length);
                Buffer.BlockCopy(positionSerializeData, 0, positions, 0, positionSerializeData.Length);
                Buffer.BlockCopy(rotationSerializeData, 0, rotations, 0, rotationSerializeData.Length);
                Buffer.BlockCopy(scaleSerializeData, 0, scales, 0, scaleSerializeData.Length);

                Instances.Clear();
                for (int i = 0; i < indices.Length; ++i)
                {
                    GGrassInstance grass = GGrassInstance.Create(indices[i]);
                    grass.Position = new Vector3(
                        positions[i * 3 + 0],
                        positions[i * 3 + 1],
                        positions[i * 3 + 2]);
                    grass.Rotation = new Quaternion(
                        rotations[i * 4 + 0],
                        rotations[i * 4 + 1],
                        rotations[i * 4 + 2],
                        rotations[i * 4 + 3]);
                    grass.Scale = new Vector3(
                        scales[i * 3 + 0],
                        scales[i * 3 + 1],
                        scales[i * 3 + 2]);
                    Instances.Add(grass);
                }

                GCompressor.CleanUp();
            }
        }

        internal void UpgradeSerializeVersion()
        {
            //v250: Compressed serialization
            if (instances != null && instances.Count > 0)
            {
                if (instances_NonSerialized != null)
                {
                    instances_NonSerialized.AddRange(instances);
                    RecalculateBounds();
                    Changed();
                }
            }
        }

        public int GetMemStats()
        {
            int mem = 0;
            if (prototypeIndexSerializeData != null)
            {
                mem += prototypeIndexSerializeData.Length;
                mem += positionSerializeData.Length;
                mem += rotationSerializeData.Length;
                mem += scaleSerializeData.Length;
            }
            return mem;
        }

        public NativeArray<Vector2> GetGrassPositionArray(Allocator allocator = Allocator.TempJob)
        {
            int instanceCount = InstanceCount;
            List<GGrassInstance> instances = Instances;
            NativeArray<Vector2> positions = new NativeArray<Vector2>(instanceCount, allocator, NativeArrayOptions.UninitializedMemory);
            Vector2 pos = Vector2.zero;

            for (int i = 0; i < instanceCount; ++i)
            {
                GGrassInstance g = instances[i];
                pos.Set(g.position.x, g.position.z);
                positions[i] = pos;
            }
            return positions;
        }
    }
}
#endif
