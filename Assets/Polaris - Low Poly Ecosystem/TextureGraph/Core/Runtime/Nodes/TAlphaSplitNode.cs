using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Alpha Split",
        CreationMenu = "Basic/Alpha Split",
        Icon = "TextureGraph/NodeIcons/AlphaSplit",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.tnavvdw83jgm")]
    public class TAlphaSplitNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/AlphaSplit";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly string RRR = "RRR";
            public static readonly int PASS_RGB = 0;
            public static readonly int PASS_A = 1;
        }

        public readonly TSlot rgbaSlot = new TSlot("RGBA", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot rgbSlot = new TSlot("RGB", TSlotType.Output, TSlotDataType.RGBA, 1);
        public readonly TSlot alphaSlot = new TSlot("A", TSlotType.Output, TSlotDataType.Gray, 2);

        private Shader shader;
        private Material material;

        public override TSlot GetMainOutputSlot()
        {
            return rgbSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT_RGB = context.RequestTargetRT(TSlotReference.Create(GUID, rgbSlot.Id), GetRenderTextureRequest(rgbSlot));
            RenderTexture targetRT_A = context.RequestTargetRT(TSlotReference.Create(GUID, alphaSlot.Id), GetRenderTextureRequest(alphaSlot));

            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);
            TSlotReference rgbaSlotRef = TSlotReference.Create(GUID, rgbaSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(rgbaSlotRef));
            TSlotReference connectedRgbaSlotRef = context.GetInputLink(rgbaSlotRef);
            TSlot connectedRgbaSlot = context.GetSlot(connectedRgbaSlotRef);
            if (connectedRgbaSlot != null && connectedRgbaSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            TDrawing.DrawFullQuad(targetRT_RGB, material, TConst.PASS_RGB);
            TDrawing.DrawFullQuad(targetRT_A, material, TConst.PASS_A);
        }
    }
}
