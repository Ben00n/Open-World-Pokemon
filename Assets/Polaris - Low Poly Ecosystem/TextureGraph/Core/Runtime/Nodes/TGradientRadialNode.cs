using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gradient Radial",
        CreationMenu = "Shapes & Patterns/Gradient Radial",
        Icon = "TextureGraph/NodeIcons/GradientRadial",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.3gbdgbt4god")]
    public class TGradientRadialNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GradientRadial";
            public static readonly int CENTER = Shader.PropertyToID("_Center");
            public static readonly int END_POINT = Shader.PropertyToID("_EndPoint");
            public static readonly int PASS = 0;
        }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 0);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TVector2Parameter centerPoint;
        public TVector2Parameter CenterPoint
        {
            get
            {
                return centerPoint;
            }
            set
            {
                centerPoint = value;
            }
        }

        [SerializeField]
        private TVector2Parameter endPoint;
        public TVector2Parameter EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                endPoint = value;
            }
        }

        public TGradientRadialNode() : base()
        {
            centerPoint = new TVector2Parameter() { value = new Vector2(0.5f, 0.5f) };
            endPoint = new TVector2Parameter() { value = new Vector2(1f, 0.5f) };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            material.SetVector(TConst.CENTER, centerPoint.value);
            material.SetVector(TConst.END_POINT, endPoint.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
