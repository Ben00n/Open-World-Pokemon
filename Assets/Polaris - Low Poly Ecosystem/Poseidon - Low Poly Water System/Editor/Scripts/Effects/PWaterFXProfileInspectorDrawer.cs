using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Poseidon.FX
{
    public class PWaterFXProfileInspectorDrawer
    {
        private PWaterFXProfile instance;
        private PWater water;

        private PWaterFXProfileInspectorDrawer(PWaterFXProfile instance, PWater water)
        {
            this.instance = instance;
            this.water = water;
        }

        public static PWaterFXProfileInspectorDrawer Create(PWaterFXProfile instance, PWater water)
        {
            return new PWaterFXProfileInspectorDrawer(instance, water);
        }

        public void DrawGUI()
        {
            EditorGUI.BeginChangeCheck();
            DrawUnderwaterGUI();
            DrawWetLensGUI();
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(instance);
            }
        }

        private void DrawUnderwaterGUI()
        {
            string label = "Underwater";
            string id = "underwater" + instance.GetInstanceID();

            PEditorCommon.Foldout(label, false, id, () =>
            {
                instance.EnableUnderwater = EditorGUILayout.Toggle("Enable", instance.EnableUnderwater);
                if (instance.EnableUnderwater)
                {
                    PEditorCommon.SpacePixel(0);
                    EditorGUILayout.LabelField("Water Body", PEditorCommon.BoldHeader);
                    instance.UnderwaterMaxDepth = EditorGUILayout.FloatField("Max Depth", instance.UnderwaterMaxDepth);
                    instance.UnderwaterSurfaceColorBoost = EditorGUILayout.Slider("Surface Color Boost", instance.UnderwaterSurfaceColorBoost, 1f, 3f);

                    PEditorCommon.SpacePixel(0);
                    EditorGUILayout.LabelField("Fog", PEditorCommon.BoldHeader);
                    instance.UnderwaterShallowFogColor = EditorGUILayout.ColorField("Shallow Fog Color", instance.UnderwaterShallowFogColor);
                    instance.UnderwaterDeepFogColor = EditorGUILayout.ColorField("Deep Fog Color", instance.UnderwaterDeepFogColor);
                    instance.UnderwaterViewDistance = EditorGUILayout.FloatField("View Distance", instance.UnderwaterViewDistance);

                    PEditorCommon.SpacePixel(0);
                    EditorGUILayout.LabelField("Caustic", PEditorCommon.BoldHeader);
                    instance.UnderwaterEnableCaustic = EditorGUILayout.Toggle("Enable", instance.UnderwaterEnableCaustic);
                    if (instance.UnderwaterEnableCaustic)
                    {
                        instance.UnderwaterCausticTexture = PEditorCommon.InlineTextureField("Texture", instance.UnderwaterCausticTexture, -1);
                        instance.UnderwaterCausticSize = EditorGUILayout.FloatField("Size", instance.UnderwaterCausticSize);
                        instance.UnderwaterCausticStrength = EditorGUILayout.Slider("Strength", instance.UnderwaterCausticStrength, 0f, 1f);
                    }

                    PEditorCommon.SpacePixel(0);
                    EditorGUILayout.LabelField("Distortion", PEditorCommon.BoldHeader);
                    instance.UnderwaterEnableDistortion = EditorGUILayout.Toggle("Enable", instance.UnderwaterEnableDistortion);
                    if (instance.UnderwaterEnableDistortion)
                    {
                        instance.UnderwaterDistortionTexture = PEditorCommon.InlineTextureField("Normal Map", instance.UnderwaterDistortionTexture, -1);
                        instance.UnderwaterDistortionStrength = EditorGUILayout.FloatField("Strength", instance.UnderwaterDistortionStrength);
                        instance.UnderwaterWaterFlowSpeed = EditorGUILayout.FloatField("Water Flow Speed", instance.UnderwaterWaterFlowSpeed);
                    }
                    PEditorCommon.Separator();
                }

                if (water != null && instance.EnableUnderwater)
                {
                    GUI.enabled = water.Profile != null;
                    if (GUILayout.Button("Inherit Parameters"))
                    {
                        instance.UnderwaterMaxDepth = water.Profile.MaxDepth;
                        instance.UnderwaterShallowFogColor = water.Profile.Color;
                        instance.UnderwaterDeepFogColor = water.Profile.DepthColor;
                        instance.UnderwaterEnableCaustic = water.Profile.EnableCaustic;
                        instance.UnderwaterCausticTexture = water.Profile.CausticTexture;
                        instance.UnderwaterCausticSize = water.Profile.CausticSize;
                        instance.UnderwaterCausticStrength = water.Profile.CausticStrength;
                    }
                    GUI.enabled = true;
                }
            });
        }

        private void DrawWetLensGUI()
        {
            string label = "Wet Lens";
            string id = "wet-lens" + instance.GetInstanceID();

            PEditorCommon.Foldout(label, false, id, () =>
            {
                instance.EnableWetLens = EditorGUILayout.Toggle("Enable", instance.EnableWetLens);
                if (instance.EnableWetLens)
                {
                    instance.WetLensNormalMap = PEditorCommon.InlineTextureField("Normal Map", instance.WetLensNormalMap, -1);
                    instance.WetLensStrength = EditorGUILayout.Slider("Strength", instance.WetLensStrength, 0f, 1f);
                    instance.WetLensDuration = EditorGUILayout.FloatField("Duration", instance.WetLensDuration);
                    instance.WetLensFadeCurve = EditorGUILayout.CurveField("Fade", instance.WetLensFadeCurve, Color.red, new Rect(0, 0, 1, 1));
                }
            });
        }
    }
}
