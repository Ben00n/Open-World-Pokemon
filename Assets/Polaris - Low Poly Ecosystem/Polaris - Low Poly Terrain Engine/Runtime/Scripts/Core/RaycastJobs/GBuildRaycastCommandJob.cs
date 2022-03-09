#if GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.Collections;
using Unity.Burst;

namespace Pinwheel.Griffin
{
#if GRIFFIN_BURST
    [BurstCompile(CompileSynchronously = false)]
#endif
    public struct GBuildRaycastCommandJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<Rect> dirtyRects;
        [ReadOnly]
        public NativeArray<Vector2> positions;

        [WriteOnly]
        public NativeArray<RaycastCommand> commands;

        public int mask;
        public int maxHit;
        public Vector3 terrainPosition;
        public Vector3 terrainSize;

        public void Execute(int index)
        {
            bool isDirty = false;
            Vector2 pos = positions[index];
            for (int i = 0; i < dirtyRects.Length; ++i)
            {
                if (dirtyRects[i].Contains(pos))
                {
                    isDirty = true;
                    break;
                }
            }

            Vector3 from = new Vector3(terrainPosition.x + pos.x * terrainSize.x, 10000, terrainPosition.z + pos.y * terrainSize.z);
            RaycastCommand cmd = new RaycastCommand(from, Vector3.down, float.MaxValue, isDirty ? mask : 0, maxHit);
            commands[index] = cmd;
        }
    }
}
#endif
