using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gradient Linear",
        CreationMenu = "Shapes & Patterns/Gradient Linear",
        Icon = "TextureGraph/NodeIcons/GradientLinear",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.yy8z1edqzl4q")]
    public class TGradientLinearNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GradientLinear";
            public static readonly int UV_TO_GRADIENT_MATRIX = Shader.PropertyToID("_UvToGradientMatrix");
            public static readonly int MID_POINT = Shader.PropertyToID("_MidPoint");
            public static readonly int PASS = 0;
        }

        public enum TRotation
        {
            Degree0 = 0,
            Degree90 = 90,
            Degree180 = 180,
            Degree270 = 270
        }

        [System.Serializable]
        public sealed class TRotationParameter : TGenericParameter<TRotation> { }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 0);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TRotationParameter rotation;
        public TRotationParameter Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        [SerializeField]
        private TIntParameter scale;
        public TIntParameter Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TFloatParameter midPoint;
        public TFloatParameter MidPoint
        {
            get
            {
                return midPoint;
            }
            set
            {
                midPoint.value = Mathf.Clamp01(value.value);
            }
        }

        public TGradientLinearNode() : base()
        {
            rotation = new TRotationParameter() { value = TRotation.Degree0 };
            scale = new TIntParameter() { value = 1 };
            midPoint = new TFloatParameter() { value = 1 };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            Vector3 t = Vector3.zero;
            Quaternion r = Quaternion.Euler(0, 0, (float)rotation.value);
            Vector3 s = new Vector3(1.0f / scale.value, 1.0f / scale.value, 1);
            Matrix4x4 uvToGradientMatrix = Matrix4x4.TRS(t, r, s).inverse;
            material.SetMatrix(TConst.UV_TO_GRADIENT_MATRIX, uvToGradientMatrix);
            material.SetFloat(TConst.MID_POINT, midPoint.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
