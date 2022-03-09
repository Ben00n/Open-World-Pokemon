using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using System.IO;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.TextureGraph
{
    [TNodeMetadata(
        Title = "Output To RT",
        CreationMenu = "Output/Output To RT",
        Icon = "TextureGraph/NodeIcons/OutputToRT",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.20acpz4z0fp2")]
    public class TOutputToRTNode : TAbstractTextureNode
    {
        public class TConst
        {
            public static readonly string RESOURCES_FOLDER = "Resources/";
            public static readonly string SHADER = "Hidden/TextureGraph/OutputToRT";
            public static readonly int MAIN_TEX = Shader.PropertyToID("_MainTex");
            public static readonly string RRR = "RRR";
            public static readonly int PASS = 0;
        }

        public readonly TSlot inputSlot = new TSlot("Texture", TSlotType.Input, TSlotDataType.RGBA, 0);
        public readonly TSlot outputSlot = new TSlot("Output", TSlotType.Output, TSlotDataType.RGBA, 1);

        [SerializeField]
        private TStringParameter filePath;
        public TStringParameter FilePath
        {
            get
            {
                return filePath;
            }
        }

        private Shader shader;
        private Material material;

        public TOutputToRTNode() : base()
        {
            filePath = new TStringParameter();
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture previewRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            CustomRenderTexture targetRT = null;
            if (string.IsNullOrEmpty(filePath.value))
            {
                targetRT = null;
            }
            else
            {
                if (filePath.value.StartsWith(TConst.RESOURCES_FOLDER))
                {
                    targetRT = Resources.Load<CustomRenderTexture>(filePath.value.Substring(TConst.RESOURCES_FOLDER.Length));
                }
                else
                {
#if UNITY_EDITOR
                    targetRT = AssetDatabase.LoadAssetAtPath<CustomRenderTexture>(filePath.value);
#endif
                }
            }

            if (shader == null)
            {
                shader = Shader.Find(TConst.SHADER);
            }
            if (material == null)
            {
                material = new Material(shader);
            }

            if (targetRT != null)
            {
                TSlotReference inputRef = TSlotReference.Create(GUID, inputSlot.Id);
                Texture src = context.GetInputTexture(inputRef);
                material.SetTexture(TConst.MAIN_TEX, src);

                TSlotReference connectedInputRef = context.GetInputLink(inputRef);
                TSlot connectedInput = context.GetSlot(connectedInputRef);
                if (connectedInput != null && connectedInput.DataType == TSlotDataType.Gray)
                {
                    material.EnableKeyword(TConst.RRR);
                }
                else
                {
                    material.DisableKeyword(TConst.RRR);
                }

                TDrawing.DrawQuad(targetRT, TDrawing.fullRectUvPoints, material, TConst.PASS);
                TDrawing.CopyToRT(targetRT, previewRT);
            }
            else
            {
                TDrawing.CopyToRT(Texture2D.blackTexture, previewRT);
            }
        }

        public void SetPath(string path)
        {
            int resourcesStringIndex = path.LastIndexOf(TConst.RESOURCES_FOLDER);
            if (resourcesStringIndex >= 0)
            {
                string resourcesPath = path.Substring(resourcesStringIndex);
                string ext = Path.GetExtension(resourcesPath);
                filePath.value = resourcesPath.Replace(ext, string.Empty);
            }
            else
            {
                filePath.value = path;
            }
        }
    }
}
