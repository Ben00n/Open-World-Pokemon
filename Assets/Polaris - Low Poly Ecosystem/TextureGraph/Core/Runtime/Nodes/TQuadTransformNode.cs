using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEngine.Rendering;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Quad Transform",
        CreationMenu = "Transformation/Quad Transform",
        Icon = "TextureGraph/NodeIcons/QuadTransform",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.gay50n1w4mx1")]
    public class TQuadTransformNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/QuadTransform";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int CULL = Shader.PropertyToID("_Cull");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TVector2Parameter[] quad;
        public TVector2Parameter[] Quad
        {
            get
            {
                return quad;
            }
            set
            {
                if (value.Length != 4)
                {
                    throw new System.ArgumentException("The quad must have exact 4 points");
                }
                quad = value;
            }
        }

        [SerializeField]
        private TColorParameter backgroundColor;
        public TColorParameter BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        [SerializeField]
        private TCullModeParameter cullMode;
        public TCullModeParameter CullMode
        {
            get
            {
                return cullMode;
            }
            set
            {
                cullMode = value;
            }
        }

        [SerializeField]
        private TBoolParameter flipOrder;
        public TBoolParameter FlipOrder
        {
            get
            {
                return flipOrder;
            }
            set
            {
                flipOrder = value;
            }
        }

        public TQuadTransformNode() : base()
        {
            quad = new TVector2Parameter[4];
            quad[0] = new TVector2Parameter() { value = new Vector2(0, 0) };
            quad[1] = new TVector2Parameter() { value = new Vector2(0, 1) };
            quad[2] = new TVector2Parameter() { value = new Vector2(1, 1) };
            quad[3] = new TVector2Parameter() { value = new Vector2(1, 0) };

            backgroundColor = new TColorParameter() { value = Color.black };
            cullMode = new TCullModeParameter() { value = UnityEngine.Rendering.CullMode.Off };
            flipOrder = new TBoolParameter() { value = false };
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

            material.SetFloat(TConst.CULL, (float)cullMode.value);

            Vector2[] vertices;
            Vector2[] uvs;
            if (flipOrder.value == true)
            {
                vertices = new Vector2[6]
                {
                    quad[0].value, quad[2].value, quad[3].value, quad[0].value, quad[1].value, quad[2].value
                };
                uvs = new Vector2[6]
                {
                    Vector2.zero, Vector2.one, Vector2.right, Vector2.zero, Vector2.up, Vector2.one
                };
            }
            else
            {
                vertices = new Vector2[6]
                {
                    quad[0].value, quad[1].value, quad[2].value, quad[0].value,quad[2].value, quad[3].value
                };
                uvs = new Vector2[6]
                {
                    Vector2.zero, Vector2.up, Vector2.one, Vector2.zero, Vector2.one, Vector2.right
                };
            }
            TDrawing.FillTexture(targetRT, BackgroundColor.value);
            TDrawing.DrawDoubleTris(targetRT, vertices, uvs, material, TConst.PASS);
        }
    }
}
