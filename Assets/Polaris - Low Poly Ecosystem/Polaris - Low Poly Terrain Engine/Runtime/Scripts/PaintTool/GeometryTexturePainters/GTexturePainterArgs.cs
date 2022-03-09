#if GRIFFIN
using UnityEngine;
using System.Collections.Generic;

namespace Pinwheel.Griffin.PaintTool
{
    public struct GTexturePainterArgs
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
        public Vector3[] WorldPointCorners { get; set; }
        public Texture BrushMask { get; set; }
        public float Radius { get; set; }
        public float Rotation { get; set; }
        public float Opacity { get; set; }
        public Color Color { get; set; }
        public int SplatIndex { get; set; }
        public Vector3 SamplePoint { get; set; }
        public string CustomArgs { get; set; }
        public GPainterMouseEventType MouseEventType { get; set; }
        public GPainterActionType ActionType { get; set; }
        public bool ForceUpdateGeometry { get; set; }
        public bool EnableTerrainMask { get; set; }
        public GConditionalPaintingConfigs ConditionalPaintingConfigs { get; set; }
    }
}
#endif
