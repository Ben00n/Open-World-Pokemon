using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
using System.IO;

namespace Pinwheel.TextureGraph
{
    public class TExportWindow : EditorWindow
    {
        private TGraphEditor GraphEditor { get; set; }
        private string ExportDirectory { get; set; }
        private string NameTemplate { get; set; }

        private const string DIRECTORY_KEY = "texture-graph-export-directory";
        private const string NAME_TEMPLATE_KEY = "texture-graph-export-name-template";
        private const string EXTENSION_KEY = "texture-graph-export-ext";
        private const string SELECTED_KEY = "texture-graph-export-selected";

        public const string WC_ID = "<id>";
        public const string WC_GUID = "<guid>";
        public const string WC_WIDTH = "<width>";
        public const string WC_HEIGHT = "<height>";
        public const string WC_USAGE = "<usage>";
        public const string WC_TIME = "<time>";
        public const string WC_TICK = "<tick>";

        private const int NO_ERROR = 0;
        private const int ERROR_DIRECTORY_NOT_EXIST = 1;
        private const int ERROR_NAME_EMPTY = 2;
        private const int ERROR_NAME_NO_START_WITH_LETTER = 3;
        private const int ERROR_NAME_NO_WILDCARD = 4;

        private static readonly string[] ERROR_MESSAGES = new string[]
        {
            "No error",
            "The selected directory is not exist.",
            "Name Template cannot be empty.",
            "Name Template must begin with a letter.",
            "Name Template must has at least one valid wildcard."
        };

        private const int SELECTION_TOGGLE_WIDTH = 30;

        private static readonly GUIContent idGUI = new GUIContent("Id", "User define id for each output node. You can change this value by selecting the output node and use the Parameters panel. This value should not be duplicated.");
        private static readonly GUIContent extGUI = new GUIContent("Extension", "File extension for exported image.");

        private Vector2 scrollPos;

        public static TExportWindow ShowWindow(TGraphEditor graphEditor)
        {
            TExportWindow window = CreateInstance<TExportWindow>();
            window.titleContent = new GUIContent("Export");
            window.GraphEditor = graphEditor;
            window.ShowUtility();
            return window;
        }

        public void OnEnable()
        {
            ExportDirectory = EditorPrefs.GetString(DIRECTORY_KEY, Application.dataPath);
            NameTemplate = EditorPrefs.GetString(NAME_TEMPLATE_KEY, "Output_<guid>");
        }

        public void OnDisable()
        {
            EditorPrefs.SetString(DIRECTORY_KEY, ExportDirectory);
            EditorPrefs.SetString(NAME_TEMPLATE_KEY, NameTemplate);
        }

