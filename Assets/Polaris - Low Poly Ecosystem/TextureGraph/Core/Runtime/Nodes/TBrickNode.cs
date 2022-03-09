using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Brick",
        CreationMenu = "Shapes & Patterns/Brick",
        Icon = "TextureGraph/NodeIcons/Brick",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.2pjx79i9ffei")]
    public class TBrickNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Brick";
            public static readonly int UV_TO_CELL_MATRIX = Shader.PropertyToID("_UvToCellMatrix");
            public static readonly int CELL_TO_BRICK_MATRIX = Shader.PropertyToID("_CellToBrickMatrix");
            public static readonly int GAP_SIZE = Shader.PropertyToID("_GapSize");
            public static readonly int INNER_SIZE = Shader.PropertyToID("_InnerSize");

            public static readonly int PASS = 0;
        }

        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.Gray, 0);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TIntParameter tiling;
        public TIntParameter Tiling
        {
            get
            {
                return tiling;
            }
            set
            {
                tiling.value = Mathf.Max(1, value.value);
            }
        }

        [SerializeField]
        private TFloatParameter gapSize;
        public TFloatParameter GapSize
        {
            get
            {
                return gapSize;
            }
            set
            {
                gapSize.value = Mathf.Clamp01(value.value);
            }
        }

        [SerializeField]
        private TFloatParameter innerSize;
        public TFloatParameter InnerSize
        {
            get
            {
                return innerSize;
            }
            set
            {
                innerSize.value = Mathf.Clamp01(value.value);
            }
        }

        public TBrickNode() : base()
        {
            tiling = new TIntParameter() { value = 2 };
            gapSize = new TFloatParameter() { value = 0.1f };
            innerSize = new TFloatParameter() { value = 1 };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            Vector3 t = Vector3.zero;
            Quaternion r = Quaternion.identity;
            Vector3 s = new Vector3(1, 2f, 1) * tiling.value;
            Matrix4x4 uvToCellMatrix = Matrix4x4.TRS(t, r, s);
            material.SetMatrix(TConst.UV_TO_CELL_MATRIX, uvToCellMatrix);
            material.SetFloat(TConst.GAP_SIZE, gapSize.value);
            material.SetFloat(TConst.INNER_SIZE, innerSize.value);

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }
    }
}
