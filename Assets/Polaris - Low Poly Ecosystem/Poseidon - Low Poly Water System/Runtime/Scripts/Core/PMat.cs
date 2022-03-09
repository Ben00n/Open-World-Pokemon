using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

namespace Pinwheel.Poseidon
{
    public static class PMat
    {
        public const string KW_LIGHTING_BLINN_PHONG = "LIGHTING_BLINN_PHONG";
        public const string KW_LIGHTING_LAMBERT = "LIGHTING_LAMBERT";

        public const string KW_FLAT_LIGHTING = "FLAT_LIGHTING";

        public const int QUEUE_TRANSPARENT = 3000;

        public const string COLOR = "_Color";
        public const string SPEC_COLOR = "_Specular";
        public const string SPEC_COLOR_BLINN_PHONG = "_SpecColor";
        public const string SMOOTHNESS = "_Smoothness";

        public const string KW_MESH_NOISE = "MESH_NOISE";
        public const string MESH_NOISE = "_MeshNoise";

        public const string KW_LIGHT_ABSORPTION = "LIGHT_ABSORPTION";
        public const string DEPTH_COLOR = "_DepthColor";
        public const string MAX_DEPTH = "_MaxDepth";
        
        public const string KW_FOAM = "FOAM";
        public const string KW_FOAM_HQ = "FOAM_HQ";
        public const string KW_FOAM_CREST = "FOAM_CREST";
        public const string KW_FOAM_SLOPE = "FOAM_SLOPE";
        public const string FOAM_COLOR = "_FoamColor";
        public const string FOAM_DISTANCE = "_FoamDistance";
        public const string FOAM_NOISE_SCALE_HQ = "_FoamNoiseScaleHQ";
        public const string FOAM_NOISE_SPEED_HQ = "_FoamNoiseSpeedHQ";
        public const string SHORELINE_FOAM_STRENGTH = "_ShorelineFoamStrength";
        public const string CREST_FOAM_STRENGTH = "_CrestFoamStrength";
        public const string CREST_MAX_DEPTH = "_CrestMaxDepth";
        public const string SLOPE_FOAM_STRENGTH = "_SlopeFoamStrength";
        public const string SLOPE_FOAM_FLOW_SPEED = "_SlopeFoamFlowSpeed";
        public const string SLOPE_FOAM_DISTANCE = "_SlopeFoamDistance";

        public const string RIPPLE_HEIGHT = "_RippleHeight";
        public const string RIPPLE_SPEED = "_RippleSpeed";
        public const string RIPPLE_NOISE_SCALE = "_RippleNoiseScale";

        public const string KW_WAVE = "WAVE";
        public const string WAVE_DIRECTION = "_WaveDirection";
        public const string WAVE_SPEED = "_WaveSpeed";
        public const string WAVE_HEIGHT = "_WaveHeight";
        public const string WAVE_LENGTH = "_WaveLength";
        public const string WAVE_STEEPNESS = "_WaveSteepness";
        public const string WAVE_DEFORM = "_WaveDeform";
        public const string KW_WAVE_MASK = "WAVE_MASK";
        public const string WAVE_MASK = "_WaveMask";
        public const string WAVE_MASK_BOUNDS = "_WaveMaskBounds";

        public const string FRESNEL_STRENGTH = "_FresnelStrength";
        public const string FRESNEL_BIAS = "_FresnelBias";

        public const string KW_REFLECTION = "REFLECTION";
        public const string KW_REFLECTION_BLUR = "REFLECTION_BLUR";
        public const string REFLECTION_TEX = "_ReflectionTex";
        public const string REFLECTION_DISTORTION_STRENGTH = "_ReflectionDistortionStrength";

        public const string KW_REFRACTION = "REFRACTION";
        public const string REFRACTION_TEX = "_RefractionTex";
        public const string REFRACTION_DISTORTION_STRENGTH = "_RefractionDistortionStrength";

        public const string KW_CAUSTIC = "CAUSTIC";
        public const string CAUSTIC_TEX = "_CausticTex";
        public const string CAUSTIC_SIZE = "_CausticSize";
        public const string CAUSTIC_STRENGTH = "_CausticStrength";
        public const string CAUSTIC_DISTORTION_STRENGTH = "_CausticDistortionStrength";

        public const string KW_BACK_FACE = "BACK_FACE";

