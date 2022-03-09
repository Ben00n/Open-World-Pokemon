using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Blend",
        CreationMenu = "Basic/Blend",
        Icon = "TextureGraph/NodeIcons/Blend",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.r0wol7p7depn")]
    public class TBlendNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Blend";
            public static readonly int BACKGROUND = Shader.PropertyToID("_Background");
            public static readonly int FOREGROUND = Shader.PropertyToID("_Foreground");
            public static readonly int MASK = Shader.PropertyToID("_Mask");
            public static readonly int OPACITY = Shader.PropertyToID("_Opacity");
            public static readonly int ALPHA_MODE = Shader.PropertyToID("_AlphaMode");
            public static readonly string BG_RRR = "BG_RRR";
            public static readonly string FG_RRR = "FG_RRR";
        }

        public enum TBlendMode
        {
            Normal, Add, Subtract, Multiply, Divide, Screen, Overlay, HardLight, SoftLight, Max, Min
        }

        [System.Serializable]
        public sealed class TBlendModeParameter : TGenericParameter<TBlendMode> { }

        public enum TAlphaBlendMode
        {
            One, FromBackground, FromForeground, Fade, Premultiply
        }

        [System.Serializable]
        public sealed class TAlphaBlendModeParameter : TGenericParameter<TAlphaBlendMode> { }

        public readonly TSlot backgroundSlot = new TSlot("Background", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot foregroundSlot = new TSlot("Foreground", TSlotType.Input, TSlotDataType.RGBA, 1);
        public readonly TSlot maskSlot = new TSlot("Mask", TSlotType.Input, TSlotDataType.Gray, 2);

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 3);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TBlendModeParameter colorBlendMode;
        public TBlendModeParameter ColorBlendMode
        {
            get
            {
                return colorBlendMode;
            }
            set
            {
                colorBlendMode = value;
            }
        }

        [SerializeField]
        private TAlphaBlendModeParameter alphaBlendMode;
        public TAlphaBlendModeParameter AlphaBlendMode
        {
            get
            {
                return alphaBlendMode;
            }
            set
            {
                alphaBlendMode = value;
            }
        }

        [SerializeField]
        private TFloatParameter opacity;
        public TFloatParameter Opacity
        {
            get
            {
                return opacity;
            }
            set
            {
                float v = value.value;
                v = Mathf.Clamp01(v);
                opacity.value = v;
            }
        }

        public TBlendNode() : base()
        {
            colorBlendMode = new TBlendModeParameter() { value = TBlendMode.Normal };
            alphaBlendMode = new TAlphaBlendModeParameter() { value = TAlphaBlendMode.One };
            opacity = new TFloatParameter() { value = 1 };
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
            material.SetTexture(TConst.BACKGROUND, context.GetInputTexture(bgRef));
            TSlotReference connectedBgRef = context.GetInputLink(bgRef);
            TSlot connectedBgSlot = context.GetSlot(connectedBgRef);
            if (connectedBgSlot != null && connectedBgSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.BG_RRR);
            }
            else
            {
                material.DisableKeyword(TConst.BG_RRR);
            }

            TSlotReference fgRef = TSlotReference.Create(GUID, foregroundSlot.Id);
            material.SetTexture(TConst.FOREGROUND, context.GetInputTexture(fgRef));
            TSlotReference connectedFgRef = context.GetInputLink(fgRef);
            TSlot connectedFgSlot = context.GetSlot(connectedFgRef);
            if (connectedFgSlot != null && connectedFgSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.FG_RRR);
            }
            else
            {
                material.DisableKeyword(TConst.FG_RRR);
            }

            material.SetTexture(TConst.MASK, context.GetInputTexture(TSlotReference.Create(GUID, maskSlot.Id)));
            material.SetFloat(TConst.OPACITY, Opacity.value);
            material.SetInt(TConst.ALPHA_MODE, (int)AlphaBlendMode.value);

            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, (int)ColorBlendMode.value);
        }
    }
}
