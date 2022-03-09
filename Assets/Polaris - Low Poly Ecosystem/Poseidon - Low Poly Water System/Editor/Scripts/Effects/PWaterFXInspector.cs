using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;
#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine.Rendering.PostProcessing;
#endif
#if POSEIDON_URP
using UnityEngine.Rendering.Universal;
using Pinwheel.Poseidon.FX.Universal;
#endif
using System.Reflection;

namespace Pinwheel.Poseidon.FX
{
    [CustomEditor(typeof(PWaterFX))]
    public class PWaterFXInspector : Editor
    {
        private PWaterFX instance;
        private void OnEnable()
        {
            instance = target as PWaterFX;
            SceneView.duringSceneGui += DuringSceneGUI;

            if (instance.Profile != null)
            {
                instance.UpdatePostProcessOrVolumeProfile();
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            bool willDrawGUI = false;
            string msg = null;
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
#if UNITY_POST_PROCESSING_STACK_V2
                willDrawGUI = true;
#else
                msg = "Please install Post Processing Stack V2 to setup water effect";
#endif
            }
            else if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
#if POSEIDON_URP
                willDrawGUI = true;
#endif
            }
            else
            {
                willDrawGUI = false;
                msg = "Water effect doesn't support custom render pipeline.";
            }

            if (!willDrawGUI)
            {
                EditorGUILayout.LabelField(msg, PEditorCommon.WordWrapItalicLabel);
                return;
            }

#if POSEIDON_URP
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                if (!CheckRendererFeatureConfig())
                {
                    EditorGUILayout.LabelField("Please add a Water Effect Renderer Feature to the current URP Asset to setup water effect.", PEditorCommon.WordWrapItalicLabel);
                    return;
                }
            }
#endif

            instance.Water = EditorGUILayout.ObjectField("Water", instance.Water, typeof(PWater), true) as PWater;
            instance.Profile = PEditorCommon.ScriptableObjectField<PWaterFXProfile>("Profile", instance.Profile);

            if (instance.Water == null || instance.Profile == null)
                return;
            DrawVolumesGUI();
            EditorGUI.BeginChangeCheck();
            PWaterFXProfileInspectorDrawer.Create(instance.Profile, instance.Water).DrawGUI();
            if (EditorGUI.EndChangeCheck())
            {
                instance.UpdatePostProcessOrVolumeProfile();
            }
            DrawEventsGUI();
        }

#if POSEIDON_URP
        private bool CheckRendererFeatureConfig()
        {
            UniversalRenderPipelineAsset uAsset = UniversalRenderPipeline.asset;
            ScriptableRenderer renderer = uAsset.scriptableRenderer;
            PropertyInfo rendererFeaturesProperty = renderer.GetType().GetProperty("rendererFeatures", BindingFlags.Instance | BindingFlags.NonPublic);
            if (rendererFeaturesProperty != null)
            {
                List<ScriptableRendererFeature> rendererFeatures = rendererFeaturesProperty.GetValue(renderer) as List<ScriptableRendererFeature>;
                if (rendererFeatures != null)
                {
                    bool waterFxFeatureAdded = false;
                    for (int i=0;i<rendererFeatures.Count;++i)
                    {
                        if (rendererFeatures[i] is PWaterEffectRendererFeature)
                        {
                            waterFxFeatureAdded = true;
                        }
                    }
                    return waterFxFeatureAdded;
                }
            }
            return true;
        }
#endif

        private void DrawVolumesGUI()
        {
            string label = "Volumes";
            string id = "volumes" + instance.GetInstanceID();

            PEditorCommon.Foldout(label, false, id, () =>
            {
                instance.VolumeExtent = PEditorCommon.InlineVector3Field("Extent", instance.VolumeExtent);
                instance.VolumeLayer = EditorGUILayout.LayerField("Layer", instance.VolumeLayer);
            });
        }

        private void DrawEventsGUI()
        {
            string label = "Events";
            string id = "events" + instance.GetInstanceID();

            PEditorCommon.Foldout(label, false, id, () =>
            {
                SerializedObject so = new SerializedObject(instance);
                SerializedProperty sp0 = so.FindProperty("onEnterWater");
                if (sp0 != null)
                {
                    EditorGUILayout.PropertyField(sp0);
                }

                SerializedProperty sp1 = so.FindProperty("onExitWater");
                if (sp1 != null)
                {
                    EditorGUILayout.PropertyField(sp1);
                }
                so.ApplyModifiedProperties();
            });
        }

        private void DuringSceneGUI(SceneView sv)
        {
            if (instance.Water == null || instance.Profile == null)
                return;

            Bounds waterBounds = instance.Water.Bounds;
            Vector3 size = waterBounds.size + instance.VolumeExtent;

            Color color = Handles.yAxisColor;
            Matrix4x4 matrix = Matrix4x4.TRS(waterBounds.center + instance.transform.position, instance.transform.rotation, instance.transform.localScale);
            using (var scope = new Handles.DrawingScope(color, matrix))
            {
                Handles.DrawWireCube(Vector3.zero, size);
            }
        }
    }
}