        public const string NOISE_TEX = "_NoiseTex";
        public const string TIME = "_PoseidonTime";
        public const string SINE_TIME = "_PoseidonSineTime";

        public const string KW_AURA_LIGHTING = "AURA_LIGHTING";
        public const string AURA_LIGHTING_FACTOR = "_AuraLightingFactor";
        public const string KW_AURA_FOG = "AURA_FOG";

        public const string PP_NOISE_TEX = "_NoiseTex";
        public const string PP_INTENSITY = "_Intensity";

        public const string PP_WATER_LEVEL = "_WaterLevel";
        public const string PP_MAX_DEPTH = "_MaxDepth";
        public const string PP_SURFACE_COLOR_BOOST = "_SurfaceColorBoost";

        public const string PP_SHALLOW_FOG_COLOR = "_ShallowFogColor";
        public const string PP_DEEP_FOG_COLOR = "_DeepFogColor";
        public const string PP_VIEW_DISTANCE = "_ViewDistance";

        public const string KW_PP_CAUSTIC = "CAUSTIC";
        public const string PP_CAUSTIC_TEX = "_CausticTex";
        public const string PP_CAUSTIC_SIZE = "_CausticSize";
        public const string PP_CAUSTIC_STRENGTH = "_CausticStrength";

        public const string KW_PP_DISTORTION = "DISTORTION";
        public const string PP_DISTORTION_TEX = "_DistortionTex";
        public const string PP_DISTORTION_STRENGTH = "_DistortionStrength";
        public const string PP_WATER_FLOW_SPEED = "_WaterFlowSpeed";

        public const string PP_CAMERA_VIEW_DIR = "_CameraViewDir";
        public const string PP_CAMERA_FOV = "_CameraFov";
        public const string PP_CAMERA_TO_WORLD_MATRIX = "_CameraToWorldMatrix";

        public const string PP_WET_LENS_TEX = "_WetLensTex";
        public const string PP_WET_LENS_STRENGTH = "_Strength";

        private static Material activeMaterial;

        public static void SetActiveMaterial(Material mat)
        {
            activeMaterial = mat;
        }

        public static void GetColor(string prop, ref Color value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    value = activeMaterial.GetColor(prop);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void GetFloat(string prop, ref float value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    value = activeMaterial.GetFloat(prop);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void GetVector(string prop, ref Vector4 value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    value = activeMaterial.GetVector(prop);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void GetTexture(string prop, ref Texture value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    value = activeMaterial.GetTexture(prop);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void GetKeywordEnabled(string kw, ref bool value)
        {
            try
            {
                value = activeMaterial.IsKeywordEnabled(kw);
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void SetColor(string prop, Color value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    activeMaterial.SetColor(prop, value);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void SetFloat(string prop, float value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    activeMaterial.SetFloat(prop, value);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void SetVector(string prop, Vector4 value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    activeMaterial.SetVector(prop, value);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void SetTexture(string prop, Texture value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    activeMaterial.SetTexture(prop, value);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }
        
        public static void SetKeywordEnable(string kw, bool enable)
        {
            try
            {
                if (enable)
                {
                    activeMaterial.EnableKeyword(kw);
                }
                else
                {
                    activeMaterial.DisableKeyword(kw);
                }
            }
            catch (NullReferenceException nullEx)
            {
                Debug.LogError(nullEx.ToString());
            }
            catch { }
        }

        public static void SetOverrideTag(string tag, string value)
        {
            activeMaterial.SetOverrideTag(tag, value);
        }

        public static void SetRenderQueue(int queue)
        {
            activeMaterial.renderQueue = queue;
        }

        public static void SetRenderQueue(RenderQueue queue)
        {
            activeMaterial.renderQueue = (int)queue;
        }

        public static void SetSourceBlend(BlendMode mode)
        {
            activeMaterial.SetInt("_SrcBlend", (int)mode);
        }

        public static void SetDestBlend(BlendMode mode)
        {
            activeMaterial.SetInt("_DstBlend", (int)mode);
        }

        public static void SetZWrite(bool value)
        {
            activeMaterial.SetInt("ZWrite", value ? 1 : 0);
        }

        public static void SetBlend(bool value)
        {
            activeMaterial.SetInt("_Blend", value ? 1 : 0);
        }

        public static void SetShader(Shader shader)
        {
            int queue = activeMaterial.renderQueue;
            activeMaterial.shader = shader;
            activeMaterial.renderQueue = queue;
        }
    }
}