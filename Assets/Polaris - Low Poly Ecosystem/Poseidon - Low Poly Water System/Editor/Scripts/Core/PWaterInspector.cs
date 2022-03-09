using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using Pinwheel.Poseidon.FX;
using UnityEngine.Rendering;
#if TEXTURE_GRAPH
using Pinwheel.TextureGraph;
#endif

namespace Pinwheel.Poseidon
{
    [CustomEditor(typeof(PWater))]
    public class PWaterInspector : Editor
    {
        private PWater water;
        private PWaterProfile profile;
        private bool willDrawDebugGUI = false;

        private SerializedObject so;
        private SerializedProperty reflectionLayersSO;
        private SerializedProperty refractionLayersSO;

        private readonly int[] renderTextureSizes = new int[] { 128, 256, 512, 1024, 2048 };
        private readonly string[] renderTextureSizeLabels = new string[] { "128", "256", "512", "1024", "2048*" };

        private readonly int[] meshTypes = new int[]
        {
            (int)PWaterMeshType.TileablePlane,
            (int)PWaterMeshType.Area,
            (int)PWaterMeshType.Spline,
            (int)PWaterMeshType.CustomMesh
        };
        private readonly string[] meshTypeLabels = new string[]
        {
            "Tilealbe Plane",
            "Area (Experimental)",
            "Spline (Experimental)",
            "Custom (Experimental)"
        };

        private bool isEditingTileIndices = false;
        private bool isEditingAreaMesh = false;
        private bool isEditingSplineMesh = false;

        private PTilesEditingGUIDrawer tileEditingGUIDrawer;
        private PAreaEditingGUIDrawer areaEditingGUIDrawer;
        private PSplineEditingGUIDrawer splineEditingGUIDrawer;

        private static Mesh quadMesh;
        private static Mesh QuadMesh
        {
            get
            {
                if (quadMesh == null)
                {
                    quadMesh = Resources.GetBuiltinResource<Mesh>("Quad.fbx");
                }
                return quadMesh;
            }
        }

        private static Material maskVisualizerMaterial;
        private static Material MaskVisualizerMaterial
        {
            get
            {
                if (maskVisualizerMaterial == null)
                {
                    maskVisualizerMaterial = new Material(Shader.Find("Hidden/Poseidon/WaveMaskVisualizer"));
                }
                return maskVisualizerMaterial;
            }
        }

        private enum PWaveMaskVisualizationMode
        {
            None,
            //Flow, 
            Crest,
            Height
        }

        private PWaveMaskVisualizationMode waveMaskVisMode;

        private void OnEnable()
        {
            LoadPrefs();
            water = target as PWater;
            if (water.Profile != null)
            {
                water.ReCalculateBounds();
            }

            tileEditingGUIDrawer = new PTilesEditingGUIDrawer(water);
            areaEditingGUIDrawer = new PAreaEditingGUIDrawer(water);
            splineEditingGUIDrawer = new PSplineEditingGUIDrawer(water);

            SceneView.duringSceneGui += DuringSceneGUI;
            Camera.onPreCull += OnRenderCamera;
            RenderPipelineManager.beginCameraRendering += OnRenderCameraSRP;
        }

        private void OnDisable()
        {
            SavePrefs();
            isEditingTileIndices = false;
            if (isEditingAreaMesh)
            {
                isEditingAreaMesh = false;
                water.GenerateAreaMesh();
            }
            if (isEditingSplineMesh)
            {
                isEditingSplineMesh = false;
                water.GenerateSplineMesh();
            }

            SceneView.duringSceneGui -= DuringSceneGUI;
            Camera.onPreCull -= OnRenderCamera;
            RenderPipelineManager.beginCameraRendering -= OnRenderCameraSRP;
        }

        private void LoadPrefs()
        {
            waveMaskVisMode = (PWaveMaskVisualizationMode)SessionState.GetInt("poseidon-wave-mask-vis-mode", 0);
        }

        private void SavePrefs()
        {
            SessionState.SetInt("poseidon-wave-mask-vis-mode", (int)waveMaskVisMode);
        }

