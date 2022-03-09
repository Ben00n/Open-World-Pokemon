using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gradient Axial",
        CreationMenu = "Shapes & Patterns/Gradient Axial",
        Icon = "TextureGraph/NodeIcons/GradientAxial",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.6az5dlk86rr5")]
    public class TGradientAxialNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GradientAxial";
            public static readonly int UV_TO_LINE_MATRIX = Shader.PropertyToID("_UvToLineMatrix");
            public static readonly string REFLECTED = "REFLECTED";
            public static readonly int PASS = 0;
        }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 0);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TVector2Parameter startPoint;
        public TVector2Parameter StartPoint
        {
            get
            {
                return startPoint;
            }
            set
            {
                startPoint = value;
            }
        }

        [SerializeField]
        private TVector2Parameter endPoint;
        public TVector2Parameter EndPoint
        {
            get
            {
                return endPoint;
            }
            set
            {
                endPoint = value;
            }
        }

        [SerializeField]
        private TBoolParameter reflected;
        public TBoolParameter Reflected
        {
            get
            {
                return reflected;
            }
            set
            {
                reflected = value;
            }
        }

        public TGradientAxialNode() : base()
        {
            startPoint = new TVector2Parameter() { value = new Vector2(0f, 0f) };
            endPoint = new TVector2Parameter() { value = new Vector2(1f, 1f) };
            reflected = new TBoolParameter() { value = false };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            Vector3 pos = startPoint.value;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, Vector3.Normalize(endPoint.value - startPoint.value));
            float d = Vector3.Distance(startPoint.value, endPoint.value);
            Vector3 scale = new Vector3(d, d, 1);
            Matrix4x4 uvToLineMatrix = Matrix4x4.TRS(pos, rotation, scale).inverse;
            material.SetMatrix(TConst.UV_TO_LINE_MATRIX, uvToLineMatrix);

            if (Reflected.value == true)
            {
                material.EnableKeyword(TConst.REFLECTED);
            }
            else
            {
                material.DisableKeyword(TConst.REFLECTED);
            }

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
