using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Channels Shuffle",
        CreationMenu = "Basic/Channels Shuffle",
        Icon = "TextureGraph/NodeIcons/ChannelsShuffle",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.72pcc396gw6e")]
    public class TChannelsShuffleNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/ChannelsShuffle";
            public static readonly int TEXTURE_0 = Shader.PropertyToID("_Texture0");
            public static readonly int TEXTURE_1 = Shader.PropertyToID("_Texture1");
            public static readonly int CHANNEL_SOURCE = Shader.PropertyToID("_ChannelSource");
            public static readonly string RRR_0 = "RRR_0";
            public static readonly string RRR_1 = "RRR_1";
            public static readonly int PASS = 0;
        }

        public enum TChannelSource
        {
            R0, G0, B0, A0, R1, G1, B1, A1
        }

        [System.Serializable]
        public sealed class TChannelSourceParameter : TGenericParameter<TChannelSource> { }

        public readonly TSlot inputSlot0 = new TSlot("Input 0", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot inputSlot1 = new TSlot("Input 1", TSlotType.Input, TSlotDataType.RGBA, 1);

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 2);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TChannelSourceParameter outputRedSource;
        public TChannelSourceParameter OutputRedSource
        {
            get
            {
                return outputRedSource;
            }
            set
            {
                outputRedSource = value;
            }
        }

        [SerializeField]
        private TChannelSourceParameter outputGreenSource;
        public TChannelSourceParameter OutputGreenSource
        {
            get
            {
                return outputGreenSource;
            }
            set
            {
                outputGreenSource = value;
            }
        }

        [SerializeField]
        private TChannelSourceParameter outputBlueSource;
        public TChannelSourceParameter OutputBlueSource
        {
            get
            {
                return outputBlueSource;
            }
            set
            {
                outputBlueSource = value;
            }
        }

        [SerializeField]
        private TChannelSourceParameter outputAlphaSource;
        public TChannelSourceParameter OutputAlphaSource
        {
            get
            {
                return outputAlphaSource;
            }
            set
            {
                outputAlphaSource = value;
            }
        }

        public TChannelsShuffleNode() : base()
        {
            outputRedSource = new TChannelSourceParameter() { value = TChannelSource.R0 };
            outputGreenSource = new TChannelSourceParameter() { value = TChannelSource.G0 };
            outputBlueSource = new TChannelSourceParameter() { value = TChannelSource.B0 };
            outputAlphaSource = new TChannelSourceParameter() { value = TChannelSource.A0 };
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

            TSlotReference inputRef0 = TSlotReference.Create(GUID, inputSlot0.Id);
            material.SetTexture(TConst.TEXTURE_0, context.GetInputTexture(inputRef0));
            TSlotReference connectedInputRef0 = context.GetInputLink(inputRef0);
            TSlot connectedInputSlot0 = context.GetSlot(connectedInputRef0);
            if (connectedInputSlot0 != null && connectedInputSlot0.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR_0);
            }
            else
            {
                material.DisableKeyword(TConst.RRR_0);
            }

            TSlotReference inputRef1 = TSlotReference.Create(GUID, inputSlot1.Id);
            material.SetTexture(TConst.TEXTURE_1, context.GetInputTexture(inputRef1));
            TSlotReference connectedInputRef1 = context.GetInputLink(inputRef1);
            TSlot connectedInputSlot1 = context.GetSlot(connectedInputRef1);
            if (connectedInputSlot1 != null && connectedInputSlot1.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR_1);
            }
            else
            {
                material.DisableKeyword(TConst.RRR_1);
            }

            material.SetVector(TConst.CHANNEL_SOURCE, new Vector4((int)outputRedSource.value, (int)outputGreenSource.value, (int)outputBlueSource.value, (int)outputAlphaSource.value));
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);
        }
    }
}