        public override void OnInspectorGUI()
        {
            if (water.transform.rotation != Quaternion.identity)
            {
                string warning = "The water object is designed to work without rotation. Some features may not work correctly.";
                EditorGUILayout.LabelField(warning, PEditorCommon.WarningLabel);
            }

            water.Profile = PEditorCommon.ScriptableObjectField<PWaterProfile>("Profile", water.Profile);
            profile = water.Profile;
            if (water.Profile == null)
                return;
            so = new SerializedObject(profile);
            reflectionLayersSO = so.FindProperty("reflectionLayers");
            refractionLayersSO = so.FindProperty("refractionLayers");

            EditorGUI.BeginChangeCheck();
            DrawMeshSettingsGUI();
            DrawRenderingSettingsGUI();
            DrawTimeSettingsGUI();
            DrawColorsSettingsGUI();
            DrawFresnelSettingsGUI();
            DrawRippleSettingsGUI();
            DrawWaveSettingsGUI();
            DrawLightAbsorbtionSettingsGUI();
            DrawFoamSettingsGUI();
            DrawReflectionSettingsGUI();
            DrawRefractionSettingsGUI();
            DrawCausticSettingsGUI();
#if AURA_IN_PROJECT
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                DrawAuraIntegrationSettingsGUI();
            }
#endif
            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(water);
                EditorUtility.SetDirty(profile);
                water.UpdateMaterial();
            }

            DrawEffectsGUI();
            if (willDrawDebugGUI)
                DrawDebugGUI();

            if (so != null)
            {
                so.Dispose();
            }

            if (reflectionLayersSO != null)
            {
                reflectionLayersSO.Dispose();
            }

