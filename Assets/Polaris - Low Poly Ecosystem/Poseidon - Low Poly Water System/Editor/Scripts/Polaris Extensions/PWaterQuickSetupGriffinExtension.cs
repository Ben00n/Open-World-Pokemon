#if GRIFFIN && UNITY_EDITOR && POSEIDON
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Pinwheel.Poseidon;
using Pinwheel.Griffin;

namespace Pinwheel.Poseidon.GriffinExtension
{
    public static class PWaterQuickSetupGriffinExtension
    {
        private const string GX_POSEIDON_WATER_QUICK_SETUP = "http://bit.ly/2qb2gLO";

        public static string GetExtensionName()
        {
            return "Poseidon - Water Quick Setup";
        }

        public static string GetPublisherName()
        {
            return "Pinwheel Studio";
        }

        public static string GetDescription()
        {
            return
                "Quickly add low poly water which fit into your scene mood.";
        }

        public static string GetVersion()
        {
            return "v1.0.0";
        }

        public static void OpenSupportLink()
        {
            PEditorCommon.OpenEmailEditor(
                "customer@pinwheel.studio",
                "Griffin Extension - Poseidon",
                "YOUR_MESSAGE_HERE");
        }

        public static void OnGUI()
        {
            PWater water = PGriffinExtensionCommon.FindTargetWaterObject();
            if (water == null)
            {
                DrawCreateWaterGUI();
            }
            else
            {
                DrawWaterConfigGUI(water);
            }
        }

        private static void DrawCreateWaterGUI()
        {
            if (GUILayout.Button("Add Low Poly Water"))
            {
                GAnalytics.Record(GX_POSEIDON_WATER_QUICK_SETUP);
                InitDefaultConfigs();
                AddWater();
            }
        }

        private static void InitDefaultConfigs()
        {
            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            config.WaterLevel = 1;
            config.MeshResolution = 100;
            config.MeshNoise = 0;

            Bounds levelBounds = GCommon.GetLevelBounds();
            config.ApproximateSizeX = levelBounds.size.x;
            config.ApproximateSizeZ = levelBounds.size.z;
            config.TileSize = Vector2.one * 100;
        }

        private static void AddWater()
        {
            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            PWater water = PGriffinExtensionCommon.CreateTargetWaterObject();
            water.TileSize = config.TileSize;
            water.MeshResolution = config.MeshResolution;
            water.MeshNoise = config.MeshNoise;
            water.GeneratePlaneMesh();

            CenterToLevel(water);
            MatchWaterLevel(water);
            MatchWaterSize(water);

            Selection.activeGameObject = water.gameObject;
        }

        private static void CenterToLevel(PWater water)
        {
            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            Bounds levelBounds = GCommon.GetLevelBounds();
            water.transform.position = new Vector3(levelBounds.center.x, config.WaterLevel, levelBounds.center.z);
        }

        private static void MatchWaterLevel(PWater water)
        {
            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            water.transform.position = new Vector3(water.transform.position.x, config.WaterLevel, water.transform.position.z);
        }

        private static void MatchWaterSize(PWater water)
        {
            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            water.TileSize = config.TileSize;
            int radiusX = 1 + Mathf.FloorToInt(config.ApproximateSizeX * 0.5f / water.TileSize.x);
            int radiusZ = 1 + Mathf.FloorToInt(config.ApproximateSizeZ * 0.5f / water.TileSize.y);

            water.TileIndices.Clear();
            for (int x = -radiusX; x < radiusX; ++x)
            {
                for (int z = -radiusZ; z < radiusZ; ++z)
                {
                    water.TileIndices.AddIfNotContains(new PIndex2D(x, z));
                }
            }
        }

        private static void DrawWaterConfigGUI(PWater water)
        {
            DrawTemplateSelectionGUI(water);

            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            bool changed = false;
            bool meshChanged = false;
            EditorGUI.BeginChangeCheck();
            config.WaterLevel = EditorGUILayout.FloatField("Water Level", config.WaterLevel);
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            EditorGUI.BeginChangeCheck();
            config.MeshResolution = EditorGUILayout.DelayedIntField("Resolution", config.MeshResolution);
            if (EditorGUI.EndChangeCheck())
            {
                meshChanged = true;
            }

            EditorGUI.BeginChangeCheck();
            config.MeshNoise = EditorGUILayout.FloatField("Noise", config.MeshNoise);
            config.ApproximateSizeX = EditorGUILayout.FloatField("Approx. Size X", config.ApproximateSizeX);
            config.ApproximateSizeZ = EditorGUILayout.FloatField("Approx. Size Z", config.ApproximateSizeZ);
            config.TileSize = PEditorCommon.InlineVector2Field("Tile Size", config.TileSize);
            if (EditorGUI.EndChangeCheck())
            {
                changed = true;
            }

            if (meshChanged)
            {
                water.GeneratePlaneMesh();
            }

            if (changed || meshChanged)
            {
                water.MeshNoise = config.MeshNoise;
                water.TileSize = config.TileSize;
                water.UpdateMaterial();
                MatchWaterLevel(water);
                MatchWaterSize(water);
            }

            EditorUtility.SetDirty(config);

            GEditorCommon.Separator();
            if (GUILayout.Button("Center To Level Bounds"))
            {
                CenterToLevel(water);
            }
            if (GUILayout.Button("Fill Level Bounds"))
            {
                Bounds levelBounds = GCommon.GetLevelBounds();
                config.ApproximateSizeX = levelBounds.size.x;
                config.ApproximateSizeZ = levelBounds.size.z;
                CenterToLevel(water);
                MatchWaterSize(water);
            }
            if (GUILayout.Button("Done"))
            {
                Done(water);
            }
        }

        private const string WATER_TEMPLATE_RESOURCES_PATH = "WaterTemplates";
        private static Rect templatePopupRect;
        private static void DrawTemplateSelectionGUI(PWater water)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Template");
            Rect r = EditorGUILayout.GetControlRect();
            if (Event.current.type == EventType.Repaint)
            {
                templatePopupRect = r;
            }

            if (GUI.Button(r, "Select", EditorStyles.popup))
            {
                GenericMenu menu = new GenericMenu();

                PWaterProfile[] templates = Resources.LoadAll<PWaterProfile>(WATER_TEMPLATE_RESOURCES_PATH);
                if (templates.Length == 0)
                {
                    menu.AddDisabledItem(new GUIContent("No Template found!"));
                }
                else
                {
                    for (int i = 0; i < templates.Length; ++i)
                    {
                        int index = i;
                        string label = templates[i].name.Replace("_", "/");
                        menu.AddItem(
                            new GUIContent(label),
                            false,
                            () =>
                            {
                                ApplyTemplate(water, templates[index]);
                            });
                    }
                }
                menu.DropDown(templatePopupRect);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void ApplyTemplate(PWater water, PWaterProfile template)
        {
            water.Profile.CopyFrom(template);

            PWaterQuickSetupConfig config = PWaterQuickSetupConfig.Instance;
            water.MeshResolution = config.MeshResolution;
            water.MeshNoise = config.MeshNoise;
            water.GeneratePlaneMesh();
            water.UpdateMaterial();
            water.TileSize = config.TileSize;
            MatchWaterLevel(water);
            MatchWaterSize(water);
        }

        private static void Done(PWater water)
        {
            water.name = water.name.Replace("~", "");
            Selection.activeGameObject = water.gameObject;
            EditorGUIUtility.PingObject(water.gameObject);
        }
    }
}
#endif
