using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Threshold",
        CreationMenu = "Filter/Threshold",
        Icon = "TextureGraph/NodeIcons/Threshold",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.6iv7kp4qfhhh")]
    public class TThresholdNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Threshold";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int THRESHOLD_LOW = Shader.PropertyToID("_ThresholdLow");
            public static readonly int THRESHOLD_HIGH = Shader.PropertyToID("_ThresholdHigh");
            public static readonly int MODE = Shader.PropertyToID("_Mode");
            public static readonly int PASS = 0;
        }

        public enum TCompareMode
        {
            Exclusive, Inclusive
        }

        [System.Serializable]
        public sealed class TCompareModeParameter : TGenericParameter<TCompareMode> { }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.Gray, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 1);

        [SerializeField]
        private TFloatParameter thresholdLow;
        public TFloatParameter ThresholdLow
        {
            get
            {
                return thresholdLow;
            }
            set
            {
                thresholdLow.value = Mathf.Clamp01(Mathf.Min(thresholdHigh.value, value.value));
            }
        }

        [SerializeField]
        private TFloatParameter thresholdHigh;
        public TFloatParameter ThresholdHigh
        {
            get
            {
                return thresholdHigh;
            }
            set
            {
                thresholdHigh.value = Mathf.Clamp01(Mathf.Max(thresholdLow.value, value.value));
            }
        }

        [SerializeField]
        private TCompareModeParameter mode;
        public TCompareModeParameter Mode
        {
            get
            {
                return mode;
            }
            set
            {
                mode = value;
            }
        }

        private Shader shader;
        private Material material;


        public TThresholdNode() : base()
        {
            thresholdLow = new TFloatParameter() { value = 0 };
            thresholdHigh = new TFloatParameter() { value = 1 };
            mode = new TCompareModeParameter() { value = TCompareMode.Exclusive };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(TSlotReference.Create(GUID, inputSlot.Id)));
            material.SetFloat(TConst.THRESHOLD_LOW, ThresholdLow.value);
            material.SetFloat(TConst.THRESHOLD_HIGH, ThresholdHigh.value);
            material.SetFloat(TConst.MODE, (float)Mode.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