            if (refractionLayersSO != null)
            {
                refractionLayersSO.Dispose();
            }
        }

        private void DrawEffectsGUI()
        {
            PWaterFX fx = water.GetComponent<PWaterFX>();
            if (fx != null)
                return;

            string label = "Effects";
            string id = "effects" + water.GetInstanceID();

            PEditorCommon.Foldout(label, false, id, () =>
            {
                GUI.enabled = true;
                if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
                {
                    bool isStackV2Installed = false;
#if UNITY_POST_PROCESSING_STACK_V2
                isStackV2Installed = true;
#endif
                    if (!isStackV2Installed)
                    {
                        EditorGUILayout.LabelField("Water effect need the Post Processing Stack V2 to work. Please install it using the Package Manager", PEditorCommon.WordWrapItalicLabel);
                    }
                    GUI.enabled = isStackV2Installed;
                }
                if (GUILayout.Button("Add Effects"))
                {
                    fx = water.gameObject.AddComponent<PWaterFX>();
                    fx.Water = water;
                }
                GUI.enabled = true;
            });
        }

        private void DrawMeshSettingsGUI()
        {
            string label = "Mesh";
            string id = "water-profile-mesh";
            GenericMenu menu = new GenericMenu();
            menu.AddItem(
                new GUIContent("Generate"),
                false,
                () => { water.GenerateMesh(); });

            PEditorCommon.Foldout(label, true, id, () =>
            {
                if (water.MeshType == PWaterMeshType.TileablePlane)
                {
                    DrawTilableMeshGUI();
                }
                else if (water.MeshType == PWaterMeshType.Area)
                {
                    DrawAreaMeshGUI();
                }
                else if (water.MeshType == PWaterMeshType.Spline)
                {
                    DrawSplineMeshGUI();
                }
                else if (water.MeshType == PWaterMeshType.CustomMesh)
                {
                    DrawCustomMeshGUI();
                }
            }, menu);
        }

        private void DrawTilableMeshGUI()
        {
            if (!isEditingTileIndices)
            {
                EditorGUI.BeginChangeCheck();
                //water.MeshType = (PWaterMeshType)EditorGUILayout.EnumPopup("Mesh Type", water.MeshType);
                water.MeshType = (PWaterMeshType)EditorGUILayout.IntPopup("Mesh Type", (int)water.MeshType, meshTypeLabels, meshTypes);
                water.PlanePattern = (PPlaneMeshPattern)EditorGUILayout.EnumPopup("Pattern", water.PlanePattern);
                water.MeshResolution = EditorGUILayout.DelayedIntField("Resolution", water.MeshResolution);
                if (EditorGUI.EndChangeCheck())
                {
                    water.GenerateMesh();
                    water.ReCalculateBounds();
                }
                water.MeshNoise = EditorGUILayout.FloatField("Noise", water.MeshNoise);

                EditorGUI.BeginChangeCheck();
                water.TileSize = PEditorCommon.InlineVector2Field("Tile Size", water.TileSize);
                water.TilesFollowMainCamera = EditorGUILayout.Toggle("Follow Main Camera", water.TilesFollowMainCamera);
                SerializedObject so = new SerializedObject(water);
                SerializedProperty sp = so.FindProperty("tileIndices");

                if (sp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sp, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        so.ApplyModifiedProperties();
                    }
                }

                sp.Dispose();
                so.Dispose();

                if (EditorGUI.EndChangeCheck())
                {
                    water.ReCalculateBounds();
                }
            }

            if (!isEditingTileIndices)
            {
                if (GUILayout.Button("Edit Tiles"))
                {
                    isEditingTileIndices = true;
                }
            }
            else
            {
                EditorGUILayout.LabelField("Edit water tiles in Scene View.", PEditorCommon.WordWrapItalicLabel);
                if (GUILayout.Button("End Editing Tiles"))
                {
                    isEditingTileIndices = false;
                }
            }
        }

        private void DrawAreaMeshGUI()
        {
            if (!isEditingAreaMesh)
            {
                EditorGUI.BeginChangeCheck();
                //water.MeshType = (PWaterMeshType)EditorGUILayout.EnumPopup("Mesh Type", water.MeshType);
                water.MeshType = (PWaterMeshType)EditorGUILayout.IntPopup("Mesh Type", (int)water.MeshType, meshTypeLabels, meshTypes);
                water.MeshResolution = EditorGUILayout.DelayedIntField("Resolution", water.MeshResolution);
                if (EditorGUI.EndChangeCheck())
                {
                    water.GenerateMesh();
                    water.ReCalculateBounds();
                }
                water.MeshNoise = EditorGUILayout.FloatField("Noise", water.MeshNoise);

                SerializedObject so = new SerializedObject(water);
                SerializedProperty sp = so.FindProperty("areaMeshAnchors");

                if (sp != null)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(sp, true);
                    if (EditorGUI.EndChangeCheck())
                    {
                        so.ApplyModifiedProperties();
                    }
                }

                sp.Dispose();
                so.Dispose();
            }

            if (!isEditingAreaMesh)
            {
                if (GUILayout.Button("Edit Area"))
                {
                    isEditingAreaMesh = true;
                }
            }
            else
            {
                if (GUILayout.Button("End Editing Area"))
                {
                    isEditingAreaMesh = false;
                    water.GenerateAreaMesh();
                    water.ReCalculateBounds();
                }
            }
        }

        private void DrawSplineMeshGUI()
        {
            if (!isEditingSplineMesh)
            {
                EditorGUI.BeginChangeCheck();
                //water.MeshType = (PWaterMeshType)EditorGUILayout.EnumPopup("Mesh Type", water.MeshType);
                water.MeshType = (PWaterMeshType)EditorGUILayout.IntPopup("Mesh Type", (int)water.MeshType, meshTypeLabels, meshTypes);
                water.SplineResolutionX = EditorGUILayout.DelayedIntField("Resolution X", water.SplineResolutionX);
                water.SplineResolutionY = EditorGUILayout.DelayedIntField("Resolution Y", water.SplineResolutionY);
                water.SplineWidth = EditorGUILayout.DelayedFloatField("Width", water.SplineWidth);
                if (EditorGUI.EndChangeCheck())
                {
                    water.GenerateMesh();
                    water.ReCalculateBounds();
                }
                //water.MeshNoise = EditorGUILayout.FloatField("Noise", water.MeshNoise);
            }

            if (!isEditingSplineMesh)
            {
                if (GUILayout.Button("Edit Spline"))
                {
                    isEditingSplineMesh = true;
                }
            }
            else
            {
                PSplineToolConfig.Instance.RaycastLayer = EditorGUILayout.LayerField("Raycast Layer", PSplineToolConfig.Instance.RaycastLayer);
                PSplineToolConfig.Instance.YOffset = EditorGUILayout.FloatField("Y Offset", PSplineToolConfig.Instance.YOffset);
                EditorGUI.BeginChangeCheck();
                PSplineToolConfig.Instance.AutoTangent = EditorGUILayout.Toggle("Auto Tangent", PSplineToolConfig.Instance.AutoTangent);
                if (EditorGUI.EndChangeCheck() && PSplineToolConfig.Instance.AutoTangent)
                {
                    water.Spline.SmoothTangents();
                    water.GenerateSplineMesh();
                    water.ReCalculateBounds();
                }
                EditorUtility.SetDirty(PSplineToolConfig.Instance);
                DrawSelectedAnchorGUI();
                DrawSelectedSegmentGUI();

                if (GUILayout.Button("Smooth Tangents"))
                {
                    water.Spline.SmoothTangents();
                    water.GenerateSplineMesh();
                    water.ReCalculateBounds();
                }
                if (GUILayout.Button("Pivot To Spline Center"))
                {
                    PSplineUtilities.WaterPivotToSplineCenter(water);
                    water.GenerateSplineMesh();
                    water.ReCalculateBounds();
                }
                if (GUILayout.Button("End Editing Spline"))
                {
                    isEditingSplineMesh = false;
                    water.GenerateSplineMesh();
                    water.ReCalculateBounds();
                }
            }
        }

        private void DrawSelectedAnchorGUI()
        {
            int anchorIndex = splineEditingGUIDrawer.selectedAnchorIndex;
            if (anchorIndex < 0 ||
                anchorIndex >= water.Spline.Anchors.Count)
                return;
            string label = "Selected Anchor";
            string id = "poseidon-selected-anchor";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                EditorGUI.indentLevel -= 1;
                PSplineAnchor a = water.Spline.Anchors[anchorIndex];
                EditorGUI.BeginChangeCheck();
                a.Position = PEditorCommon.InlineVector3Field("Position", a.Position);
                a.Rotation = Quaternion.Euler(PEditorCommon.InlineVector3Field("Rotation", a.Rotation.eulerAngles));
                a.Scale = PEditorCommon.InlineVector3Field("Scale", a.Scale);
                water.Spline.Anchors[anchorIndex] = a;
                if (EditorGUI.EndChangeCheck())
                {
                    if (PSplineToolConfig.Instance.AutoTangent)
                    {
                        int[] segmentIndices = water.Spline.SmoothTangents(anchorIndex);
                        water.GenerateSplineMeshAtSegments(segmentIndices);
                    }
                    else
                    {
                        List<int> segmentIndices = water.Spline.FindSegments(anchorIndex);
                        water.GenerateSplineMeshAtSegments(segmentIndices);
                    }
                }
                EditorGUI.indentLevel += 1;
            });
        }

        private void DrawSelectedSegmentGUI()
        {
            int segmentIndex = splineEditingGUIDrawer.selectedSegmentIndex;
            if (segmentIndex < 0 ||
                segmentIndex >= water.Spline.Anchors.Count)
                return;
            string label = "Selected Segment";
            string id = "poseidon-selected-segment";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                EditorGUI.indentLevel -= 1;
                EditorGUI.BeginChangeCheck();
                PSplineSegment s = water.Spline.Segments[segmentIndex];
                GUI.enabled = !PSplineToolConfig.Instance.AutoTangent;
                s.StartTangent = PEditorCommon.InlineVector3Field("Start Tangent", s.StartTangent);
                s.EndTangent = PEditorCommon.InlineVector3Field("End Tangent", s.EndTangent);
                GUI.enabled = true;
                s.ResolutionMultiplierY = EditorGUILayout.Slider("Resolution Multiplier Y", s.ResolutionMultiplierY, 0f, 2f);
                water.Spline.Segments[segmentIndex] = s;
                if (EditorGUI.EndChangeCheck())
                {
                    water.GenerateSplineMeshAtSegment(segmentIndex);
                }
                EditorGUI.indentLevel += 1;
            });
        }

        private void DrawCustomMeshGUI()
        {
            EditorGUI.BeginChangeCheck();
            //water.MeshType = (PWaterMeshType)EditorGUILayout.EnumPopup("Mesh Type", water.MeshType);
            water.MeshType = (PWaterMeshType)EditorGUILayout.IntPopup("Mesh Type", (int)water.MeshType, meshTypeLabels, meshTypes);
            water.SourceMesh = EditorGUILayout.ObjectField("Source Mesh", water.SourceMesh, typeof(Mesh), false) as Mesh;
            if (EditorGUI.EndChangeCheck())
            {
                if (water.SourceMesh != null)
                {
                    water.GenerateMesh();
                }
                water.ReCalculateBounds();
            }
            water.MeshNoise = EditorGUILayout.FloatField("Noise", water.MeshNoise);
        }

        private void DrawRenderingSettingsGUI()
        {
            string label = "Rendering";
            string id = "water-profile-general";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                GUI.enabled = false;
                EditorGUILayout.ObjectField("Material", water.MaterialToRender, typeof(Material), false);
                if (water.MeshType == PWaterMeshType.TileablePlane && water.ShouldRenderBackface)
                {
                    EditorGUILayout.ObjectField("Material Back Face", water.MaterialBackFace, typeof(Material), false);
                }
                GUI.enabled = true;
                profile.RenderQueueIndex = EditorGUILayout.IntField("Queue Index", profile.RenderQueueIndex);

                profile.LightingModel = (PLightingModel)EditorGUILayout.EnumPopup("Light Model", profile.LightingModel);
                profile.UseFlatLighting = EditorGUILayout.Toggle("Flat Lighting", profile.UseFlatLighting);
                if (water.MeshType == PWaterMeshType.TileablePlane)
                {
                    water.ShouldRenderBackface = EditorGUILayout.Toggle("Render Back Face", water.ShouldRenderBackface);
                }

            });
        }

        private void DrawTimeSettingsGUI()
        {
            string label = "Time";
            string id = "water-time";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                water.TimeMode = (PTimeMode)EditorGUILayout.EnumPopup("Time Mode", water.TimeMode);
                if (water.TimeMode == PTimeMode.Auto)
                {
                    EditorGUILayout.LabelField("Time", water.GetTimeParam().ToString());
                }
                else
                {
                    water.ManualTimeSeconds = EditorGUILayout.FloatField("Time", (float)water.ManualTimeSeconds);
                }
            });
        }

        private void DrawColorsSettingsGUI()
        {
            string label = "Colors";
            string id = "water-profile-colors";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                profile.Color = EditorGUILayout.ColorField("Color", profile.Color);
                if (profile.EnableLightAbsorption)
                {
                    profile.DepthColor = EditorGUILayout.ColorField("Depth Color", profile.DepthColor);
                }
                if (profile.LightingModel == PLightingModel.PhysicalBased || profile.LightingModel == PLightingModel.BlinnPhong)
                {
                    //instance.SpecColor = EditorGUILayout.ColorField("Specular Color", instance.SpecColor);
                    profile.SpecColor = EditorGUILayout.ColorField(new GUIContent("Specular Color"), profile.SpecColor, true, false, true);
                    profile.Smoothness = EditorGUILayout.Slider("Smoothness", profile.Smoothness, 0f, 1f);
                }
            });
        }

        private void DrawFresnelSettingsGUI()
        {
            string label = "Fresnel";
            string id = "water-profile-fresnel";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                profile.FresnelStrength = EditorGUILayout.Slider("Strength", profile.FresnelStrength, 0f, 10f);
                profile.FresnelBias = EditorGUILayout.Slider("Bias", profile.FresnelBias, 0f, 1f);
            });
        }

        private void DrawLightAbsorbtionSettingsGUI()
        {
            string label = "Light Absorption";
            string id = "water-profile-absorption";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                profile.EnableLightAbsorption = EditorGUILayout.Toggle("Enable", profile.EnableLightAbsorption);
                if (profile.EnableLightAbsorption)
                {
                    profile.DepthColor = EditorGUILayout.ColorField("Depth Color", profile.DepthColor);
                    profile.MaxDepth = EditorGUILayout.FloatField("Max Depth", profile.MaxDepth);
                }
            });
        }

        private void DrawFoamSettingsGUI()
        {
            string label = "Foam";
            string id = "water-profile-foam";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                profile.EnableFoam = EditorGUILayout.Toggle("Enable", profile.EnableFoam);
                if (profile.EnableFoam)
                {
                    profile.EnableFoamHQ = EditorGUILayout.Toggle("High Quality", profile.EnableFoamHQ);
                    if (profile.EnableFoamHQ)
                    {
                        profile.FoamNoiseScaleHQ = EditorGUILayout.FloatField("Scale", profile.FoamNoiseScaleHQ);
                        profile.FoamNoiseSpeedHQ = EditorGUILayout.FloatField("Speed", profile.FoamNoiseSpeedHQ);
                    }
                    profile.FoamColor = EditorGUILayout.ColorField(new GUIContent("Color"), profile.FoamColor, true, true, true);

                    PEditorCommon.Header("Shoreline");
                    profile.FoamDistance = EditorGUILayout.FloatField("Distance", profile.FoamDistance);
                    profile.ShorelineFoamStrength = EditorGUILayout.Slider("Strength", profile.ShorelineFoamStrength, 0f, 1f);

                    if (profile.EnableWave)
                    {
                        PEditorCommon.Header("Crest");
                        profile.CrestMaxDepth = EditorGUILayout.FloatField("Max Depth", profile.CrestMaxDepth);
                        profile.CrestFoamStrength = EditorGUILayout.Slider("Strength", profile.CrestFoamStrength, 0f, 1f);
                    }

                    if (water.MeshType == PWaterMeshType.Spline)
                    {
                        PEditorCommon.Header("Slope");
                        profile.SlopeFoamDistance = EditorGUILayout.FloatField("Distance", profile.SlopeFoamDistance);
                        profile.SlopeFoamFlowSpeed = EditorGUILayout.FloatField("Flow Speed", profile.SlopeFoamFlowSpeed);
                        profile.SlopeFoamStrength = EditorGUILayout.Slider("Strength", profile.SlopeFoamStrength, 0f, 1f);
                    }

                }
            });
        }

        private void DrawRippleSettingsGUI()
        {
            string label = "Ripple";
            string id = "water-profile-ripple";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                profile.RippleHeight = EditorGUILayout.Slider("Height", profile.RippleHeight, 0f, 1f);
                profile.RippleNoiseScale = EditorGUILayout.FloatField("Scale", profile.RippleNoiseScale);
                profile.RippleSpeed = EditorGUILayout.FloatField("Speed", profile.RippleSpeed);
            });
        }

        private void DrawWaveSettingsGUI()
        {
            string label = "Wave";
            string id = "water-profile-wave";

            PEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableWave = EditorGUILayout.Toggle("Enable", profile.EnableWave);
                if (profile.EnableWave)
                {
                    profile.WaveDirection = EditorGUILayout.Slider("Direction", profile.WaveDirection, 0f, 360f);
                    profile.WaveSpeed = EditorGUILayout.FloatField("Speed", profile.WaveSpeed);
                    profile.WaveHeight = EditorGUILayout.FloatField("Height", profile.WaveHeight);
                    profile.WaveLength = EditorGUILayout.FloatField("Length", profile.WaveLength);
                    profile.WaveSteepness = EditorGUILayout.Slider("Steepness", profile.WaveSteepness, 0f, 1f);
                    profile.WaveDeform = EditorGUILayout.Slider("Deform", profile.WaveDeform, 0f, 1f);

                    water.UseWaveMask = EditorGUILayout.Toggle("Use Mask", water.UseWaveMask);
                    if (water.UseWaveMask)
                    {
                        waveMaskVisMode = (PWaveMaskVisualizationMode)EditorGUILayout.EnumPopup("Visualization", waveMaskVisMode);
                        EditorGUIUtility.wideMode = true;
                        water.WaveMaskBounds = EditorGUILayout.RectField("Bounds", water.WaveMaskBounds);
                        EditorGUIUtility.wideMode = false;
                        //#if TEXTURE_GRAPH
                        //                        water.UseEmbeddedWaveMask = EditorGUILayout.Toggle("Use Embedded Mask", water.UseEmbeddedWaveMask);
                        //                        if (water.UseEmbeddedWaveMask)
                        //                        {
                        //                            water.TextureGraphAsset = EditorGUILayout.ObjectField("Texture Graph Asset", water.TextureGraphAsset, typeof(TGraph), false) as TGraph;
                        //                            GUI.enabled = false;
                        //                            PEditorCommon.InlineTextureField("Mask", water.EmbeddedWaveMask, -1);
                        //                            GUI.enabled = water.TextureGraphAsset != null;
                        //                            if (GUILayout.Button("Embed Mask"))
                        //                            {
                        //                                water.EmbedWaveMask();
                        //                            }
                        //                            GUI.enabled = true;
                        //                        }
                        //                        else
                        //#endif
                        {
                            water.WaveMask = PEditorCommon.InlineTexture2DField("Mask", water.WaveMask, -1);
                        }
                    }
                }
            });
        }

        private void DrawReflectionSettingsGUI()
        {
            if (water.MeshType == PWaterMeshType.Spline)
                return;

            bool stereoEnable = false;
            if (Camera.main != null)
            {
                stereoEnable = Camera.main.stereoEnabled;
            }

            string label = "Reflection" + (stereoEnable ? " (Not support for VR)" : "");
            string id = "water-profile-reflection";

            GUI.enabled = !stereoEnable;
            PEditorCommon.Foldout(label, true, id, () =>
            {
                profile.EnableReflection = EditorGUILayout.Toggle("Enable", profile.EnableReflection);
                if (profile.EnableReflection)
                {
                    profile.ReflectCustomSkybox = EditorGUILayout.Toggle("Custom Skybox", profile.ReflectCustomSkybox);
                    profile.EnableReflectionPixelLight = EditorGUILayout.Toggle("Pixel Light", profile.EnableReflectionPixelLight);
                    profile.ReflectionClipPlaneOffset = EditorGUILayout.FloatField("Clip Plane Offset", profile.ReflectionClipPlaneOffset);

                    if (reflectionLayersSO != null)
                    {
                        EditorGUILayout.PropertyField(reflectionLayersSO);
                    }
                    so.ApplyModifiedProperties();

                    profile.ReflectionTextureResolution = EditorGUILayout.IntPopup("Resolution", profile.ReflectionTextureResolution, renderTextureSizeLabels, renderTextureSizes);
                    profile.ReflectionDistortionStrength = EditorGUILayout.FloatField("Distortion", profile.ReflectionDistortionStrength);
                }
            });
            GUI.enabled = true;
        }

        private void DrawRefractionSettingsGUI()
        {
            string label = "Refraction";
            string id = "water-profile-refraction";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                if (water.MeshType != PWaterMeshType.Spline)
                {
                    profile.EnableRefraction = EditorGUILayout.Toggle("Enable", profile.EnableRefraction);
                }
                if (profile.EnableRefraction || water.MeshType == PWaterMeshType.Spline)
                {
                    profile.RefractionDistortionStrength = EditorGUILayout.FloatField("Distortion", profile.RefractionDistortionStrength);
                }
            });
        }

        private void DrawCausticSettingsGUI()
        {
            string label = "Caustic";
            string id = "water-profile-caustic";

            PEditorCommon.Foldout(label, true, id, () =>
            {
                bool valid = (profile.EnableRefraction || water.MeshType == PWaterMeshType.Spline);
                if (!valid)
                {
                    EditorGUILayout.LabelField("Requires Refraction.", PEditorCommon.WordWrapItalicLabel);
                    profile.EnableCaustic = false;
                }

                GUI.enabled = valid;
                profile.EnableCaustic = EditorGUILayout.Toggle("Enable", profile.EnableCaustic);
                if (profile.EnableCaustic)
                {
                    profile.CausticTexture = PEditorCommon.InlineTextureField("Texture", profile.CausticTexture, -1);
                    profile.CausticSize = EditorGUILayout.FloatField("Size", profile.CausticSize);
                    profile.CausticStrength = EditorGUILayout.Slider("Strength", profile.CausticStrength, 0f, 5f);
                    profile.CausticDistortionStrength = EditorGUILayout.FloatField("Distortion", profile.CausticDistortionStrength);
                }
                GUI.enabled = true;
            });
        }