        public void OnGUI()
        {
            EditorGUI.indentLevel += 1;

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawDirectoryAndNamingGUI();
            DrawOutputGUI();
            EditorGUILayout.EndScrollView();

            TEditorCommon.Separator();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Export All", GUILayout.Width(100)))
            {
                DoExportAll();
            }
            if (GUILayout.Button("Export Selected", GUILayout.Width(100)))
            {
                DoExportSelected();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
        }

        private void DrawDirectoryAndNamingGUI()
        {
            TEditorCommon.Header("Directory & Naming");
            EditorGUILayout.BeginHorizontal();
            ExportDirectory = EditorGUILayout.TextField("Directory", ExportDirectory);
            if (GUILayout.Button("Browse", GUILayout.Width(100)))
            {
                EditorGUI.FocusTextInControl("");
                ExportDirectory = EditorUtility.SaveFolderPanel("Export To...", ExportDirectory, "");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            NameTemplate = EditorGUILayout.TextField("Name Template", NameTemplate);
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Width(100));
            if (GUI.Button(r, "Wildcards"))
            {
                GenericMenu wildcardsMenu = new GenericMenu();
                wildcardsMenu.AddItem(new GUIContent("Id"), false, () => { AddWildcard(WC_ID); });
                wildcardsMenu.AddItem(new GUIContent("Guid"), false, () => { AddWildcard(WC_GUID); });
                wildcardsMenu.AddItem(new GUIContent("Width"), false, () => { AddWildcard(WC_WIDTH); });
                wildcardsMenu.AddItem(new GUIContent("Height"), false, () => { AddWildcard(WC_HEIGHT); });
                wildcardsMenu.AddItem(new GUIContent("Usage"), false, () => { AddWildcard(WC_USAGE); });
                wildcardsMenu.AddItem(new GUIContent("Time"), false, () => { AddWildcard(WC_TIME); });
                wildcardsMenu.AddItem(new GUIContent("Tick"), false, () => { AddWildcard(WC_TICK); });

                EditorGUI.FocusTextInControl("");
                wildcardsMenu.DropDown(r);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void AddWildcard(string t)
        {
            NameTemplate += t;
        }

        private void DrawOutputGUI()
        {
            TEditorCommon.Header("Outputs");
            List<TOutputNode> outputNodes = new List<TOutputNode>();
            foreach (TAbstractTextureNode n in GraphEditor.ClonedGraph.GraphData.Nodes)
            {
                if (n is TOutputNode output)
                {
                    outputNodes.Add(output);
                }
            }

            if (outputNodes.Count == 0)
            {
                EditorGUILayout.LabelField("You must have at least one Output node in your graph.", TEditorCommon.WordWrapItalicLabel);
            }

            foreach (TOutputNode n in outputNodes)
            {
                string id = n.OutputId.value;
                EditorGUILayout.BeginHorizontal();
                bool selected = GetSelectedState(n.GUID);
                EditorGUI.BeginChangeCheck();
                selected = EditorGUILayout.Toggle(selected, GUILayout.Width(SELECTION_TOGGLE_WIDTH));
                if (EditorGUI.EndChangeCheck())
                {
                    SetSelectedState(n.GUID, selected);
                }

                EditorGUILayout.BeginVertical();
                float labelWidth = EditorGUIUtility.labelWidth;
                EditorGUIUtility.labelWidth = labelWidth - SELECTION_TOGGLE_WIDTH;
                n.OutputId.value = EditorGUILayout.TextField(idGUI, n.OutputId.value);
                TImageExtension ext = GetExtension(n.GUID);
                EditorGUI.BeginChangeCheck();
                ext = (TImageExtension)EditorGUILayout.EnumPopup(extGUI, ext);
                if (EditorGUI.EndChangeCheck())
                {
                    SetExtension(n.GUID, ext);
                }
                EditorGUIUtility.labelWidth = labelWidth;
                EditorGUILayout.EndVertical();

                Rect previewRect = (EditorGUILayout.GetControlRect(GUILayout.Width(100), GUILayout.Height(100)));
                Texture preview = null;
                TSlot previewSlot = n.GetMainOutputSlot();
                if (previewSlot != null)
                {
                    preview = GraphEditor.ClonedGraph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
                }
                if (preview == null)
                {
                    EditorGUI.DrawRect(previewRect, Color.black);
                    EditorGUI.LabelField(previewRect, "No preview available.", TEditorCommon.CenteredWhiteLabel);
                }
                else
                {
                    if (TUtilities.IsGrayscaleFormat(preview.graphicsFormat))
                    {
                        Material mat = TEditorCommon.PreviewRedToGrayMaterial;
                        EditorGUI.DrawPreviewTexture(previewRect, preview, mat, ScaleMode.ScaleToFit, 1);
                    }
                    else
                    {
                        EditorGUI.DrawTextureTransparent(previewRect, preview, ScaleMode.ScaleToFit);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        private TImageExtension GetExtension(Guid nodeGuid)
        {
            string key = EXTENSION_KEY + nodeGuid.ToString();
            int value = EditorPrefs.GetInt(key, 0);
            return (TImageExtension)value;
        }

        private void SetExtension(Guid nodeGuid, TImageExtension ext)
        {
            string key = EXTENSION_KEY + nodeGuid.ToString();
            EditorPrefs.SetInt(key, (int)ext);
        }

        private bool GetSelectedState(Guid nodeGuid)
        {
            string key = SELECTED_KEY + nodeGuid.ToString();
            return EditorPrefs.GetBool(key, true);
        }

        private void SetSelectedState(Guid nodeGuid, bool value)
        {
            string key = SELECTED_KEY + nodeGuid.ToString();
            EditorPrefs.SetBool(key, value);
        }

        private int PreExportCheck()
        {
            if (!Directory.Exists(ExportDirectory))
            {
                return ERROR_DIRECTORY_NOT_EXIST;
            }
            if (string.IsNullOrEmpty(NameTemplate))
            {
                return ERROR_NAME_EMPTY;
            }
            else
            {
                if (!Char.IsLetter(NameTemplate[0]))
                {
                    return ERROR_NAME_NO_START_WITH_LETTER;
                }
            }

            if (!NameTemplate.Contains(WC_ID) &&
                !NameTemplate.Contains(WC_GUID) &&
                !NameTemplate.Contains(WC_WIDTH) &&
                !NameTemplate.Contains(WC_HEIGHT) &&
                !NameTemplate.Contains(WC_USAGE) &&
                !NameTemplate.Contains(WC_TIME) &&
                !NameTemplate.Contains(WC_TICK))
            {
                return ERROR_NAME_NO_WILDCARD;
            }

            return NO_ERROR;
        }

        private void DoExportAll()
        {
            int error = PreExportCheck();
            if (error != 0)
            {
                string msg = ERROR_MESSAGES[error];
                EditorUtility.DisplayDialog("Error", msg, "OK");
                return;
            }
            else
            {
                List<TOutputNode> outputNodes = new List<TOutputNode>();
                foreach (TAbstractTextureNode n in GraphEditor.ClonedGraph.GraphData.Nodes)
                {
                    if (n is TOutputNode output)
                    {
                        outputNodes.Add(output);
                    }
                }
                ExportNodes(outputNodes);
            }
        }

        private void DoExportSelected()
        {
            int error = PreExportCheck();
            if (error != 0)
            {
                string msg = ERROR_MESSAGES[error];
                EditorUtility.DisplayDialog("Error", msg, "OK");
                return;
            }
            else
            {
                List<TOutputNode> outputNodes = new List<TOutputNode>();
                foreach (TAbstractTextureNode n in GraphEditor.ClonedGraph.GraphData.Nodes)
                {
                    if (n is TOutputNode output)
                    {
                        if (GetSelectedState(n.GUID) == true)
                        {
                            outputNodes.Add(output);
                        }
                    }
                }

                ExportNodes(outputNodes);
            }
        }

        private void ExportNodes(List<TOutputNode> outputNodes)
        {
            TGraphContext context = TGraphContext.Create(GraphEditor.ClonedGraph);
            foreach (TOutputNode n in outputNodes)
            {
                DateTime now = DateTime.Now;
                Vector2Int resolution = n.GetOutputResolution(context);
                string fileNameNoExt = NameTemplate
                    .Replace(WC_GUID, n.GUID.ToString())
                    .Replace(WC_ID, n.OutputId.value)
                    .Replace(WC_WIDTH, resolution.x.ToString())
                    .Replace(WC_HEIGHT, resolution.y.ToString())
                    .Replace(WC_USAGE, n.Usage.value.ToString())
                    .Replace(WC_TIME, $"{now.Hour}-{now.Minute}-{now.Second}")
                    .Replace(WC_TICK, DateTime.Now.Ticks.ToString());
                string filePathNoExt = Path.Combine(ExportDirectory, fileNameNoExt);
                TImageExtension ext = GetExtension(n.GUID);
                n.SaveToImage(context, filePathNoExt, ext);
            }
            AssetDatabase.Refresh();
        }
    }
}
