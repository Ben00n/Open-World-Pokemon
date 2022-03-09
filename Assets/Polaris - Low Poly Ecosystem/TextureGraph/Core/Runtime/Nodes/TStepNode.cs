using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Step",
        CreationMenu = "Filter/Step",
        Icon = "TextureGraph/NodeIcons/Step",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.h3vfxjp4qt9o")]
    public class TStepNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Step";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int STEP = Shader.PropertyToID("_Step");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        [SerializeField]
        private TIntParameter step;
        public TIntParameter Step
        {
            get
            {
                return step;
            }
            set
            {
                step.value = Mathf.Max(2, value.value);
            }
        }

        private Shader shader;
        private Material material;

        public TStepNode() : base()
        {
            step = new TIntParameter() { value = 25 };
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
            material.SetFloat(TConst.STEP, Step.value);

            TSlotReference connectedInputRef = context.GetInputLink(inputRef);
            TSlot connectedInput = context.GetSlot(connectedInputRef);
            if (connectedInput != null && connectedInput.DataType == TSlotDataType.Gray)
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
