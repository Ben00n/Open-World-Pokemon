using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Curve",
        CreationMenu = "Filter/Curve",
        Icon = "TextureGraph/NodeIcons/Curve",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.sxc45wjan3iq")]
    public class TCurveNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Curve";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int CURVE_RGB = Shader.PropertyToID("_CurveRGB");
            public static readonly int CURVE_R = Shader.PropertyToID("_CurveR");
            public static readonly int CURVE_G = Shader.PropertyToID("_CurveG");
            public static readonly int CURVE_B = Shader.PropertyToID("_CurveB");
            public static readonly int CURVE_A = Shader.PropertyToID("_CurveA");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TCurveParameter curveRGB;
        public TCurveParameter CurveRGB
        {
            get
            {
                if (curveRGB.value == null)
                {
                    curveRGB.value = AnimationCurve.Linear(0, 0, 1, 1);
                }
                return curveRGB;
            }
            set
            {
                curveRGB = value;
            }
        }

        [SerializeField]
        private TCurveParameter curveR;
        public TCurveParameter CurveR
        {
            get
            {
                if (curveR.value == null)
                {
                    curveR.value = AnimationCurve.Linear(0, 0, 1, 1);
                }
                return curveR;
            }
            set
            {
                curveR = value;
            }
        }

        [SerializeField]
        private TCurveParameter curveG;
        public TCurveParameter CurveG
        {
            get
            {
                if (curveG.value == null)
                {
                    curveG.value = AnimationCurve.Linear(0, 0, 1, 1);
                }
                return curveG;
            }
            set
            {
                curveG = value;
            }
        }

        [SerializeField]
        private TCurveParameter curveB;
        public TCurveParameter CurveB
        {
            get
            {
                if (curveB.value == null)
                {
                    curveB.value = AnimationCurve.Linear(0, 0, 1, 1);
                }
                return curveB;
            }
            set
            {
                curveB = value;
            }
        }

        [SerializeField]
        private TCurveParameter curveA;
        public TCurveParameter CurveA
        {
            get
            {
                if (curveA.value == null)
                {
                    curveA.value = AnimationCurve.Linear(0, 0, 1, 1);
                }
                return curveA;
            }
            set
            {
                curveA = value;
            }
        }

        public TCurveNode() : base()
        {
            curveRGB = new TCurveParameter();
            curveR = new TCurveParameter();
            curveG = new TCurveParameter();
            curveB = new TCurveParameter();
            curveA = new TCurveParameter();
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

            Texture2D texRGB = CreateCurveTexture(CurveRGB.value);
            Texture2D texR = CreateCurveTexture(CurveR.value);
            Texture2D texG = CreateCurveTexture(CurveG.value);
            Texture2D texB = CreateCurveTexture(CurveB.value);
            Texture2D texA = CreateCurveTexture(CurveA.value);

            TSlotReference inputRef = TSlotReference.Create(GUID, inputSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(inputRef));
            material.SetTexture(TConst.CURVE_RGB, texRGB);
            material.SetTexture(TConst.CURVE_R, texR);
            material.SetTexture(TConst.CURVE_G, texG);
            material.SetTexture(TConst.CURVE_B, texB);
            material.SetTexture(TConst.CURVE_A, texA);

            TSlotReference connectedInputRef = context.GetInputLink(inputRef);
            TSlot connectedInputSlot = context.GetSlot(connectedInputRef);
            if (connectedInputSlot != null && connectedInputSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);

            TUtilities.DestroyObject(texRGB);
            TUtilities.DestroyObject(texR);
            TUtilities.DestroyObject(texG);
            TUtilities.DestroyObject(texB);
            TUtilities.DestroyObject(texA);
        }

        private Texture2D CreateCurveTexture(AnimationCurve curve)
        {
            int width = 1024;
            Texture2D tex = new Texture2D(width, 1, TextureFormat.RGBAFloat, false, true);
            Color[] colors = new Color[width];
            for (int i = 0; i < width; ++i)
            {
                float f = i * 1.0f / (width - 1);
                Color c = Color.white * curve.Evaluate(f);
                colors[i] = c;
            }
            tex.SetPixels(colors);
            tex.Apply();
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            return tex;
        }
    }
}
