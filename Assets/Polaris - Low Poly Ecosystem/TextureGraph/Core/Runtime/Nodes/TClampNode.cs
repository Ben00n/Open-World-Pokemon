using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Clamp",
        CreationMenu = "Filter/Clamp",
        Icon = "TextureGraph/NodeIcons/Clamp",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.ilseqto00ab7")]
    public class TClampNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Clamp";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int MIN = Shader.PropertyToID("_Min");
            public static readonly int MAX = Shader.PropertyToID("_Max");
            public static readonly int APPLY_ALPHA = Shader.PropertyToID("_ApplyToAlpha");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        [SerializeField]
        private TFloatParameter min;
        public TFloatParameter Min
        {
            get
            {
                return min;
            }
            set
            {
                float v = value.value;
                v = Mathf.Clamp01(Mathf.Min(v, max.value));
                min.value = v;
            }
        }

        [SerializeField]
        private TFloatParameter max;
        public TFloatParameter Max
        {
            get
            {
                return max;
            }
            set
            {
                float v = value.value;
                v = Mathf.Clamp01(Mathf.Max(v, min.value));
                max.value = v;
            }
        }

        [SerializeField]
        private TBoolParameter applyToAlpha;
        public TBoolParameter ApplyToAlpha
        {
            get
            {
                return applyToAlpha;
            }
            set
            {
                applyToAlpha = value;
            }
        }

        private Shader shader;
        private Material material;

        public TClampNode() : base()
        {
            min = new TFloatParameter() { value = 0 };
            max = new TFloatParameter() { value = 1 };
            applyToAlpha = new TBoolParameter() { value = false };
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
            material.SetFloat(TConst.MIN, Min.value);
            material.SetFloat(TConst.MAX, Max.value);
            material.SetFloat(TConst.APPLY_ALPHA, ApplyToAlpha.value ? 1 : 0);

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

            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);
        }
    }
}
