using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Warp",
        CreationMenu = "Effects/Warp",
        Icon = "TextureGraph/NodeIcons/Warp",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.m94rpjm9uswd")]
    public class TWarpNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Warp";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int VECTOR_MAP = Shader.PropertyToID("_VectorMap");
            public static readonly int INTENSITY = Shader.PropertyToID("_Intensity");
            public static readonly int RRR = Shader.PropertyToID("_RRR");
            public static readonly int PASS = 0;
        }

        public readonly TSlot mainTextureSlot = new TSlot("Main Texture", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot vectorMapSlot = new TSlot("Vector Map", TSlotType.Input, TSlotDataType.RGBA, 1);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 2);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TFloatParameter intensity;
        public TFloatParameter Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity.value = Mathf.Clamp01(value.value);
            }
        }

        public TWarpNode() : base()
        {
            intensity = new TFloatParameter() { value = 0 };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            TSlotReference mainTexRef = TSlotReference.Create(GUID, mainTextureSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(mainTexRef));
            TSlotReference connectedMainTexRef = context.GetInputLink(mainTexRef);
            TSlot connectedMainTexSlot = context.GetSlot(connectedMainTexRef);
            if (connectedMainTexSlot != null && connectedMainTexSlot.DataType == TSlotDataType.Gray)
            {
                material.SetFloat(TConst.RRR, 1);
            }
            else
            {
                material.SetFloat(TConst.RRR, 0);
            }

            TSlotReference vectorMapRef = TSlotReference.Create(GUID, vectorMapSlot.Id);
            material.SetTexture(TConst.VECTOR_MAP, context.GetInputTexture(vectorMapRef));
            material.SetFloat(TConst.INTENSITY, intensity.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
