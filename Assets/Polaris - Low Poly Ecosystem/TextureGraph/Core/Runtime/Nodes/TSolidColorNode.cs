using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEngine.UIElements;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Solid Color",
        CreationMenu = "Basic/Solid Color",
        Icon = "TextureGraph/NodeIcons/SolidColor",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.5p39mjqy159i")]
    public class TSolidColorNode : TAbstractTextureNode
    {
        private class TConst
        {
            public static readonly string SHADER_NAME = "Hidden/TextureGraph/SolidColor";
            public static readonly int COLOR = Shader.PropertyToID("_Color");
            public static readonly int PASS = 0;
        }

        public readonly TSlot outputSlot = new TSlot("Output Color", TSlotType.Output, TSlotDataType.RGBA, 0);

        [SerializeField]
        private TColorParameter color;
        public TColorParameter Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        private Shader shader;
        private Material material;

        public TSolidColorNode() : base()
        {
            color = new TColorParameter() { value = new Color(1, 1, 1, 1) };
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
                shader = Shader.Find(TConst.SHADER_NAME);
            }
            if (material == null)
            {
                material = new Material(shader);
            }

            material.SetColor(TConst.COLOR, Color.value);
            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);
        }
    }
}
