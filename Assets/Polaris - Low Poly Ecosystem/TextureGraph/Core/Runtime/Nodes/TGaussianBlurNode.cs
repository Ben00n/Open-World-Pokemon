using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gaussian Blur",
        CreationMenu = "Filter/Gaussian Blur",
        Icon = "TextureGraph/NodeIcons/GaussianBlur",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.1vwf7gj7esgm")]
    public class TGaussianBlurNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GaussianBlur";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int RRR = Shader.PropertyToID("_RRR");
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TIntParameter radius;
        public TIntParameter Radius
        {
            get
            {
                return radius;
            }
            set
            {
                int v = value.value;
                v = Mathf.Clamp(v, 0, 100);
                radius.value = v;
            }
        }

        public TGaussianBlurNode() : base()
        {
            radius = new TIntParameter() { value = 1 };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            if (shader == null)
            {
                shader = Shader.Find(TConst.SHADER);
            }
            if (material == null)
            {
                material = new Material(shader);
            }

            TSlotReference inputRef = TSlotReference.Create(GUID, inputSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(inputRef));
            TSlotReference connectedInputRef = context.GetInputLink(inputRef);
            TSlot connectedInputSlot = context.GetSlot(connectedInputRef);
            if (connectedInputSlot != null && connectedInputSlot.DataType == TSlotDataType.Gray)
            {
                material.SetFloat(TConst.RRR, 1);
            }
            else
            {
                material.SetFloat(TConst.RRR, 0);
            }

            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, radius.value);
        }
    }
}
