using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Perlin Noise", 
        CreationMenu = "Noise/Perlin Noise", 
        Icon = "TextureGraph/NodeIcons/PerlinNoise",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.iy77afv0gtiz")]
    public class TPerlinNoiseNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/PerlinNoise";
            public static readonly int SCALE = Shader.PropertyToID("_Scale");
            public static readonly int SEED = Shader.PropertyToID("_Seed");
            public static readonly int VARIANT = Shader.PropertyToID("_VariantMatrix");
        }

        public readonly TSlot outputSlot = new TSlot("Noise Map", TSlotType.Output, TSlotDataType.Gray, 0);

        [SerializeField]
        private TIntParameter scale;
        public TIntParameter Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TIntParameter seed;
        public TIntParameter Seed
        {
            get
            {
                return seed;
            }
            set
            {
                seed.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TFloatParameter variant;
        public TFloatParameter Variant
        {
            get
            {
                return variant;
            }
            set
            {
                variant.value = Mathf.Clamp01(value.value);
            }
        }

        protected virtual int ShaderPass => 0;

        private Shader shader;
        private Material material;

        public TPerlinNoiseNode() : base()
        {
            scale = new TIntParameter() { value = 5 };
            seed = new TIntParameter() { value = 1 };
            variant = new TFloatParameter() { value = 0 };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            material.SetFloat(TConst.SCALE, Scale.value);
            material.SetFloat(TConst.SEED, Seed.value);
            material.SetMatrix(TConst.VARIANT, Matrix4x4.Rotate(Quaternion.Euler(0, 0, Variant.value * 360)));
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, ShaderPass);
        }
    }
}
