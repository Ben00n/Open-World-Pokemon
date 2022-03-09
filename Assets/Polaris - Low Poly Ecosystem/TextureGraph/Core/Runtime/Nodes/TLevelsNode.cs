using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Levels", 
        CreationMenu = "Filter/Levels", 
        Icon = "TextureGraph/NodeIcons/Levels",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.lhbrlxqzk8on")]
    public class TLevelsNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Levels";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int IN_LUMINANCE = Shader.PropertyToID("_InLuminance");
            public static readonly int OUT_LUMINANCE = Shader.PropertyToID("_OutLuminance");
            public static readonly int IN_RED = Shader.PropertyToID("_InRed");
            public static readonly int OUT_RED = Shader.PropertyToID("_OutRed");
            public static readonly int IN_GREEN = Shader.PropertyToID("_InGreen");
            public static readonly int OUT_GREEN = Shader.PropertyToID("_OutGreen");
            public static readonly int IN_BLUE = Shader.PropertyToID("_InBlue");
            public static readonly int OUT_BLUE = Shader.PropertyToID("_OutBlue");
            public static readonly int IN_ALPHA = Shader.PropertyToID("_InAlpha");
            public static readonly int OUT_ALPHA = Shader.PropertyToID("_OutAlpha");


            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        [System.Serializable]
        public class TLevelsControl
        {
            public float inLow;
            public float inMid;
            public float inHigh;
            public float outLow;
            public float outHigh;

            public Vector4 In
            {
                get
                {
                    return new Vector4(inLow, Mathf.Lerp(inLow, inHigh, inMid), inHigh, 0);
                }
            }

            public Vector4 Out
            {
                get
                {
                    return new Vector4(outLow, outHigh, 0, 0);
                }
            }

            public TLevelsControl()
            {
                inLow = 0f;
                inMid = 0.5f;
                inHigh = 1f;

                outLow = 0f;
                outHigh = 1f;
            }
        }

        [System.Serializable]
        public sealed class TLevelsControlParam : TGenericParameter<TLevelsControl> { }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Ouput", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TLevelsControlParam luminanceControl;
        public TLevelsControlParam LuminanceControl
        {
            get
            {
                return luminanceControl;
            }
            set
            {
                luminanceControl = value;
            }
        }

        [SerializeField]
        private TLevelsControlParam redControl;
        public TLevelsControlParam RedControl
        {
            get
            {
                return redControl;
            }
            set
            {
                redControl = value;
            }
        }

        [SerializeField]
        private TLevelsControlParam greenControl;
        public TLevelsControlParam GreenControl
        {
            get
            {
                return greenControl;
            }
            set
            {
                greenControl = value;
            }
        }

        [SerializeField]
        private TLevelsControlParam blueControl;
        public TLevelsControlParam BlueControl
        {
            get
            {
                return blueControl;
            }
            set
            {
                blueControl = value;
            }
        }

        [SerializeField]
        private TLevelsControlParam alphaControl;
        public TLevelsControlParam AlphaControl
        {
            get
            {
                return alphaControl;
            }
            set
            {
                alphaControl = value;
            }
        }

        public TLevelsNode() : base()
        {
            luminanceControl = new TLevelsControlParam() { value = new TLevelsControl() };
            redControl = new TLevelsControlParam() { value = new TLevelsControl() };
            greenControl = new TLevelsControlParam() { value = new TLevelsControl() };
            blueControl = new TLevelsControlParam() { value = new TLevelsControl() };
            alphaControl = new TLevelsControlParam() { value = new TLevelsControl() };
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

            material.SetVector(TConst.IN_LUMINANCE, luminanceControl.value.In);
            material.SetVector(TConst.OUT_LUMINANCE, luminanceControl.value.Out);

            material.SetVector(TConst.IN_RED, redControl.value.In);
            material.SetVector(TConst.OUT_RED, redControl.value.Out);

            material.SetVector(TConst.IN_GREEN, greenControl.value.In);
            material.SetVector(TConst.OUT_GREEN, greenControl.value.Out);

            material.SetVector(TConst.IN_BLUE, blueControl.value.In);
            material.SetVector(TConst.OUT_BLUE, blueControl.value.Out);

            material.SetVector(TConst.IN_ALPHA, alphaControl.value.In);
            material.SetVector(TConst.OUT_ALPHA, alphaControl.value.Out);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
