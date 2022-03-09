using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "RGBA Split",
        CreationMenu = "Basic/RGBA Split",
        Icon = "TextureGraph/NodeIcons/RGBASplit",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.mhjbj3wz4b9h")]
    public class TRgbaSplitNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/RGBASplit";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly string RRR = "RRR";
            public static readonly int PASS_R = 0;
            public static readonly int PASS_G = 1;
            public static readonly int PASS_B = 2;
            public static readonly int PASS_A = 3;
        }

        public readonly TSlot rgbaSlot = new TSlot("RGBA", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot redSlot = new TSlot("R", TSlotType.Output, TSlotDataType.Gray, 1);
        public readonly TSlot greenSlot = new TSlot("G", TSlotType.Output, TSlotDataType.Gray, 2);
        public readonly TSlot blueSlot = new TSlot("B", TSlotType.Output, TSlotDataType.Gray, 3);
        public readonly TSlot alphaSlot = new TSlot("A", TSlotType.Output, TSlotDataType.Gray, 4);

        private Shader shader;
        private Material material;

        public override TSlot GetMainOutputSlot()
        {
            return redSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT_R = context.RequestTargetRT(TSlotReference.Create(GUID, redSlot.Id), GetRenderTextureRequest(redSlot));
            RenderTexture targetRT_G = context.RequestTargetRT(TSlotReference.Create(GUID, greenSlot.Id), GetRenderTextureRequest(greenSlot));
            RenderTexture targetRT_B = context.RequestTargetRT(TSlotReference.Create(GUID, blueSlot.Id), GetRenderTextureRequest(blueSlot));
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


            TDrawing.DrawFullQuad(targetRT_R, material, TConst.PASS_R);
            TDrawing.DrawFullQuad(targetRT_G, material, TConst.PASS_G);
            TDrawing.DrawFullQuad(targetRT_B, material, TConst.PASS_B);
            TDrawing.DrawFullQuad(targetRT_A, material, TConst.PASS_A);
        }
    }
}
