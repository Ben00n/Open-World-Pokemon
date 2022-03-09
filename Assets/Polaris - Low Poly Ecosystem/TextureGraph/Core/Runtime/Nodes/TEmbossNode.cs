using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Emboss",
        CreationMenu = "Effects/Emboss",
        Icon = "TextureGraph/NodeIcons/Emboss",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.bljgb9ryzbsc")]
    public class TEmbossNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Emboss";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int HEIGHT_MAP = Shader.PropertyToID("_HeightMap");
            public static readonly int INTENSITY = Shader.PropertyToID("_Intensity");
            public static readonly int LIGHT_DIRECTION = Shader.PropertyToID("_LightDirection");
            public static readonly int HIGHTLIGHT_COLOR = Shader.PropertyToID("_HighlightColor");
            public static readonly int SHADOW_COLOR = Shader.PropertyToID("_ShadowColor");

            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot mainTextureSlot = new TSlot("Main Texture", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot heightMapSlot = new TSlot("Height Map", TSlotType.Input, TSlotDataType.Gray, 1);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 2);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TFloatParameter lightAngle;
        public TFloatParameter LightAngle
        {
            get
            {
                return lightAngle;
            }
            set
            {
                lightAngle.value = Mathf.Clamp(value.value, 0f, 360f);
            }
        }

        [SerializeField]
        private TFloatParameter intensity;
        public TFloatParameter Intensity
        {
            get
            {
                return intensity;
            }
            set
            {
                intensity.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TColorParameter highlightColor;
        public TColorParameter HighlightColor
        {
            get
            {
                return highlightColor;
            }
            set
            {
                highlightColor = value;
            }
        }

        [SerializeField]
        private TColorParameter shadowColor;
        public TColorParameter ShadowColor
        {
            get
            {
                return shadowColor;
            }
            set
            {
                shadowColor = value;
            }
        }

        public TEmbossNode() : base()
        {
            lightAngle = new TFloatParameter() { value = 0 };
            intensity = new TFloatParameter() { value = 0.1f };
            highlightColor = new TColorParameter() { value = Color.white };
            shadowColor = new TColorParameter() { value = Color.black };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            TSlotReference mainTexRef = TSlotReference.Create(GUID, mainTextureSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(mainTexRef));
            TSlotReference connectedMainTexRef = context.GetInputLink(mainTexRef);
            TSlot connectedMainTexSlot = context.GetSlot(connectedMainTexRef);
            if (connectedMainTexSlot != null && connectedMainTexSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            TSlotReference heightMapRef = TSlotReference.Create(GUID, heightMapSlot.Id);
            material.SetTexture(TConst.HEIGHT_MAP, context.GetInputTexture(heightMapRef));

            float lightAngleRad = lightAngle.value * Mathf.Deg2Rad;
            Vector4 lightDir = new Vector4(Mathf.Cos(lightAngleRad), Mathf.Sin(lightAngleRad), 0, 0);
            material.SetVector(TConst.LIGHT_DIRECTION, lightDir);
            material.SetFloat(TConst.INTENSITY, intensity.value);
            material.SetColor(TConst.HIGHTLIGHT_COLOR, highlightColor.value);
            material.SetColor(TConst.SHADOW_COLOR, shadowColor.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
