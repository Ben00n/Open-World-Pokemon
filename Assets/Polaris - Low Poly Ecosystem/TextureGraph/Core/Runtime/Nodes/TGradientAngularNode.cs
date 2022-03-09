using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Gradient Angular",
        CreationMenu = "Shapes & Patterns/Gradient Angular",
        Icon = "TextureGraph/NodeIcons/GradientAngular",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.x5s7jn5y7v11")]
    public class TGradientAngularNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/GradientAngular";
            public static readonly int CENTER = Shader.PropertyToID("_Center");
            public static readonly int END_POINT = Shader.PropertyToID("_EndPoint");
            public static readonly int TRANSFORM_MATRIX = Shader.PropertyToID("_TransformMatrix");
            public static readonly int PASS = 0;
        }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 0);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TVector2Parameter centerPoint;
        public TVector2Parameter CenterPoint
        {
            get
            {
                return centerPoint;
            }
            set
            {
                centerPoint = value;
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

        public TGradientAngularNode() : base()
        {
            centerPoint = new TVector2Parameter() { value = new Vector2(0.5f, 0.5f) };
            endPoint = new TVector2Parameter() { value = new Vector2(1f, 0.5f) };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            Vector3 pos = centerPoint.value;
            Quaternion rotation = Quaternion.FromToRotation(Vector3.right, Vector3.Normalize(endPoint.value - centerPoint.value));
            Vector3 scale = Vector3.one;
            Matrix4x4 transformMatrix = Matrix4x4.TRS(pos, rotation, scale).inverse;
            material.SetMatrix(TConst.TRANSFORM_MATRIX, transformMatrix);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
