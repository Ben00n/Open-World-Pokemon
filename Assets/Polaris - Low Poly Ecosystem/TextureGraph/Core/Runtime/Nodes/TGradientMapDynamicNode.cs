using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gradient Map Dynamic", 
        CreationMenu = "Basic/Gradient Map Dynamic", 
        Icon = "TextureGraph/NodeIcons/GradientMapDynamic",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.9n00yio4yh4o")]
    public class TGradientMapDynamicNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GradientMapDynamic";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int GRADIENT_TEX = Shader.PropertyToID("_GradientTex");
            public static readonly int SLICE = Shader.PropertyToID("_Slice");

            public static readonly string RRR = "RRR";
        }

        public readonly TSlot mainTextureSlot = new TSlot("Main Texture", TSlotType.Input, TSlotDataType.Gray, 0);
        public readonly TSlot gradientSlot = new TSlot("Gradient", TSlotType.Input, TSlotDataType.RGBA, 1);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 2);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TAxisParameter axis;
        public TAxisParameter Axis
        {
            get
            {
                return axis;
            }
            set
            {
                axis = value;
            }
        }

        [SerializeField]
        private TFloatParameter slice;
        public TFloatParameter Slice
        {
            get
            {
                return slice;
            }
            set
            {
                slice.value = Mathf.Clamp01(value.value);
            }
        }

        public TGradientMapDynamicNode() : base()
        {
            axis = new TAxisParameter() { value = TAxis.Horizontal };
            slice = new TFloatParameter() { value = 0f };
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

            TSlotReference gradientRef = TSlotReference.Create(GUID, gradientSlot.Id);
            material.SetTexture(TConst.GRADIENT_TEX, context.GetInputTexture(gradientRef));
            TSlotReference connectedGradientRef = context.GetInputLink(gradientRef);
            TSlot connectedGradientSlot = context.GetSlot(connectedGradientRef);
            if (connectedGradientSlot!=null && connectedGradientSlot.DataType== TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            material.SetFloat(TConst.SLICE, slice.value);
            TDrawing.DrawFullQuad(targetRT, material, (int)axis.value);
        }
    }
}
