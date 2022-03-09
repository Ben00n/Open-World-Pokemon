using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Jupiter
{
    [CustomEditor(typeof(JSky))]
    public class JSkyInspector : Editor
    {
        private JSky sky;
        private JSkyProfile profile;
        private JDayNightCycle dnc;

        private const string DNC_LABEL = "Controlled by Day Night Cycle";

        private void OnEnable()
        {
            sky = target as JSky;
            profile = sky.Profile;
        }

        public override void OnInspectorGUI()
        {
            sky.Profile = JEditorCommon.ScriptableObjectField<JSkyProfile>("Profile", sky.Profile);
            profile = sky.Profile;
            if (sky.Profile == null)
                return;
            dnc = sky.GetComponent<JDayNightCycle>();

            DrawSceneReferencesGUI();
            EditorGUI.BeginChangeCheck();
            DrawSkyGUI();
            DrawStarsGUI();
            DrawSunGUI();
            DrawMoonGUI();
            DrawHorizonCloudGUI();
            DrawOverheadCloudGUI();
            DrawDetailOverlayGUI();
            DrawUtilitiesGUI();

            if (EditorGUI.EndChangeCheck())
            {
                profile.UpdateMaterialProperties();
                EditorUtility.SetDirty(sky);
                EditorUtility.SetDirty(profile);
            }

            DrawAddDayNightCycleGUI();
        }

        private void DrawSceneReferencesGUI()
        {
            string label = "Scene References";
            string id = "scene-references";

            JEditorCommon.Foldout(label, false, id, () =>
            {
                sky.SunLightSource = EditorGUILayout.ObjectField("Sun Light Source", sky.SunLightSource, typeof(Light), true) as Light;
                sky.MoonLightSource = EditorGUILayout.ObjectField("Moon Light Source", sky.MoonLightSource, typeof(Light), true) as Light;
            });
        }

        private void DrawSkyGUI()
        {
            string label = "Sky";
            string id = "sky" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.SkyColor = ColorField("Sky Color", profile.SkyColor, true, true, true, nameof(profile.SkyColor));
                profile.HorizonColor = ColorField("Horizon Color", profile.HorizonColor, true, true, true, nameof(profile.HorizonColor));
                profile.GroundColor = ColorField("Ground Color", profile.GroundColor, true, true, true, nameof(profile.GroundColor));
                if (profile.AllowStepEffect)
                {
                    profile.HorizonStep = EditorGUILayout.IntField("Horizon Step", profile.HorizonStep);
                }
                profile.HorizonExponent = FloatField("Horizon Exponent", profile.HorizonExponent, nameof(profile.HorizonExponent));
                profile.HorizonThickness = Slider("Horizon Thickness", profile.HorizonThickness, 0f, 1f, nameof(profile.HorizonThickness));
                profile.FogSyncOption = (JFogSyncOption)EditorGUILayout.EnumPopup("Fog Sync", profile.FogSyncOption);
                if (profile.FogSyncOption == JFogSyncOption.CustomColor)
                {
                    profile.FogColor = ColorField("Fog Color", profile.FogColor, true, true, false, nameof(profile.FogColor));
                }
            });
        }

        private void DrawStarsGUI()
        {
            string label = "Stars";
            string id = "stars" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableStars = EditorGUILayout.Toggle("Enable", profile.EnableStars);
                if (profile.EnableStars)
                {
                    profile.UseBakedStars = EditorGUILayout.Toggle("Baked", profile.UseBakedStars);
                }
                if (profile.EnableStars && !profile.UseBakedStars)
                {
                    profile.StarsStartPosition = Slider("Start", profile.StarsStartPosition, -1, 1, nameof(profile.StarsStartPosition));
                    profile.StarsEndPosition = Slider("End", profile.StarsEndPosition, -1, 1, nameof(profile.StarsEndPosition));
                    profile.StarsOpacity = Slider("Opacity", profile.StarsOpacity, 0f, 1f, nameof(profile.StarsOpacity));
                    profile.StarsLayerCount = EditorGUILayout.DelayedIntField("Layers", profile.StarsLayerCount);

                    if (profile.StarsLayerCount > 0)
                    {
                        JEditorCommon.Separator();
                        EditorGUILayout.LabelField("Layer 0");
                        EditorGUI.indentLevel += 1;
                        profile.StarsColor0 = ColorField("Color", profile.StarsColor0, true, true, true, nameof(profile.StarsColor0));
                        profile.StarsDensity0 = Slider("Density", profile.StarsDensity0, 0.01f, 1f, nameof(profile.StarsDensity0));
                        profile.StarsSize0 = Slider("Size", profile.StarsSize0, 0.01f, 1f, nameof(profile.StarsSize0));
                        profile.StarsGlow0 = Slider("Glow", profile.StarsGlow0, 0f, 1f, nameof(profile.StarsGlow0));
                        profile.StarsTwinkle0 = FloatField("Twinkle", profile.StarsTwinkle0, nameof(profile.StarsTwinkle0));
                        EditorGUI.indentLevel -= 1;
                    }

                    if (profile.StarsLayerCount > 1)
                    {
                        JEditorCommon.Separator();
                        EditorGUILayout.LabelField("Layer 1");
                        EditorGUI.indentLevel += 1;
                        profile.StarsColor1 = ColorField("Color", profile.StarsColor1, true, true, true, nameof(profile.StarsColor1));
                        profile.StarsDensity1 = Slider("Density", profile.StarsDensity1, 0.01f, 1f, nameof(profile.StarsDensity1));
                        profile.StarsSize1 = Slider("Size", profile.StarsSize1, 0.01f, 1f, nameof(profile.StarsSize1));
                        profile.StarsGlow1 = Slider("Glow", profile.StarsGlow1, 0f, 1f, nameof(profile.StarsGlow1));
                        profile.StarsTwinkle1 = FloatField("Twinkle", profile.StarsTwinkle1, nameof(profile.StarsTwinkle1));
                        EditorGUI.indentLevel -= 1;
                    }

                    if (profile.StarsLayerCount > 2)
                    {
                        JEditorCommon.Separator();
                        EditorGUILayout.LabelField("Layer 2");
                        EditorGUI.indentLevel += 1;
                        profile.StarsColor2 = ColorField("Color", profile.StarsColor2, true, true, true, nameof(profile.StarsColor2));
                        profile.StarsDensity2 = Slider("Density", profile.StarsDensity2, 0.01f, 1f, nameof(profile.StarsDensity2));
                        profile.StarsSize2 = Slider("Size", profile.StarsSize2, 0.01f, 1f, nameof(profile.StarsSize2));
                        profile.StarsGlow2 = Slider("Glow", profile.StarsGlow2, 0f, 1f, nameof(profile.StarsGlow2));
                        profile.StarsTwinkle2 = FloatField("Twinkle", profile.StarsTwinkle2, nameof(profile.StarsTwinkle2));
                        EditorGUI.indentLevel -= 1;
                    }
                }
                if (profile.EnableStars && profile.UseBakedStars)
                {
                    profile.StarsCubemap = JEditorCommon.InlineCubemapField("Cubemap", profile.StarsCubemap, -1);
                    profile.StarsTwinkleMap = JEditorCommon.InlineTexture2DField("Twinkle Map", profile.StarsTwinkleMap, -1);
                    profile.StarsOpacity = Slider("Opacity", profile.StarsOpacity, 0f, 1f, nameof(profile.StarsOpacity));
                }
            });
        }

        private void DrawSunGUI()
        {
            string label = "Sun";
            string id = "sun" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableSun = EditorGUILayout.Toggle("Enable", profile.EnableSun);
                if (profile.EnableSun)
                {
                    profile.UseBakedSun = EditorGUILayout.Toggle("Baked", profile.UseBakedSun);
                }
                if (profile.EnableSun && !profile.UseBakedSun)
                {
                    profile.SunTexture = JEditorCommon.InlineTexture2DField("Texture", profile.SunTexture, -1);
                    profile.SunColor = ColorField("Color", profile.SunColor, true, true, true, nameof(profile.SunColor));
                    profile.SunSize = Slider("Size", profile.SunSize, 0f, 1f, nameof(profile.SunSize));
                    profile.SunSoftEdge = Slider("Soft Edge", profile.SunSoftEdge, 0f, 1f, nameof(profile.SunSoftEdge));
                    profile.SunGlow = Slider("Glow", profile.SunGlow, 0f, 1f, nameof(profile.SunGlow));
                }
                if (profile.EnableSun && profile.UseBakedSun)
                {
                    profile.SunCubemap = JEditorCommon.InlineCubemapField("Cubemap", profile.SunCubemap, -1);
                }
                if (profile.EnableSun)
                {
                    profile.SunLightColor = ColorField("Light Color", profile.SunLightColor, true, false, false, nameof(profile.SunLightColor));
                    profile.SunLightIntensity = FloatField("Light Intensity", profile.SunLightIntensity, nameof(profile.SunLightIntensity));
                }
            });
        }

        private void DrawMoonGUI()
        {
            string label = "Moon";
            string id = "moon" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableMoon = EditorGUILayout.Toggle("Enable", profile.EnableMoon);
                if (profile.EnableMoon)
                {
                    profile.UseBakedMoon = EditorGUILayout.Toggle("Baked", profile.UseBakedMoon);
                }
                if (profile.EnableMoon && !profile.UseBakedMoon)
                {
                    profile.MoonTexture = JEditorCommon.InlineTexture2DField("Texture", profile.MoonTexture, -1);
                    profile.MoonColor = ColorField("Color", profile.MoonColor, true, true, true, nameof(profile.MoonColor));
                    profile.MoonSize = Slider("Size", profile.MoonSize, 0f, 1f, nameof(profile.MoonSize));
                    profile.MoonSoftEdge = Slider("Soft Edge", profile.MoonSoftEdge, 0f, 1f, nameof(profile.MoonSoftEdge));
                    profile.MoonGlow = Slider("Glow", profile.MoonGlow, 0f, 1f, nameof(profile.MoonGlow));
                }
                if (profile.EnableMoon && profile.UseBakedMoon)
                {
                    profile.MoonCubemap = JEditorCommon.InlineCubemapField("Cubemap", profile.MoonCubemap, -1);
                }
                if (profile.EnableMoon)
                {
                    profile.MoonLightColor = ColorField("Light Color", profile.MoonLightColor, true, false, false, nameof(profile.MoonLightColor));
                    profile.MoonLightIntensity = FloatField("Light Intensity", profile.MoonLightIntensity, nameof(profile.MoonLightIntensity));
                }
            });
        }

        private void DrawHorizonCloudGUI()
        {
            string label = "Horizon Cloud";
            string id = "horizon-cloud" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableHorizonCloud = EditorGUILayout.Toggle("Enable", profile.EnableHorizonCloud);
                if (profile.EnableHorizonCloud)
                {
                    profile.CustomCloudTexture = JEditorCommon.InlineTexture2DField("Texture", profile.CustomCloudTexture, -1);
                    profile.HorizonCloudColor = ColorField("Color", profile.HorizonCloudColor, true, true, false, nameof(profile.HorizonCloudColor));
                    profile.HorizonCloudStartPosition = Slider("Start", profile.HorizonCloudStartPosition, -1, 1, nameof(profile.HorizonCloudStartPosition));
                    profile.HorizonCloudEndPosition = Slider("End", profile.HorizonCloudEndPosition, -1, 1, nameof(profile.HorizonCloudEndPosition));
                    profile.HorizonCloudSize = FloatField("Size", profile.HorizonCloudSize, nameof(profile.HorizonCloudSize));
                    if (profile.AllowStepEffect)
                    {
                        profile.HorizonCloudStep = EditorGUILayout.IntField("Step", profile.HorizonCloudStep);
                    }
                    profile.HorizonCloudAnimationSpeed = FloatField("Animation Speed", profile.HorizonCloudAnimationSpeed, nameof(profile.HorizonCloudAnimationSpeed));
                }
            });
        }

        private void DrawOverheadCloudGUI()
        {
            string label = "Overhead Cloud";
            string id = "overhead-cloud" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableOverheadCloud = EditorGUILayout.Toggle("Enable", profile.EnableOverheadCloud);
                if (profile.EnableOverheadCloud)
                {
                    profile.CustomCloudTexture = JEditorCommon.InlineTexture2DField("Texture", profile.CustomCloudTexture, -1);
                    profile.OverheadCloudColor = ColorField("Color", profile.OverheadCloudColor, true, true, false, nameof(profile.OverheadCloudColor));
                    profile.OverheadCloudAltitude = FloatField("Altitude", profile.OverheadCloudAltitude, nameof(profile.OverheadCloudAltitude));
                    profile.OverheadCloudSize = FloatField("Size", profile.OverheadCloudSize, nameof(profile.OverheadCloudSize));
                    if (profile.AllowStepEffect)
                    {
                        profile.OverheadCloudStep = EditorGUILayout.IntField("Step", profile.OverheadCloudStep);
                    }
                    profile.OverheadCloudAnimationSpeed = FloatField("Animation Speed", profile.OverheadCloudAnimationSpeed, nameof(profile.OverheadCloudAnimationSpeed));
                    profile.OverheadCloudFlowDirectionX = Slider("Flow X", profile.OverheadCloudFlowDirectionX, -1, 1, nameof(profile.OverheadCloudFlowDirectionX));
                    profile.OverheadCloudFlowDirectionZ = Slider("Flow Z", profile.OverheadCloudFlowDirectionZ, -1, 1, nameof(profile.OverheadCloudFlowDirectionZ));
                    profile.OverheadCloudRemapMin = FloatField("Remap Min", profile.OverheadCloudRemapMin, nameof(profile.OverheadCloudRemapMin));
                    profile.OverheadCloudRemapMax = FloatField("Remap Max", profile.OverheadCloudRemapMax, nameof(profile.OverheadCloudRemapMax));
                    profile.OverheadCloudCastShadow = EditorGUILayout.Toggle("Cast Shadow (Experimental)", profile.OverheadCloudCastShadow);
                    if (profile.OverheadCloudCastShadow)
                    {
                        profile.OverheadCloudShadowClipMask = EditorGUILayout.Slider("Clip Mask", profile.OverheadCloudShadowClipMask, 0f, 1f);
                    }
                }
            });
        }

        private void DrawDetailOverlayGUI()
        {
            string label = "Detail Overlay";
            string id = "detail-overlay" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.EnableDetailOverlay = EditorGUILayout.Toggle("Enable", profile.EnableDetailOverlay);
                if (profile.EnableDetailOverlay)
                {
                    profile.DetailOverlayTintColor = ColorField("Color", profile.DetailOverlayTintColor, true, true, false, nameof(profile.DetailOverlayTintColor));
                    profile.DetailOverlayCubeMap = JEditorCommon.InlineCubemapField("Cubemap", profile.DetailOverlayCubeMap, -1);
                    profile.DetailOverlayLayer = (JDetailOverlayLayer)EditorGUILayout.EnumPopup("Layer", profile.DetailOverlayLayer);
                    profile.DetailOverlayRotationSpeed = FloatField("Rotation Speed", profile.DetailOverlayRotationSpeed, nameof(profile.DetailOverlayRotationSpeed));
                }
            });
        }

        private void DrawUtilitiesGUI()
        {
            string label = "Utilities";
            string id = "utilities" + profile.GetInstanceID(); ;

            JEditorCommon.Foldout(label, false, id, () =>
            {
                profile.AllowStepEffect = EditorGUILayout.Toggle("Allow Step Effect", profile.AllowStepEffect);
            });
        }

        private void DrawAddDayNightCycleGUI()
        {
            JDayNightCycle cycle = sky.GetComponent<JDayNightCycle>();
            if (cycle != null)
                return;

            string label = "Day Night Cycle";
            string id = "day-night-cycle" + sky.GetInstanceID();

            JEditorCommon.Foldout(label, false, id, () =>
            {
                if (GUILayout.Button("Add Day Night Cycle"))
                {
                    sky.gameObject.AddComponent<JDayNightCycle>();
                }
            });
        }

        private Color ColorField(string label, Color color, bool colorPicker, bool alpha, bool hdr, string propName)
        {
            if (dnc != null && dnc.enabled == true && dnc.Profile != null && dnc.Profile.ContainProperty(propName))
            {
                EditorGUILayout.LabelField(label, DNC_LABEL, JEditorCommon.ItalicLabel);
                return color;
            }
            else
            {
                return EditorGUILayout.ColorField(new GUIContent(label), color, colorPicker, alpha, hdr);
            }
        }

        private float FloatField(string label, float value, string propName)
        {
            if (dnc != null && dnc.enabled == true && dnc.Profile != null && dnc.Profile.ContainProperty(propName))
            {
                EditorGUILayout.LabelField(label, DNC_LABEL, JEditorCommon.ItalicLabel);
                return value;
            }
            else
            {
                return EditorGUILayout.FloatField(label, value);
            }
        }

        private float Slider(string label, float value, float min, float max, string propName)
        {
            if (dnc != null && dnc.enabled == true && dnc.Profile != null && dnc.Profile.ContainProperty(propName))
            {
                EditorGUILayout.LabelField(label, DNC_LABEL, JEditorCommon.ItalicLabel);
                return value;
            }
            else
            {
                return EditorGUILayout.Slider(label, value, min, max);
            }
        }
    }
}
