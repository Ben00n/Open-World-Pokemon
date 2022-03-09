using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
using UnityEngine.UIElements;

namespace Pinwheel.TextureGraph
{
    public class TNodeView : Node
    {
        public TGraph Graph { get; set; }
        public Guid NodeDataGuid { get; set; }

        public void DrawPreviewCallback()
        {
            TAbstractTextureNode n = Graph.GraphData.GetNodeByGUID(NodeDataGuid);
            if (n == null)
                return;
            Texture preview = null;
            TSlot previewSlot = n.GetMainOutputSlot();
            if (previewSlot != null)
            {
                preview = Graph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
            }

            Rect r = new Rect(0, 0, TextureGraph.TConst.NODE_PREVIEW_SIZE.x, TextureGraph.TConst.NODE_PREVIEW_SIZE.y);
            if (preview == null)
            {
                EditorGUI.DrawRect(r, Color.black);
                EditorGUI.LabelField(r, "No preview available.", TEditorCommon.CenteredWhiteLabel);
            }
            else
            {
                if (TUtilities.IsGrayscaleFormat(preview.graphicsFormat))
                {
                    Material mat = TEditorCommon.PreviewRedToGrayMaterial;
                    EditorGUI.DrawPreviewTexture(r, preview, mat, ScaleMode.ScaleToFit, 1);
                }
                else
                {
                    GUI.DrawTexture(r, preview, ScaleMode.ScaleToFit, true, 1);
                }
            }
        }

        public void SetMetadata(string text)
        {
            Label metaLabel = this.Q<Label>("metaLabel");
            metaLabel.text = text;
        }
    }
}
