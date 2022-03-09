using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "RGBA Merge",
        CreationMenu = "Basic/RGBA Merge",
        Icon = "TextureGraph/NodeIcons/RGBAMerge",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.pes2y4dk8ov5")]
    public class TRgbaMergeNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/RGBAMerge";
            public static readonly int RED_TEX = Shader.PropertyToID("_RedTex");
            public static readonly int GREEN_TEX = Shader.PropertyToID("_GreenTex");
            public static readonly int BLUE_TEX = Shader.PropertyToID("_BlueTex");
            public static readonly int ALPHA_TEX = Shader.PropertyToID("_AlphaTex");
            public static readonly int PASS = 0;
        }

        public readonly TSlot redSlot = new TSlot("R", TSlotType.Input, TSlotDataType.Gray, 0);
        public readonly TSlot greenSlot = new TSlot("G", TSlotType.Input, TSlotDataType.Gray, 1);
        public readonly TSlot blueSlot = new TSlot("B", TSlotType.Input, TSlotDataType.Gray, 2);
        public readonly TSlot alphaSlot = new TSlot("A", TSlotType.Input, TSlotDataType.Gray, 3);
        public readonly TSlot rgbaSlot = new TSlot("RGBA", TSlotType.Output, TSlotDataType.RGBA, 4);

        private Shader shader;
        private Material material;

        public override TSlot GetMainOutputSlot()
        {
            return rgbaSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, rgbaSlot.Id), GetRenderTextureRequest(rgbaSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            material.SetTexture(TConst.RED_TEX, context.GetInputTexture(TSlotReference.Create(GUID, redSlot.Id)));
            material.SetTexture(TConst.GREEN_TEX, context.GetInputTexture(TSlotReference.Create(GUID, greenSlot.Id)));
            material.SetTexture(TConst.BLUE_TEX, context.GetInputTexture(TSlotReference.Create(GUID, blueSlot.Id)));
            material.SetTexture(TConst.ALPHA_TEX, context.GetInputTexture(TSlotReference.Create(GUID, alphaSlot.Id)));

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
