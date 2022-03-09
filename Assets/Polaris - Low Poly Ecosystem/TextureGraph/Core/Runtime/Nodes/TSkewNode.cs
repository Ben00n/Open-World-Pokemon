using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Skew",
        CreationMenu = "Transformation/Skew",
        Icon = "TextureGraph/NodeIcons/Skew",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.2oqp7txu2q5s")]
    public class TSkewNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Skew";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int SKEW_MATRIX = Shader.PropertyToID("_SkewMatrix");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
            public static readonly Vector2[] UVS = new Vector2[] { new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1) };
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TAxisParameter skewAxis;
        public TAxisParameter SkewAxis
        {
            get
            {
                return skewAxis;
            }
            set
            {
                skewAxis = value;
            }
        }

        [SerializeField]
        private TFloatParameter skewAmount;
        public TFloatParameter SkewAmount
        {
            get
            {
                return skewAmount;
            }
            set
            {
                skewAmount.value = Mathf.Clamp(value.value, -1f, 1f);
            }
        }

        public TSkewNode() : base()
        {
            skewAxis = new TAxisParameter() { value = TAxis.Horizontal };
            skewAmount = new TFloatParameter() { value = 0 };
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

            Matrix4x4 skewMatrix = Matrix4x4.identity;
            if (skewAxis.value == TAxis.Horizontal)
            {
                skewMatrix[0, 1] = Mathf.Tan(skewAmount.value * Mathf.PI * 0.5f);
            }
            else
            {
                skewMatrix[1, 0] = Mathf.Tan(skewAmount.value * Mathf.PI * 0.5f);
            }
            Matrix4x4 uvRemap = Matrix4x4.TRS(Vector3.one * 0.5f, Quaternion.identity, Vector3.one * 0.5f);

            material.SetMatrix(TConst.SKEW_MATRIX, uvRemap * skewMatrix);
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, TConst.UVS, material, TConst.PASS);
        }
    }
}
