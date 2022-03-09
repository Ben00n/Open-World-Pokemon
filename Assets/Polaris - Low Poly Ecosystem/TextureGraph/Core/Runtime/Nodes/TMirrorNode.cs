using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Mirror",
        CreationMenu = "Transformation/Mirror",
        Icon = "TextureGraph/NodeIcons/Mirror",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.usnt70qksj78")]
    public class TMirrorNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Mirror";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int OFFSET_X = Shader.PropertyToID("_OffsetX");
            public static readonly int OFFSET_Y = Shader.PropertyToID("_OffsetY");

            public static readonly string RRR = "RRR";
            public static readonly string MIRROR_X = "MIRROR_X";
            public static readonly string FLIP_X = "FLIP_X";
            public static readonly string MIRROR_Y = "MIRROR_Y";
            public static readonly string FLIP_Y = "FLIP_Y";

            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TBoolParameter mirrorX;
        public TBoolParameter MirrorX
        {
            get
            {
                return mirrorX;
            }
            set
            {
                mirrorX = value;
            }
        }

        [SerializeField]
        private TBoolParameter flipX;
        public TBoolParameter FlipX
        {
            get
            {
                return flipX;
            }
            set
            {
                flipX = value;
            }
        }

        [SerializeField]
        private TFloatParameter offsetX;
        public TFloatParameter OffsetX
        {
            get
            {
                return offsetX;
            }
            set
            {
                offsetX.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TBoolParameter mirrorY;
        public TBoolParameter MirrorY
        {
            get
            {
                return mirrorY;
            }
            set
            {
                mirrorY = value;
            }
        }

        [SerializeField]
        private TBoolParameter flipY;
        public TBoolParameter FlipY
        {
            get
            {
                return flipY;
            }
            set
            {
                flipY = value;
            }
        }

        [SerializeField]
        private TFloatParameter offsetY;
        public TFloatParameter OffsetY
        {
            get
            {
                return offsetY;
            }
            set
            {
                offsetY.value = Mathf.Clamp01(value.value);
            }
        }

        public TMirrorNode() : base()
        {
            mirrorX = new TBoolParameter() { value = true };
            flipX = new TBoolParameter() { value = false };
            offsetX = new TFloatParameter() { value = 0.5f };

            mirrorY = new TBoolParameter() { value = false };
            flipY = new TBoolParameter() { value = false };
            offsetY = new TFloatParameter() { value = 0.5f };
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

            if (MirrorX.value == true)
            {
                material.EnableKeyword(TConst.MIRROR_X);
            }
            else
            {
                material.DisableKeyword(TConst.MIRROR_X);
            }
            material.SetFloat(TConst.OFFSET_X, OffsetX.value);

            if (FlipX.value == true)
            {
                material.EnableKeyword(TConst.FLIP_X);
            }
            else
            {
                material.DisableKeyword(TConst.FLIP_X);
            }

            if (MirrorY.value == true)
            {
                material.EnableKeyword(TConst.MIRROR_Y);
            }
            else
            {
                material.DisableKeyword(TConst.MIRROR_Y);
            }
            material.SetFloat(TConst.OFFSET_Y, OffsetY.value);
            if (FlipY.value == true)
            {
                material.EnableKeyword(TConst.FLIP_Y);
            }
            else
            {
                material.DisableKeyword(TConst.FLIP_Y);
            }

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
