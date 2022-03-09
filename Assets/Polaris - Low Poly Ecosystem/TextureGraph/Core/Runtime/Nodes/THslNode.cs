using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "HSL", 
        CreationMenu = "Filter/HSL", 
        Icon = "TextureGraph/NodeIcons/HSL",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.pxd43kjc1php")]
    public class THslNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/HSL";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int HUE = Shader.PropertyToID("_Hue");
            public static readonly int SATURATION = Shader.PropertyToID("_Saturation");
            public static readonly int LIGHTNESS = Shader.PropertyToID("_Lightness");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TIntParameter hue;
        public TIntParameter Hue
        {
            get
            {
                return hue;
            }
            set
            {
                hue.value = Mathf.Clamp(value.value, -180, 180);
            }
        }

        [SerializeField]
        private TIntParameter saturation;
        public TIntParameter Saturation
        {
            get
            {
                return saturation;
            }
            set
            {
                saturation.value = Mathf.Clamp(value.value, -100, 100);
            }
        }

        [SerializeField]
        private TIntParameter lightness;
        public TIntParameter Lightness
        {
            get
            {
                return lightness;
            }
            set
            {
                lightness.value = Mathf.Clamp(value.value, -100, 100);
            }
        }

        public THslNode() : base()
        {
            hue = new TIntParameter() { value = 0 };
            saturation = new TIntParameter() { value = 0 };
            lightness = new TIntParameter() { value = 0 };
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

            material.SetFloat(TConst.HUE, hue.value / 180f);
            material.SetFloat(TConst.SATURATION, saturation.value / 100f);
            material.SetFloat(TConst.LIGHTNESS, lightness.value / 100f);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