#if AURA_IN_PROJECT
        private void DrawAuraIntegrationSettingsGUI()
        {
            string label = "Aura 2 Integration";
            string id = "water-profile-aura-integration";

            PEditorCommon.Foldout(label, false, id, () =>
            {
                profile.ApplyAuraFog = EditorGUILayout.Toggle("Apply Fog", profile.ApplyAuraFog);
                profile.ApplyAuraLighting = EditorGUILayout.Toggle("Apply Lighting", profile.ApplyAuraLighting);
                if (profile.ApplyAuraLighting)
                {
                    profile.AuraLightingFactor = EditorGUILayout.FloatField("Lighting Factor", profile.AuraLightingFactor);
                }
            });
        }
#endif

        private void DuringSceneGUI(SceneView sv)
        {
            Tools.hidden = isEditingTileIndices || isEditingAreaMesh || isEditingSplineMesh;
            if (water.MeshType == PWaterMeshType.TileablePlane && isEditingTileIndices)
            {
                DrawEditingTilesGUI();
            }

            if (water.MeshType == PWaterMeshType.Area && isEditingAreaMesh)
            {
                DrawEditingAreaMeshGUI();
            }

            if (water.MeshType == PWaterMeshType.Spline && isEditingSplineMesh)
            {
                DrawEditingSplineGUI();
            }

            if (isEditingTileIndices || isEditingAreaMesh || isEditingSplineMesh)
            {
                DrawBounds();
            }

            bool isWaveSectionExpanded = PEditorCommon.IsFoldoutExpanded("water-profile-wave");
            if (isWaveSectionExpanded && water.UseWaveMask)
            {
                DrawWaveMaskBounds();
            }
        }

        private void DrawEditingTilesGUI()
        {
            tileEditingGUIDrawer.Draw();
        }

        private void DrawEditingAreaMeshGUI()
        {
            areaEditingGUIDrawer.Draw();
        }

        private void DrawEditingSplineGUI()
        {
            splineEditingGUIDrawer.Draw();
        }

        private void DrawBounds()
        {
            if (Event.current == null)
                return;
            if (water.Profile == null)
                return;

            Vector3 center = water.transform.TransformPoint(water.Bounds.center);
            Vector3 size = water.transform.TransformVector(water.Bounds.size);
            Handles.color = Color.yellow;
            Handles.DrawWireCube(center, size);
        }

        private void DrawWaveMaskBounds()
        {
            if (Event.current == null)
                return;
            if (water.Profile == null)
                return;

            Vector2 center = water.WaveMaskBounds.center;
            Vector3 worldCenter = new Vector3(center.x, water.transform.position.y, center.y);
            Vector2 size = water.WaveMaskBounds.size;
            Vector3 worldSize = new Vector3(size.x, 0.01f, size.y);
            Handles.color = Color.cyan;
            Handles.DrawWireCube(worldCenter, worldSize);
        }

        public void DrawDebugGUI()
        {
            string label = "Debug";
            string id = "debug" + water.GetInstanceID().ToString();

            PEditorCommon.Foldout(label, false, id, () =>
            {
                Camera[] cams = water.GetComponentsInChildren<Camera>();
                for (int i = 0; i < cams.Length; ++i)
                {
                    if (!cams[i].name.StartsWith("~"))
                        continue;
                    if (cams[i].targetTexture == null)
                        continue;
                    EditorGUILayout.LabelField(cams[i].name);
                    Rect r = GUILayoutUtility.GetAspectRect(1);
                    EditorGUI.DrawPreviewTexture(r, cams[i].targetTexture);
                    EditorGUILayout.Space();
                }
            });
        }

        public override bool RequiresConstantRepaint()
        {
            return willDrawDebugGUI;
        }

        private void OnRenderCamera(Camera cam)
        {
            bool isWaveSectionExpanded = PEditorCommon.IsFoldoutExpanded("water-profile-wave");
            if (isWaveSectionExpanded && water.UseWaveMask)
            {
                if (waveMaskVisMode != PWaveMaskVisualizationMode.None)
                {
                    Vector2 center = water.WaveMaskBounds.center;
                    Vector3 worldCenter = new Vector3(center.x, water.transform.position.y, center.y);
                    Vector2 size = water.WaveMaskBounds.size;
                    Vector3 worldSize = new Vector3(size.x, size.y, 1);

                    MaskVisualizerMaterial.SetTexture("_MainTex", water.WaveMask);
                    MaskVisualizerMaterial.DisableKeyword("FLOW");
                    MaskVisualizerMaterial.DisableKeyword("CREST");
                    MaskVisualizerMaterial.DisableKeyword("HEIGHT");
                    //if (waveMaskVisMode == PWaveMaskVisualizationMode.Flow)
                    //{
                    //    MaskVisualizerMaterial.EnableKeyword("FLOW");
                    //}
                    //else 
                    if (waveMaskVisMode == PWaveMaskVisualizationMode.Crest)
                    {
                        MaskVisualizerMaterial.EnableKeyword("CREST");
                    }
                    else if (waveMaskVisMode == PWaveMaskVisualizationMode.Height)
                    {
                        MaskVisualizerMaterial.EnableKeyword("HEIGHT");
                    }

                    Graphics.DrawMesh(
                        QuadMesh,
                        Matrix4x4.TRS(worldCenter, Quaternion.Euler(90, 0, 0), worldSize),
                        MaskVisualizerMaterial,
                        0,
                        cam,
                        0,
                        null,
                        ShadowCastingMode.Off,
                        false,
                        null,
                        LightProbeUsage.Off,
                        null);
                }
            }
        }

        private void OnRenderCameraSRP(ScriptableRenderContext context, Camera cam)
        {
            OnRenderCamera(cam);
        }
    }
}
