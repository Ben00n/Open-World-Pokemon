using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Value Noise",
        CreationMenu = "Noise/Value Noise",
        Icon = "TextureGraph/NodeIcons/ValueNoise",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.9ka5yud1jhuc")]
    public class TValueNoiseNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/ValueNoise";
            public static readonly int SCALE = Shader.PropertyToID("_Scale");
            public static readonly int SEED = Shader.PropertyToID("_Seed");
            public static readonly int PASS = 0;
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

        private Shader shader;
        private Material material;

        public TValueNoiseNode() : base()
        {
            scale = new TIntParameter() { value = 5 };
            seed = new TIntParameter() { value = 1 };
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

            material.SetFloat(TConst.SCALE, Scale.value);
            material.SetFloat(TConst.SEED, Seed.value);
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);
        }
    }
}
