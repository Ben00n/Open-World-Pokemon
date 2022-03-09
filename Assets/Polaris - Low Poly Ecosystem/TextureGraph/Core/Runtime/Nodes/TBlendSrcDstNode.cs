using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEngine.Rendering;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Blend Src Dst",
        CreationMenu = "Basic/Blend Src Dst",
        Icon = "TextureGraph/NodeIcons/BlendSrcDst",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.in5uqcourviy")]
    public class TBlendSrcDstNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/BlendSrcDst";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int SRC = Shader.PropertyToID("_SrcColor");
            public static readonly int DST = Shader.PropertyToID("_DstColor");
            public static readonly int SRC_ALPHA = Shader.PropertyToID("_SrcAlpha");
            public static readonly int DST_ALPHA = Shader.PropertyToID("_DstAlpha");
            public static readonly int OPS = Shader.PropertyToID("_ColorOps");
            public static readonly int OPS_ALPHA = Shader.PropertyToID("_AlphaOps");

            public static readonly string RRR = "RRR";
            public static readonly int PASS_COPY = 0;
            public static readonly int PASS_BLEND = 1;
        }

        public readonly TSlot backgroundSlot = new TSlot("Background", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot foregroundSlot = new TSlot("Foreground", TSlotType.Input, TSlotDataType.RGBA, 1);

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 2);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TBlendModeParameter srcColor;
        public TBlendModeParameter SrcColor
        {
            get
            {
                return srcColor;
            }
            set
            {
                srcColor = value;
            }
        }

        [SerializeField]
        private TBlendModeParameter dstColor;
        public TBlendModeParameter DstColor
        {
            get
            {
                return dstColor;
            }
            set
            {
                dstColor = value;
            }
        }

        [SerializeField]
        private TBlendModeParameter srcAlpha;
        public TBlendModeParameter SrcAlpha
        {
            get
            {
                return srcAlpha;
            }
            set
            {
                srcAlpha = value;
            }
        }

        [SerializeField]
        private TBlendModeParameter dstAlpha;
        public TBlendModeParameter DstAlpha
        {
            get
            {
                return dstAlpha;
            }
            set
            {
                dstAlpha = value;
            }
        }

        [SerializeField]
        private TBlendOpParameter colorOps;
        public TBlendOpParameter ColorOps
        {
            get
            {
                return colorOps;
            }
            set
            {
                colorOps = value;
            }
        }

        [SerializeField]
        private TBlendOpParameter alphaOps;
        public TBlendOpParameter AlphaOps
        {
            get
            {
                return alphaOps;
            }
            set
            {
                alphaOps = value;
            }
        }

        public TBlendSrcDstNode() : base()
        {
            srcColor = new TBlendModeParameter() { value = BlendMode.SrcAlpha };
            dstColor = new TBlendModeParameter() { value = BlendMode.OneMinusSrcAlpha };

            srcAlpha = new TBlendModeParameter() { value = BlendMode.SrcAlpha };
            dstAlpha = new TBlendModeParameter() { value = BlendMode.OneMinusSrcAlpha };

            colorOps = new TBlendOpParameter() { value = BlendOp.Add };
            alphaOps = new TBlendOpParameter() { value = BlendOp.Add };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            TSlotReference bgRef = TSlotReference.Create(GUID, backgroundSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(bgRef));
            TSlotReference connectedBgRef = context.GetInputLink(bgRef);
            TSlot connectedBgSlot = context.GetSlot(connectedBgRef);
            if (connectedBgSlot != null && connectedBgSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }
            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS_COPY);

            TSlotReference fgRef = TSlotReference.Create(GUID, foregroundSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(fgRef));
            TSlotReference connectedFgRef = context.GetInputLink(fgRef);
            TSlot connectedFgSlot = context.GetSlot(connectedFgRef);
            if (connectedFgSlot != null && connectedFgSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }
            material.SetInt(TConst.SRC, (int)SrcColor.value);
            material.SetInt(TConst.DST, (int)DstColor.value);
            material.SetInt(TConst.SRC_ALPHA, (int)SrcAlpha.value);
            material.SetInt(TConst.DST_ALPHA, (int)DstAlpha.value);
            material.SetInt(TConst.OPS, (int)ColorOps.value);
            material.SetInt(TConst.OPS_ALPHA, (int)AlphaOps.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS_BLEND);
        }
    }
}
