using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public class TGraphSettingsWindow : EditorWindow
    {
        public TGraphEditor GraphEditor { get; set; }
        private Vector2Int BaseResolution { get; set; }
        private bool UseHighPrecision { get; set; }

        private static readonly GUIContent baseResolutionGUI = new GUIContent("Base Resolution", "Default resolution for all nodes");
        private static readonly GUIContent useHighPrecisionGUI = new GUIContent("High Precision", "Presicion for texture data");

        private static readonly GUIContent lightColor0GUI = new GUIContent("Light Color 0", "Color of the first light");
        private static readonly GUIContent lightIntensity0GUI = new GUIContent("Light Intensity 0", "Intensity of the first light");
        private static readonly GUIContent lightColor1GUI = new GUIContent("Light Color 1", "Color of the second light");
        private static readonly GUIContent lightIntensity1GUI = new GUIContent("Light Intensity 1", "Intensity of the second light");
        private static readonly GUIContent customMeshGUI = new GUIContent("Custom Mesh", "User-selected mesh to render preview");
        private static readonly GUIContent tessLevelGUI = new GUIContent("Tessellation Level", "Add more detail to the preview mesh");
        private static readonly GUIContent displacementStrengthGUI = new GUIContent("Displacement Strength", "How much to displace the mesh based on the height map");

        private Vector2 scrollPos;

        public static TGraphSettingsWindow ShowWindow(TGraphEditor graphEditor)
        {
            TGraphSettingsWindow window = TGraphSettingsWindow.CreateInstance<TGraphSettingsWindow>();
            window.titleContent = new GUIContent($"{graphEditor.ClonedGraph.name} Settings");
            window.GraphEditor = graphEditor;
            window.LoadSettings();
            window.ShowUtility();
            return window;
        }

        public void LoadSettings()
        {
            BaseResolution = GraphEditor.ClonedGraph.BaseResolution;
            UseHighPrecision = GraphEditor.ClonedGraph.UseHighPrecision;
        }

        private void OnGUI()
        {
            EditorGUI.indentLevel += 1;
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            DrawGraphSettings();
            Draw3DEnvironmentSettings();
            EditorGUILayout.EndScrollView();

            TEditorCommon.Separator();
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("OK", GUILayout.Width(100)))
            {
                ApplySettings();
                Close();
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(100)))
            {
                Close();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.indentLevel -= 1;
        }

        private void DrawGraphSettings()
        {
            TEditorCommon.Header("Graph");
            BaseResolution = TEditorCommon.InlineVector2IntField(baseResolutionGUI, BaseResolution);
            BaseResolution = new Vector2Int(
                Mathf.Clamp(BaseResolution.x, TConst.TEXTURE_SIZE_MIN.x, TConst.TEXTURE_SIZE_MAX.x),
                Mathf.Clamp(BaseResolution.y, TConst.TEXTURE_SIZE_MIN.y, TConst.TEXTURE_SIZE_MAX.y));
            UseHighPrecision = EditorGUILayout.Toggle(useHighPrecisionGUI, UseHighPrecision);
        }

        private void ApplySettings()
        {
            GraphEditor.ClonedGraph.BaseResolution = BaseResolution;
            GraphEditor.ClonedGraph.UseHighPrecision = UseHighPrecision;
            GraphEditor.ClonedGraph.Execute();
        }

        private void Draw3DEnvironmentSettings()
        {
            TEditorCommon.Header("3D Environment");
            TView3DEnvironmentSettings envSettings = GraphEditor.ClonedGraph.View3DEnvironmentSettings;
            EditorGUI.BeginChangeCheck();
            envSettings.Light0.LightColor = EditorGUILayout.ColorField(lightColor0GUI, envSettings.Light0.LightColor, true, false, true);
            envSettings.Light0.Intensity = EditorGUILayout.FloatField(lightIntensity0GUI, envSettings.Light0.Intensity);
            envSettings.Light1.LightColor = EditorGUILayout.ColorField(lightColor1GUI, envSettings.Light1.LightColor, true, false, true);
            envSettings.Light1.Intensity = EditorGUILayout.FloatField(lightIntensity1GUI, envSettings.Light1.Intensity);
            envSettings.CustomMesh = EditorGUILayout.ObjectField(customMeshGUI, envSettings.CustomMesh, typeof(Mesh), false) as Mesh;
            envSettings.TessellationLevel = EditorGUILayout.IntSlider(tessLevelGUI, envSettings.TessellationLevel, 1, 64);
            envSettings.DisplacementStrength = EditorGUILayout.Slider(displacementStrengthGUI, envSettings.DisplacementStrength, 0f, 1f);
            if (EditorGUI.EndChangeCheck())
            {
                if (TViewManager.IsViewVisible(GraphEditor.view3DWindow.viewDataKey))
                {
                    GraphEditor.view3DWindow.RenderPreviewScene();
                }
            }
        }
    }
}
