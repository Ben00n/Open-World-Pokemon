using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Contrast Brightness",
        CreationMenu = "Filter/Contrast & Brightness",
        Icon = "TextureGraph/NodeIcons/ContrastBrightness",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.8tem5ldszhtq")]
    public class TContrastBrightnessNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/ContrastBrightness";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int CONTRAST = Shader.PropertyToID("_Contrast");
            public static readonly int BRIGHTNESS = Shader.PropertyToID("_Brightness");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TFloatParameter contrast;
        public TFloatParameter Contrast
        {
            get
            {
                return contrast;
            }
            set
            {
                contrast.value = Mathf.Clamp(value.value, -1, 1);
            }
        }

        [SerializeField]
        private TFloatParameter brightness;
        public TFloatParameter Brightness
        {
            get
            {
                return brightness;
            }
            set
            {
                brightness.value = Mathf.Clamp(value.value, -1, 1);
            }
        }

        public TContrastBrightnessNode() : base()
        {
            contrast = new TFloatParameter() { value = 0 };
            brightness = new TFloatParameter() { value = 0 };
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

            material.SetFloat(TConst.CONTRAST, contrast.value);
            material.SetFloat(TConst.BRIGHTNESS, brightness.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
