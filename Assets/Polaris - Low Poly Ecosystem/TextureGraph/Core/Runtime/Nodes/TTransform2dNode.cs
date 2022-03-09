using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Transform 2D",
        CreationMenu = "Transformation/Transform 2D",
        Icon = "TextureGraph/NodeIcons/Transform2D",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.fno6qxisy9lm")]
    public class TTransform2dNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Transform2D";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly int TRANSFORM_MATRIX = Shader.PropertyToID("_TransformMatrix");
            public static readonly int BACKGROUND_COLOR = Shader.PropertyToID("_BackgroundColor");

            public static readonly string RRR = "RRR";
            public static readonly string TILE_X = "TILE_X";
            public static readonly string TILE_Y = "TILE_Y";
            public static readonly int PASS = 0;
            public static readonly Vector2[] UVS = new Vector2[] { new Vector2(-1, -1), new Vector2(-1, 1), new Vector2(1, 1), new Vector2(1, -1) };
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TTilingModeParameter tilingMode;
        public TTilingModeParameter TilingMode
        {
            get
            {
                return tilingMode;
            }
            set
            {
                tilingMode = value;
            }
        }

        [SerializeField]
        private TVector2Parameter offset;
        public TVector2Parameter Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
            }
        }

        [SerializeField]
        private TFloatParameter rotation;
        public TFloatParameter Rotation
        {
            get
            {
                return rotation;
            }
            set
            {
                rotation = value;
            }
        }

        [SerializeField]
        private TVector2Parameter scale;
        public TVector2Parameter Scale
        {
            get
            {
                return scale;
            }
            set
            {
                scale = value;
            }
        }

        [SerializeField]
        private TColorParameter backgroundColor;
        public TColorParameter BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
            }
        }

        public TTransform2dNode() : base()
        {
            tilingMode = new TTilingModeParameter() { value = TTilingMode.TileXY };
            offset = new TVector2Parameter() { value = Vector2.zero };
            rotation = new TFloatParameter() { value = 0 };
            scale = new TVector2Parameter() { value = Vector2.one * 100 };
            backgroundColor = new TColorParameter() { value = Color.black };
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            TGraphUtilities.PrepareShaderAndMaterial(TConst.SHADER, ref shader, ref material);

            TSlotReference inputRef = TSlotReference.Create(GUID, inputSlot.Id);
            material.SetTexture(TConst.MAIN_TEX, context.GetInputTexture(inputRef));
            TSlotReference connectedInputRef = context.GetInputLink(inputRef);
            TSlot connectedSlot = context.GetSlot(connectedInputRef);
            if (connectedSlot != null && connectedSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            if (tilingMode.value == TTilingMode.TileX || tilingMode.value == TTilingMode.TileXY)
            {
                material.EnableKeyword(TConst.TILE_X);
            }
            else
            {
                material.DisableKeyword(TConst.TILE_X);
            }

            if (tilingMode.value == TTilingMode.TileY || tilingMode.value == TTilingMode.TileXY)
            {
                material.EnableKeyword(TConst.TILE_Y);
            }
            else
            {
                material.DisableKeyword(TConst.TILE_Y);
            }

            Matrix4x4 transformMatrix = Matrix4x4.TRS(
                new Vector3(offset.value.x / 100f, offset.value.y / 100f, 0),
                Quaternion.Euler(0, 0, rotation.value),
                new Vector3(scale.value.x / 100f, scale.value.y / 100f, 1f)).inverse;
            Matrix4x4 uvRemap = Matrix4x4.TRS(Vector3.one * 0.5f, Quaternion.identity, Vector3.one * 0.5f);
            material.SetMatrix(TConst.TRANSFORM_MATRIX, uvRemap * transformMatrix);
            material.SetColor(TConst.BACKGROUND_COLOR, BackgroundColor.value);

            TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, TConst.UVS, material, TConst.PASS);
        }
    }
}
