#if GRIFFIN
using System.Collections.Generic;
using UnityEngine;

namespace Pinwheel.Griffin.PaintTool
{
    public struct GObjectPainterArgs
    {
        public Vector3 HitPoint { get; set; }
        public Collider Collider { get; set; }
        public Transform Transform { get; set; }
        public Vector2 UV { get; set; }
        public int TriangleIndex { get; set; }
        public Vector3 BarycentricCoord { get; set; }
        public float Distance { get; set; }
        public Vector3 Normal { get; set; }
        public Vector2 LightMapCoord { get; set; }
        public Rect UvRect { get; set; }
        public Vector3[] WorldPointCorners { get; internal set; }
        public Texture2D Mask { get; set; }
        public bool EnableTerrainMask { get; set; }
        public float Radius { get; internal set; }
        public float Rotation { get; internal set; }
        public int Density { get; internal set; }
        public float EraseRatio { get; internal set; }
        public float ScaleStrength { get; internal set; }
        public List<GameObject> Prototypes { get; set; }
        public List<int> PrototypeIndices { get; set; }
        public GSpawnFilter[] Filters { get; set; }
        public string CustomArgs { get; internal set; }
        public GPainterMouseEventType MouseEventType { get; set; }
        public GPainterActionType ActionType { get; set; }
    }
}
#endif
