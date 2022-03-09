using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Checkerboard",
        CreationMenu = "Shapes & Patterns/Checkerboard",
        Icon = "TextureGraph/NodeIcons/Checkerboard",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.fdfb7ce6cbp6")]
    public class TCheckerboardNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Checkerboard";
            public static readonly int SCALE = Shader.PropertyToID("_Scale");
            public static readonly int COLOR_0 = Shader.PropertyToID("_Color0");
            public static readonly int COLOR_1 = Shader.PropertyToID("_Color1");
            public static readonly int PASS = 0;
        }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 0);

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
                int v = value.value;
                v = Mathf.Max(1, v);
                scale.value = v;
            }
        }

        [SerializeField]
        private TColorParameter color0;
        public TColorParameter Color0
        {
            get
            {
                return color0;
            }
            set
            {
                color0 = value;
            }
        }

        [SerializeField]
        private TColorParameter color1;
        public TColorParameter Color1
        {
            get
            {
                return color1;
            }
            set
            {
                color1 = value;
            }
        }

        private Shader shader;
        private Material material;


        public TCheckerboardNode() : base()
        {
            scale = new TIntParameter() { value = 2 };
            color0 = new TColorParameter() { value = Color.white };
            color1 = new TColorParameter() { value = Color.black };
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
            material.SetColor(TConst.COLOR_0, Color0.value);
            material.SetColor(TConst.COLOR_1, Color1.value);
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);
        }
    }
}
