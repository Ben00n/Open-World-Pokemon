using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Alpha Merge",
        CreationMenu = "Basic/Alpha Merge",
        Icon = "TextureGraph/NodeIcons/AlphaMerge",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.94hr7z96nh0j")]
    public class TAlphaMergeNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/AlphaMerge";
            public static readonly int RGB_TEX = Shader.PropertyToID("_RgbTex");
            public static readonly int ALPHA_TEX = Shader.PropertyToID("_AlphaTex");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot rgbSlot = new TSlot("RGB", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot alphaSlot = new TSlot("A", TSlotType.Input, TSlotDataType.Gray, 1);
        public readonly TSlot rgbaSlot = new TSlot("RGBA", TSlotType.Output, TSlotDataType.RGBA, 2);

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

            TSlotReference rgbSlotRef = TSlotReference.Create(GUID, rgbSlot.Id);
            material.SetTexture(TConst.RGB_TEX, context.GetInputTexture(rgbSlotRef));
            TSlotReference connectedRgbSlotRef = context.GetInputLink(rgbSlotRef);
            TSlot connectedRgbSlot = context.GetSlot(connectedRgbSlotRef);
            if (connectedRgbSlot != null && connectedRgbSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            material.SetTexture(TConst.ALPHA_TEX, context.GetInputTexture(TSlotReference.Create(GUID, alphaSlot.Id)));

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
