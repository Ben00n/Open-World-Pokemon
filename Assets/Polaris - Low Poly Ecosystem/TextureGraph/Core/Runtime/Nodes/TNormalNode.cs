using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Normal", 
        CreationMenu = "Basic/Normal", 
        Icon = "TextureGraph/NodeIcons/Normal",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.pdrk15n6ohgq")]
    public class TNormalNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Normal";
            public static readonly int HEIGHT_MAP = Shader.PropertyToID("_HeightMap");
            public static readonly int NORMAL_STRENGTH = Shader.PropertyToID("_Strength");

            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.Gray, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;


        [SerializeField]
        private TFloatParameter strength;
        public TFloatParameter Strength
        {
            get
            {
                return strength;
            }
            set
            {
                strength.value = Mathf.Max(0, value.value);
            }
        }

        public TNormalNode() : base()
        {
            strength = new TFloatParameter() { value = 1f };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            material.SetTexture(TConst.HEIGHT_MAP, context.GetInputTexture(TSlotReference.Create(GUID, inputSlot.Id)));
            material.SetFloat(TConst.NORMAL_STRENGTH, strength.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
