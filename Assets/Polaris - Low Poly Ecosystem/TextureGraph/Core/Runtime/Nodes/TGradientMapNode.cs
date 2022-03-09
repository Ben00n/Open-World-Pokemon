using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gradient Map",
        CreationMenu = "Basic/Gradient Map",
        Icon = "TextureGraph/NodeIcons/GradientMap",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.ujzvvqrn16tk")]
    public class TGradientMapNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GradientMap";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int GRADIENT_TEX = Shader.PropertyToID("_GradientTex");
            public static readonly int SCALE = Shader.PropertyToID("_Scale");
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.Gray, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TGradientParameter gradient;
        public TGradientParameter Gradient
        {
            get
            {
                if (gradient.value == null)
                {
                    gradient.value = TUtilities.CreateLinearBWGradient();
                }
                return gradient;
            }
            set
            {
                gradient = value;
            }
        }

        [SerializeField]
        private TFloatParameter scale;
        public TFloatParameter Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        [SerializeField]
        private TTextureWrapModeParameter wrapMode;
        public TTextureWrapModeParameter WrapMode
        {
            get
            {
                return wrapMode;
            }
            set
            {
                wrapMode = value;
            }
        }

        public TGradientMapNode() : base()
        {
            gradient = new TGradientParameter();
            scale = new TFloatParameter() { value = 1 };
            wrapMode = new TTextureWrapModeParameter() { value = TextureWrapMode.Clamp };
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

            Texture2D gradientTex = CreateGradientTex();
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(TSlotReference.Create(GUID, inputSlot.Id)));
            material.SetTexture(TConst.GRADIENT_TEX, gradientTex);
            material.SetFloat(TConst.SCALE, Scale.value);
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);

            TUtilities.DestroyObject(gradientTex);
        }

        private Texture2D CreateGradientTex()
        {
            int width = 1024;
            Texture2D t = new Texture2D(width, 1, TextureFormat.RGBAFloat, false, true);
            Color[] colors = new Color[width];
            for (int x = 0; x < width; ++x)
            {
                float f = x * 1.0f / (width - 1);
                Color c = Gradient.value.Evaluate(f);
                colors[x] = c;
            }

            t.SetPixels(colors);
            t.Apply();
            t.wrapMode = WrapMode.value;
            t.filterMode = FilterMode.Bilinear;

            return t;
        }
    }
}
