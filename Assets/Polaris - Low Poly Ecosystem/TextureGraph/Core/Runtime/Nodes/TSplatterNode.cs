using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using System;
using Random = System.Random;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Splatter",
        CreationMenu = "Shapes & Patterns/Splatter",
        Icon = "TextureGraph/NodeIcons/Splatter",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.yon1t3utx822")]
    public class TSplatterNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Splatter";
            public static readonly int SHAPE_TEX = Shader.PropertyToID("_ShapeTex");
            public static readonly int SHAPE_ALPHA_TEX = Shader.PropertyToID("_ShapeAlpha");
            public static readonly int HUE_SHIFT = Shader.PropertyToID("_HueShift");
            public static readonly int SATURATION_SHIFT = Shader.PropertyToID("_SaturationShift");
            public static readonly int LIGHTNESS_SHIFT = Shader.PropertyToID("_LightnessShift");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
            public static readonly int TEXTURE_SIZE = 256;

            public static readonly Vector2[] VERTICES = new Vector2[] { new Vector2(-0.5f, -0.5f), new Vector2(-0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0.5f, -0.5f) };
            public static readonly Vector2[] UVS = new Vector2[] { Vector2.zero, Vector2.up, Vector2.one, Vector2.right };
        }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 0);

        public readonly TSlot shapeSlot = new TSlot("Shape", TSlotType.Input, TSlotDataType.RGBA, 1);
        public readonly TSlot shapeAlphaSlot = new TSlot("Shape Alpha", TSlotType.Input, TSlotDataType.Gray, 2);
        public readonly TSlot offsetMapSlot = new TSlot("Offset Map", TSlotType.Input, TSlotDataType.Gray, 3);
        public readonly TSlot rotationMapSlot = new TSlot("Rotation Map", TSlotType.Input, TSlotDataType.Gray, 4);
        public readonly TSlot scaleMapSlot = new TSlot("Scale Map", TSlotType.Input, TSlotDataType.Gray, 5);
        public readonly TSlot maskMapSlot = new TSlot("Mask Map", TSlotType.Input, TSlotDataType.Gray, 6);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TIntParameter tileX;
        public TIntParameter TileX
        {
            get
            {
                return tileX;
            }
            set
            {
                tileX.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TIntParameter tileY;
        public TIntParameter TileY
        {
            get
            {
                return tileY;
            }
            set
            {
                tileY.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TIntParameter instancePerTile;
        public TIntParameter InstancePerTile
        {
            get
            {
                return instancePerTile;
            }
            set
            {
                instancePerTile.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TVector2Parameter baseOffset;
        public TVector2Parameter BaseOffset
        {
            get
            {
                return baseOffset;
            }
            set
            {
                baseOffset.value = new Vector2(
                    Mathf.Clamp(value.value.x, -1, 1),
                    Mathf.Clamp(value.value.y, -1, 1));
            }
        }

        [SerializeField]
        private TFloatParameter offsetMinX;
        public TFloatParameter OffsetMinX
        {
            get
            {
                return offsetMinX;
            }
            set
            {
                offsetMinX = value;
            }
        }

        [SerializeField]
        private TFloatParameter offsetMaxX;
        public TFloatParameter OffsetMaxX
        {
            get
            {
                return offsetMaxX;
            }
            set
            {
                offsetMaxX = value;
            }
        }

        [SerializeField]
        private TFloatParameter offsetMinY;
        public TFloatParameter OffsetMinY
        {
            get
            {
                return offsetMinY;
            }
            set
            {
                offsetMinY = value;
            }
        }

        [SerializeField]
        private TFloatParameter offsetMaxY;
        public TFloatParameter OffsetMaxY
        {
            get
            {
                return offsetMaxY;
            }
            set
            {
                offsetMaxY = value;
            }
        }

        [SerializeField]
        private TFloatParameter offsetMapMultiplier;
        public TFloatParameter OffsetMapMultiplier
        {
            get
            {
                return offsetMapMultiplier;
            }
            set
            {
                offsetMapMultiplier.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TIntParameter offsetRandomSeed;
        public TIntParameter OffsetRandomSeed
        {
            get
            {
                return offsetRandomSeed;
            }
            set
            {
                offsetRandomSeed = value;
            }
        }

        [SerializeField]
        private TFloatParameter baseRotation;
        public TFloatParameter BaseRotation
        {
            get
            {
                return baseRotation;
            }
            set
            {
                baseRotation = value;
            }
        }

        [SerializeField]
        private TFloatParameter rotationMin;
        public TFloatParameter RotationMin
        {
            get
            {
                return rotationMin;
            }
            set
            {
                rotationMin.value = Mathf.Clamp(value.value, -360, 360);
            }
        }

        [SerializeField]
        private TFloatParameter rotationMax;
        public TFloatParameter RotationMax
        {
            get
            {
                return rotationMax;
            }
            set
            {
                rotationMax.value = Mathf.Clamp(value.value, -360, 360);
            }
        }

        [SerializeField]
        private TFloatParameter rotationMapMultiplier;
        public TFloatParameter RotationMapMultiplier
        {
            get
            {
                return rotationMapMultiplier;
            }
            set
            {
                rotationMapMultiplier.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TIntParameter rotationRandomSeed;
        public TIntParameter RotationRandomSeed
        {
            get
            {
                return rotationRandomSeed;
            }
            set
            {
                rotationRandomSeed = value;
            }
        }

        [SerializeField]
        private TVector2Parameter baseScale;
        public TVector2Parameter BaseScale
        {
            get
            {
                return baseScale;
            }
            set
            {
                baseScale = value;
            }
        }

        [SerializeField]
        private TFloatParameter scaleMinX;
        public TFloatParameter ScaleMinX
        {
            get
            {
                return scaleMinX;
            }
            set
            {
                scaleMinX = value;
            }
        }

        [SerializeField]
        private TFloatParameter scaleMaxX;
        public TFloatParameter ScaleMaxX
        {
            get
            {
                return scaleMaxX;
            }
            set
            {
                scaleMaxX = value;
            }
        }

        [SerializeField]
        private TFloatParameter scaleMinY;
        public TFloatParameter ScaleMinY
        {
            get
            {
                return scaleMinY;
            }
            set
            {
                scaleMinY = value;
            }
        }

        [SerializeField]
        private TFloatParameter scaleMaxY;
        public TFloatParameter ScaleMaxY
        {
            get
            {
                return scaleMaxY;
            }
            set
            {
                scaleMaxY = value;
            }
        }

        [SerializeField]
        private TFloatParameter scaleMapMultiplier;
        public TFloatParameter ScaleMapMultiplier
        {
            get
            {
                return scaleMapMultiplier;
            }
            set
            {
                scaleMapMultiplier.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TIntParameter scaleRandomSeed;
        public TIntParameter ScaleRandomSeed
        {
            get
            {
                return scaleRandomSeed;
            }
            set
            {
                scaleRandomSeed = value;
            }
        }

        [SerializeField]
        private TColorParameter backgroundColor;
        public TColorParameter BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        [SerializeField]
        private TIntParameter hueVariation;
        public TIntParameter HueVariation
        {
            get
            {
                return hueVariation;
            }
            set
            {
                hueVariation.value = Mathf.Clamp(value.value, -180, 180);
            }
        }

        [SerializeField]
        private TIntParameter saturationVariation;
        public TIntParameter SaturationVariation
        {
            get
            {
                return saturationVariation;
            }
            set
            {
                saturationVariation.value = Mathf.Clamp(value.value, -100, 100);
            }
        }

        [SerializeField]
        private TIntParameter lightnessVariation;
        public TIntParameter LightnessVariation
        {
            get
            {
                return lightnessVariation;
            }
            set
            {
                lightnessVariation.value = Mathf.Clamp(value.value, -100, 100);
            }
        }

        [SerializeField]
        private TIntParameter hslRandomSeed;
        public TIntParameter HslRandomSeed
        {
            get
            {
                return hslRandomSeed;
            }
            set
            {
                hslRandomSeed = value;
            }
        }

        [SerializeField]
        private TFloatParameter maskThreshold;
        public TFloatParameter MaskThreshold
        {
            get
            {
                return maskThreshold;
            }
            set
            {
                maskThreshold.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TIntParameter maskRandomSeed;
        public TIntParameter MaskRandomSeed
        {
            get
            {
                return maskRandomSeed;
            }
            set
            {
                maskRandomSeed = value;
            }
        }

        public TSplatterNode() : base()
        {
            tileX = new TIntParameter() { value = 4 };
            tileY = new TIntParameter() { value = 4 };
            instancePerTile = new TIntParameter() { value = 1 };

            baseOffset = new TVector2Parameter() { value = Vector2.zero };
            offsetMinX = new TFloatParameter() { value = -1 };
            offsetMaxX = new TFloatParameter() { value = 1 };
            offsetMinY = new TFloatParameter() { value = -1 };
            offsetMaxY = new TFloatParameter() { value = 1 };
            offsetMapMultiplier = new TFloatParameter() { value = 1 };
            offsetRandomSeed = new TIntParameter() { value = 100 };

            baseRotation = new TFloatParameter() { value = 0 };
            rotationMin = new TFloatParameter() { value = -360 };
            rotationMax = new TFloatParameter() { value = 360 };
            rotationMapMultiplier = new TFloatParameter() { value = 1 };
            rotationRandomSeed = new TIntParameter() { value = 200 };

            baseScale = new TVector2Parameter() { value = Vector2.one };
            scaleMinX = new TFloatParameter() { value = -0.5f };
            scaleMaxX = new TFloatParameter() { value = 0.5f };
            scaleMinY = new TFloatParameter() { value = -0.5f };
            scaleMaxY = new TFloatParameter() { value = 0.5f };
            scaleMapMultiplier = new TFloatParameter() { value = 0 };
            scaleRandomSeed = new TIntParameter() { value = 300 };

            backgroundColor = new TColorParameter() { value = Color.black };
            hueVariation = new TIntParameter() { value = 0 };
            saturationVariation = new TIntParameter() { value = 0 };
            lightnessVariation = new TIntParameter() { value = 0 };
            hslRandomSeed = new TIntParameter() { value = 400 };

            maskThreshold = new TFloatParameter() { value = 1f };
            maskRandomSeed = new TIntParameter() { value = 500 };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            TSlotReference shapeTexRef = TSlotReference.Create(GUID, shapeSlot.Id);
            material.SetTexture(TConst.SHAPE_TEX, context.GetInputTexture(shapeTexRef));
            TSlotReference connectedShapeRef = context.GetInputLink(shapeTexRef);
            TSlot connectedShapeSlot = context.GetSlot(connectedShapeRef);
            if (connectedShapeSlot != null && connectedShapeSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            TSlotReference shapeAlphaRef = TSlotReference.Create(GUID, shapeAlphaSlot.Id);
            material.SetTexture(TConst.SHAPE_ALPHA_TEX, context.GetInputTexture(shapeAlphaRef));

            TDrawing.FillTexture(targetRT, backgroundColor.value);

            TSlotReference offsetMapRef = TSlotReference.Create(GUID, offsetMapSlot.Id);
            Texture offsetMapRT = context.GetInputTexture(offsetMapRef);
            Texture2D offsetMap;
            if (offsetMapRT != null)
            {
                offsetMap = new Texture2D(TConst.TEXTURE_SIZE, TConst.TEXTURE_SIZE);
                TDrawing.CopyTexture(offsetMapRT, offsetMap);
            }
            else
            {
                offsetMap = new Texture2D(1, 1);
                offsetMap.SetPixel(0, 0, Color.white);
            }

            TSlotReference rotationMapRef = TSlotReference.Create(GUID, rotationMapSlot.Id);
            Texture rotationMapRT = context.GetInputTexture(rotationMapRef);
            Texture2D rotationMap;
            if (rotationMapRT != null)
            {
                rotationMap = new Texture2D(TConst.TEXTURE_SIZE, TConst.TEXTURE_SIZE);
                TDrawing.CopyTexture(rotationMapRT, rotationMap);
            }
            else
            {
                rotationMap = new Texture2D(1, 1);
                rotationMap.SetPixel(0, 0, Color.white);
            }

            TSlotReference scaleMapRef = TSlotReference.Create(GUID, scaleMapSlot.Id);
            Texture scaleMapRT = context.GetInputTexture(scaleMapRef);
            Texture2D scaleMap;
            if (scaleMapRT != null)
            {
                scaleMap = new Texture2D(TConst.TEXTURE_SIZE, TConst.TEXTURE_SIZE);
                TDrawing.CopyTexture(scaleMapRT, scaleMap);
            }
            else
            {
                scaleMap = new Texture2D(1, 1);
                scaleMap.SetPixel(0, 0, Color.white);
            }

            TSlotReference maskMapRef = TSlotReference.Create(GUID, maskMapSlot.Id);
            Texture maskMapRT = context.GetInputTexture(maskMapRef);
            Texture2D maskMap;
            if (maskMapRT != null)
            {
                maskMap = new Texture2D(TConst.TEXTURE_SIZE, TConst.TEXTURE_SIZE);
                TDrawing.CopyTexture(maskMapRT, maskMap);
            }
            else
            {
                maskMap = new Texture2D(1, 1);
                maskMap.SetPixel(0, 0, Color.white);
            }

            List<Matrix4x4> matrices = GetMatrices(offsetMap, rotationMap, scaleMap, maskMap);
            Random hslRandom = new Random(hslRandomSeed.value);
            for (int i = 0; i < matrices.Count; ++i)
            {
                float h = Mathf.Lerp(0, hueVariation.value, (float)hslRandom.NextDouble());
                float s = Mathf.Lerp(0, saturationVariation.value, (float)hslRandom.NextDouble());
                float l = Mathf.Lerp(0, lightnessVariation.value, (float)hslRandom.NextDouble());
                material.SetFloat(TConst.HUE_SHIFT, h / 180f);
                material.SetFloat(TConst.SATURATION_SHIFT, s / 100f);
                material.SetFloat(TConst.LIGHTNESS_SHIFT, l / 100f);

                DrawTile(targetRT, material, matrices[i]);
            }

            TUtilities.DestroyObject(offsetMap);
            TUtilities.DestroyObject(rotationMap);
            TUtilities.DestroyObject(scaleMap);
            TUtilities.DestroyObject(maskMap);
        }

        private List<Matrix4x4> GetMatrices(Texture2D offsetMap, Texture2D rotationMap, Texture2D scaleMap, Texture2D maskMap)
        {
            Vector3 tileSize = new Vector3(1.0f / tileX.value, 1.0f / tileY.value, 1);
            Random offsetRandom = new Random(offsetRandomSeed.value);
            Random rotationRandom = new Random(rotationRandomSeed.value);
            Random scaleRandom = new Random(scaleRandomSeed.value);
            Random maskRandom = new Random(maskRandomSeed.value);

            List<Matrix4x4> trs = new List<Matrix4x4>();
            for (int x = 0; x < tileX.value; ++x)
            {
                for (int y = 0; y < tileY.value; ++y)
                {
                    Vector2 center = new Vector2((x + 0.5f) * tileSize.x, (y + 0.5f) * tileSize.y);
                    for (int i = 0; i < instancePerTile.value; ++i)
                    {
                        float offsetMapValue = offsetMap.GetPixelBilinear(center.x, center.y).r;
                        float offsetFactorX = (float)offsetRandom.NextDouble();
                        float offsetFactorY = (float)offsetRandom.NextDouble();
                        float oMinX = offsetMapValue * offsetMapMultiplier.value * offsetMinX.value;
                        float oMaxX = offsetMapValue * offsetMapMultiplier.value * offsetMaxX.value;
                        float oMinY = offsetMapValue * offsetMapMultiplier.value * offsetMinY.value;
                        float oMaxY = offsetMapValue * offsetMapMultiplier.value * offsetMaxY.value;
                        float offsetX = Mathf.Lerp(oMinX, oMaxX, offsetFactorX);
                        float offsetY = Mathf.Lerp(oMinY, oMaxY, offsetFactorY);
                        Vector3 o = new Vector3(baseOffset.value.x + offsetX, baseOffset.value.y + offsetY, 0);
                        Vector3 t = (Vector3)center + o;

                        float maskMapValue = maskMap.GetPixelBilinear(t.x, t.y).r;
                        maskMapValue = TUtilities.ApplyThreshold(maskMapValue, maskThreshold.value);
                        if ((float)maskRandom.NextDouble() > maskMapValue)
                        {
                            continue;
                        }

                        float rotationMapValue = rotationMap.GetPixelBilinear(t.x, t.y).r;
                        float rMin = rotationMapValue * rotationMapMultiplier.value * rotationMin.value;
                        float rMax = rotationMapValue * rotationMapMultiplier.value * rotationMax.value;
                        float rotationFactor = (float)rotationRandom.NextDouble();
                        float rotation = Mathf.Lerp(rMin, rMax, rotationFactor);
                        Quaternion r = Quaternion.Euler(0, 0, baseRotation.value + rotation);

                        float scaleMapValue = scaleMap.GetPixelBilinear(t.x, t.y).r;
                        float scaleFactor = (float)scaleRandom.NextDouble();
                        float sMinX = scaleMapValue * scaleMapMultiplier.value * scaleMinX.value;
                        float sMaxX = scaleMapValue * scaleMapMultiplier.value * scaleMaxX.value;
                        float sMinY = scaleMapValue * scaleMapMultiplier.value * scaleMinY.value;
                        float sMaxY = scaleMapValue * scaleMapMultiplier.value * scaleMaxY.value;
                        float scaleX = Mathf.Lerp(sMinX, sMaxX, scaleFactor);
                        float scaleY = Mathf.Lerp(sMinY, sMaxY, scaleFactor);
                        Vector3 s = new Vector3(tileSize.x * (baseScale.value.x + scaleX), tileSize.y * (baseScale.value.y + scaleY), tileSize.z);

                        Matrix4x4 m = Matrix4x4.TRS(t, r, s);
                        trs.Add(m);
                    }
                }
            }
            return trs;
        }

        private void DrawTile(RenderTexture rt, Material mat, Matrix4x4 transform)
        {
            Vector2 v0 = transform.MultiplyPoint(TConst.VERTICES[0]);
            Vector2 v1 = transform.MultiplyPoint(TConst.VERTICES[1]);
            Vector2 v2 = transform.MultiplyPoint(TConst.VERTICES[2]);
            Vector2 v3 = transform.MultiplyPoint(TConst.VERTICES[3]);

            Draw(rt, mat, v0, v1, v2, v3);

            Vector2 o;
            if (IsOverflowLeft(v0, v1, v2, v3))
            {
                o = Vector2.right;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowRight(v0, v1, v2, v3))
            {
                o = Vector2.left;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowBottom(v0, v1, v2, v3))
            {
                o = Vector2.up;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowTop(v0, v1, v2, v3))
            {
                o = Vector2.down;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowLeft(v0, v1, v2, v3) && IsOverflowBottom(v0, v1, v2, v3))
            {
                o = Vector2.one;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowLeft(v0, v1, v2, v3) && IsOverflowTop(v0, v1, v2, v3))
            {
                o = Vector2.right + Vector2.down;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowRight(v0, v1, v2, v3) && IsOverflowTop(v0, v1, v2, v3))
            {
                o = Vector2.left + Vector2.down;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
            if (IsOverflowRight(v0, v1, v2, v3) && IsOverflowBottom(v0, v1, v2, v3))
            {
                o = Vector2.left + Vector2.up;
                Draw(rt, mat, v0 + o, v1 + o, v2 + o, v3 + o);
            }
        }

        private bool IsOverflowLeft(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return v0.x < 0 || v1.x < 0 || v2.x < 0 || v3.x < 0;
        }

        private bool IsOverflowRight(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return v0.x > 1 || v1.x > 1 || v2.x > 1 || v3.x > 1;
        }

        private bool IsOverflowBottom(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return v0.y < 0 || v1.y < 0 || v2.y < 0 || v3.y < 0;
        }

        private bool IsOverflowTop(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            return v0.y > 1 || v1.y > 1 || v2.y > 1 || v3.y > 1;
        }

        private void Draw(RenderTexture rt, Material mat, Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            RenderTexture.active = rt;
            GL.PushMatrix();
            mat.SetPass(TConst.PASS);
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.TexCoord(TConst.UVS[0]);
            GL.Vertex3(v0.x, v0.y, 0);
            GL.TexCoord(TConst.UVS[1]);
            GL.Vertex3(v1.x, v1.y, 0);
            GL.TexCoord(TConst.UVS[2]);
            GL.Vertex3(v2.x, v2.y, 0);
            GL.TexCoord(TConst.UVS[3]);
            GL.Vertex3(v3.x, v3.y, 0);
            GL.End();
            GL.PopMatrix();
            RenderTexture.active = null;
        }
    }
}
