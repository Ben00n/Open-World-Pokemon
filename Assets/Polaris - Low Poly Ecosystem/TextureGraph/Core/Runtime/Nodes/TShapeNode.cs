using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Shape",
        CreationMenu = "Shapes & Patterns/Shape",
        Icon = "TextureGraph/NodeIcons/Shape",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.esof2qtm81vr")]
    public class TShapeNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Shape";
            public static readonly int UV_TO_SHAPE_MATRIX = Shader.PropertyToID("_UvToShapeMatrix");
            public static readonly int INNER_SIZE = Shader.PropertyToID("_InnerSize");
        }

        public enum TShape
        {
            Square, Disc, Hemisphere, Cone, Paraboloid, Bell, Thorn, Pyramid, Brick, Torus
        }

        [System.Serializable]
        public sealed class TShapeParameter : TGenericParameter<TShape> { }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 0);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TShapeParameter shape;
        public TShapeParameter Shape
        {
            get
            {
                return shape;
            }
            set
            {
                shape = value;
            }
        }

        [SerializeField]
        private TVector2Parameter scale;
        public TVector2Parameter Scale
        {
            get
            {
                return scale;
            }
            set
            {
                Vector2 v = value.value;
                scale.value = new Vector2(Mathf.Max(0.001f, v.x), Mathf.Max(0.001f, v.y));
            }
        }

        [SerializeField]
        private TFloatParameter innerSize;
        public TFloatParameter InnerSize
        {
            get
            {
                return innerSize;
            }
            set
            {
                innerSize = value;
            }
        }

        public TShapeNode() : base()
        {
            shape = new TShapeParameter() { value = TShape.Square };
            scale = new TVector2Parameter() { value = new Vector2(1, 1) };
            innerSize = new TFloatParameter() { value = 0.5f };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            Vector3 t = new Vector3(0.5f, 0.5f);
            Quaternion r = Quaternion.identity;
            Vector3 s = new Vector3(scale.value.x, scale.value.y, 1);
            Matrix4x4 uvToShapeMatrix = Matrix4x4.TRS(t, r, s).inverse;
            material.SetMatrix(TConst.UV_TO_SHAPE_MATRIX, uvToShapeMatrix);
            material.SetFloat(TConst.INNER_SIZE, InnerSize.value);

            TDrawing.DrawFullQuad(targetRT, material, (int)Shape.value);
        }
    }
}
