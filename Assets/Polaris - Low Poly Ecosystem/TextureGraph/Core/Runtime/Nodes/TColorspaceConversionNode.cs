using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Colorspace Conversion",
        CreationMenu = "Basic/Colorspace Conversion",
        Icon = "TextureGraph/NodeIcons/ColorspaceConversion",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.exx85h3c4j9x")]
    public class TColorspaceConversionNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/ColorspaceConversion";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly string RRR = "RRR";
        }

        public enum TConversionMode
        {
            GammaToLinear, LinearToGamma
        }

        [System.Serializable]
        public sealed class TConversionModeParameter : TGenericParameter<TConversionMode> { }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        [SerializeField]
        private TConversionModeParameter conversionMode;
        public TConversionModeParameter ConversionMode
        {
            get
            {
                return conversionMode;
            }
            set
            {
                conversionMode = value;
            }
        }

        private Shader shader;
        private Material material;

        public TColorspaceConversionNode() : base()
        {
            conversionMode = new TConversionModeParameter() { value = TConversionMode.GammaToLinear };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            TSlotReference inputRef = TSlotReference.Create(GUID, inputSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(inputRef));
            TSlotReference connectedInputRef = context.GetInputLink(inputRef);
            TSlot connectedInputSlot = context.GetSlot(connectedInputRef);
            if (connectedInputSlot != null && connectedInputSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            TDrawing.DrawFullQuad(targetRT, material, (int)ConversionMode.value);
        }
    }
}
