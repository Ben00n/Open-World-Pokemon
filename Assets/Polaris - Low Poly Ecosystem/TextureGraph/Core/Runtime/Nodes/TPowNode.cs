using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Pow",
        CreationMenu = "Filter/Pow",
        Icon = "TextureGraph/NodeIcons/Pow",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.jy23n3j5jypm")]
    public class TPowNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Pow";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int POWER = Shader.PropertyToID("_Power");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        [SerializeField]
        private float power;
        public float Power
        {
            get
            {
                return power;
            }
            set
            {
                power = value;
            }
        }

        private Shader shader;
        private Material material;

        public TPowNode() : base()
        {
            power = 1;
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
            material.SetFloat(TConst.POWER, Power);

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
