using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using System.IO;

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Output",
        CreationMenu = "Output/Output",
        Icon = "TextureGraph/NodeIcons/Output",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.ck8hpy1ldflr")]
    public sealed class TOutputNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string SHADER = "Hidden/TextureGraph/Output";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Input", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        private Shader shader;
        private Material material;

        [SerializeField]
        private TStringParameter outputId;
        public TStringParameter OutputId
        {
            get
            {
                return outputId;
            }
            set
            {
                outputId = value;
            }
        }

        [SerializeField]
        private TTextureUsageParameter usage;
        public TTextureUsageParameter Usage
        {
            get
            {
                return usage;
            }
            set
            {
                usage = value;
            }
        }

        public TOutputNode() : base()
        {
            outputId = new TStringParameter() { value = "output" };
            usage = new TTextureUsageParameter() { value = TTextureUsage.None };
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
            TSlot connectedInputSlot = context.GetSlot(connectedInputRef);
            if (connectedInputSlot != null && connectedInputSlot.DataType == TSlotDataType.Gray)
            {
                material.EnableKeyword(TConst.RRR);
            }
            else
            {
                material.DisableKeyword(TConst.RRR);
            }

            TDrawing.DrawFullQuad(targetRT, material, TConst.PASS);
        }

        public Vector2Int GetOutputResolution(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            if (targetRT == null)
            {
                return Vector2Int.zero;
            }
            else
            {
                return new Vector2Int(targetRT.width, targetRT.height);
            }
        }

        public void SaveToImage(TGraphContext context, string filePathNoExt, TImageExtension ext)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            if (targetRT == null)
            {
                Debug.Log("Failed to save output to file. The node has not been executed.");
                return;
            }

            TextureFormat format = targetRT.format == RenderTextureFormat.ARGBFloat ?
                TextureFormat.RGBAFloat :
                TextureFormat.RGBA32;
            Texture2D tex = new Texture2D(targetRT.width, targetRT.height, format, false);
            TDrawing.CopyFromRT(tex, targetRT);

            byte[] data = null;
            if (ext == TImageExtension.PNG)
            {
                data = tex.EncodeToPNG();
            }
            else if (ext == TImageExtension.JPG)
            {
                data = tex.EncodeToJPG();
            }
            else if (ext == TImageExtension.EXR)
            {
                data = tex.EncodeToEXR();
            }
            else if (ext == TImageExtension.TGA)
            {
                data = tex.EncodeToTGA();
            }
            if (data == null)
            {
                throw new System.Exception("File extension not supported");
            }

            string filePath = filePathNoExt + "." + ext.ToString().ToLower();
            File.WriteAllBytes(filePath, data);
            TUtilities.DestroyObject(tex);
        }
    }
}
