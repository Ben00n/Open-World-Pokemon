using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TOutputNode))]
    public class TOutputNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent outputIdGUI = new GUIContent("Id", "An ID to be appended to exported file name, this should NOT be empty or duplicated");
        private static readonly GUIContent usageGUI = new GUIContent("Usage", "Usage hint for the texture, which will be used in exporting or 3D View");

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TOutputNode n = target as TOutputNode;
            n.OutputId = TParamGUI.TextField(outputIdGUI, n.OutputId);
            n.Usage.value = (TTextureUsage)EditorGUILayout.EnumPopup(usageGUI, n.Usage.value);

            if (GUILayout.Button("Save To Image"))
            {
                string path = EditorUtility.SaveFilePanel("Save", "", $"Output_{n.OutputId.value}", "png");
                if (string.IsNullOrEmpty(path))
                    return;
                n.SaveToImage(TGraphContext.Create(Graph), path, TImageExtension.PNG);
                AssetDatabase.Refresh();
            }
        }
    }
}
