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
        Title = "Load Texture", 
        CreationMenu = "Basic/Load Texture", 
        Icon = "TextureGraph/NodeIcons/LoadTexture",
        Documentation = "https://docs.google.com/document/d/1kDkFHAmuMNFTbfnBpYPkjw99CrVvUNr8_gFAJH2o-7s/edit#heading=h.lhbrlxqzk8on")]
    public class TLoadTextureNode : TAbstractTextureNode
    {
        public class TConst
        {
            public const string RESOURCES_FOLDER = "Resources/";
        }

        public readonly TSlot outputSlot = new TSlot("Texture", TSlotType.Output, TSlotDataType.RGBA, 0);

        [SerializeField]
        private TStringParameter filePath;
        public TStringParameter FilePath
        {
            get
            {
                return filePath;
            }
        }

        public Texture CustomTexture { get; set; }

        public TLoadTextureNode() : base()
        {
            filePath = new TStringParameter();
        }

        public override TSlot GetMainOutputSlot()
        {
            return outputSlot;
        }

        public override void Execute(TGraphContext context)
        {
            RenderTexture targetRT = context.RequestTargetRT(TSlotReference.Create(GUID, outputSlot.Id), GetRenderTextureRequest(outputSlot));
            Texture tex = null;
            if (CustomTexture != null)
            {
                tex = CustomTexture;
            }
            else
            {
                if (string.IsNullOrEmpty(filePath.value))
                {
                    tex = Texture2D.blackTexture;
                }
                else
                {
                    if (filePath.value.StartsWith(TConst.RESOURCES_FOLDER))
                    {
                        tex = Resources.Load<Texture>(filePath.value.Substring(TConst.RESOURCES_FOLDER.Length));
                    }
                    else
                    {
#if UNITY_EDITOR
                        tex = AssetDatabase.LoadAssetAtPath<Texture>(filePath.value);
#endif
                    }
                }
            }

            if (tex == null)
            {
                tex = Texture2D.blackTexture;
            }
            TDrawing.CopyToRT(tex, targetRT);
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
