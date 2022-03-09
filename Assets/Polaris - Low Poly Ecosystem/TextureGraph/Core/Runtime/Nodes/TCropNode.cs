using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Crop",
        CreationMenu = "Transformation/Crop",
        Icon = "TextureGraph/NodeIcons/Crop",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.giqho89dcavr")]
    public class TCropNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Crop";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int RECT = Shader.PropertyToID("_Rect");
            public static readonly int BACKGROUND_COLOR = Shader.PropertyToID("_BackgroundColor");
            public static readonly int UV_TO_RECT = Shader.PropertyToID("_UvToRectMatrix");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TVector2Parameter rectCenter;
        public TVector2Parameter RectCenter
        {
            get
            {
                return rectCenter;
            }
            set
            {
                Vector2 v = value.value;
                v.x = Mathf.Clamp(v.x, 0f, 100f);
                v.y = Mathf.Clamp(v.y, 0f, 100f);
                rectCenter.value = v;
            }
        }

        [SerializeField]
        private TFloatParameter rectRotation;
        public TFloatParameter RectRotation
        {
            get
            {
                return rectRotation;
            }
            set
            {
                rectRotation = value;
            }
        }

        [SerializeField]
        private TVector2Parameter rectSize;
        public TVector2Parameter RectSize
        {
            get
            {
                return rectSize;
            }
            set
            {
                Vector2 size = value.value;
                size.x = Mathf.Max(0.001f, size.x);
                size.y = Mathf.Max(0.001f, size.y);
                rectSize.value = size;
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

        public TCropNode() : base()
        {
            rectCenter = new TVector2Parameter() { value = new Vector2(0.5f, 0.5f) * 100f };
            rectRotation = new TFloatParameter() { value = 0 };
            rectSize = new TVector2Parameter() { value = new Vector2(1, 1) * 100f };
            backgroundColor = new TColorParameter() { value = Color.black };
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

            material.SetColor(TConst.BACKGROUND_COLOR, BackgroundColor.value);

            Matrix4x4 rectToUv = GetRectToUvMatrix();
            material.SetMatrix(TConst.UV_TO_RECT, rectToUv.inverse);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }

        public Matrix4x4 GetRectToUvMatrix()
        {
            Vector3 position = new Vector3(rectCenter.value.x / 100f, rectCenter.value.y / 100f, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, rectRotation.value);
            Vector3 scale = new Vector3(rectSize.value.x / 100f, rectSize.value.y / 100f, 1);
            return Matrix4x4.TRS(position, rotation, scale);
        }
    }
}
