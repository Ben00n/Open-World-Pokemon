using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Rendering;

namespace Pinwheel.Jupiter
{
    public static class JMat
    {
        public static readonly int NOISE_TEX = Shader.PropertyToID("_NoiseTex");
        public static readonly int CLOUD_TEX = Shader.PropertyToID("_CloudTex");

        public static readonly int SKY_COLOR = Shader.PropertyToID("_SkyColor");
        public static readonly int HORIZON_COLOR = Shader.PropertyToID("_HorizonColor");
        public static readonly int GROUND_COLOR = Shader.PropertyToID("_GroundColor");
        public static readonly int HORIZON_THICKNESS = Shader.PropertyToID("_HorizonThickness");
        public static readonly int HORIZON_EXPONENT = Shader.PropertyToID("_HorizonExponent");
        public static readonly int HORIZON_STEP = Shader.PropertyToID("_HorizonStep");
        public static readonly int FOG_COLOR = Shader.PropertyToID("_FogColor");

        public static readonly string KW_STARS = "STARS";
        public static readonly string KW_STARS_LAYER_0 = "STARS_LAYER_0";
        public static readonly string KW_STARS_LAYER_1 = "STARS_LAYER_1";
        public static readonly string KW_STARS_LAYER_2 = "STARS_LAYER_2";
        public static readonly int STARS_START = Shader.PropertyToID("_StarsStartPosition");
        public static readonly int STARS_END = Shader.PropertyToID("_StarsEndPosition");
        public static readonly int STARS_OPACITY = Shader.PropertyToID("_StarsOpacity");
        public static readonly int STARS_COLOR_0 = Shader.PropertyToID("_StarsColor0");
        public static readonly int STARS_COLOR_1 = Shader.PropertyToID("_StarsColor1");
        public static readonly int STARS_COLOR_2 = Shader.PropertyToID("_StarsColor2");
        public static readonly int STARS_DENSITY_0 = Shader.PropertyToID("_StarsDensity0");
        public static readonly int STARS_DENSITY_1 = Shader.PropertyToID("_StarsDensity1");
        public static readonly int STARS_DENSITY_2 = Shader.PropertyToID("_StarsDensity2");
        public static readonly int STARS_SIZE_0 = Shader.PropertyToID("_StarsSize0");
        public static readonly int STARS_SIZE_1 = Shader.PropertyToID("_StarsSize1");
        public static readonly int STARS_SIZE_2 = Shader.PropertyToID("_StarsSize2");
        public static readonly int STARS_GLOW_0 = Shader.PropertyToID("_StarsGlow0");
        public static readonly int STARS_GLOW_1 = Shader.PropertyToID("_StarsGlow1");
        public static readonly int STARS_GLOW_2 = Shader.PropertyToID("_StarsGlow2");
        public static readonly int STARS_TWINKLE_0 = Shader.PropertyToID("_StarsTwinkle0");
        public static readonly int STARS_TWINKLE_1 = Shader.PropertyToID("_StarsTwinkle1");
        public static readonly int STARS_TWINKLE_2 = Shader.PropertyToID("_StarsTwinkle2");

        public static readonly string KW_STARS_BAKED = "STARS_BAKED";
        public static readonly int STARS_CUBEMAP = Shader.PropertyToID("_StarsCubemap");
        public static readonly int STARS_TWINKLE_MAP = Shader.PropertyToID("_StarsTwinkleMap");

        public static readonly string KW_SUN = "SUN";
        public static readonly string KW_SUN_USE_TEXTURE = "SUN_USE_TEXTURE";
        public static readonly int SUN_TEX = Shader.PropertyToID("_SunTex");
        public static readonly int SUN_COLOR = Shader.PropertyToID("_SunColor");
        public static readonly int SUN_SIZE = Shader.PropertyToID("_SunSize");
        public static readonly int SUN_SOFT_EDGE = Shader.PropertyToID("_SunSoftEdge");
        public static readonly int SUN_GLOW = Shader.PropertyToID("_SunGlow");
        public static readonly int SUN_DIRECTION = Shader.PropertyToID("_SunDirection");
        public static readonly int SUN_TRANSFORM_MATRIX = Shader.PropertyToID("_PositionToSunUV");
        public static readonly int SUN_LIGHT_COLOR = Shader.PropertyToID("_SunLightColor");
        public static readonly int SUN_LIGHT_INTENSITY = Shader.PropertyToID("_SunLightIntensity");

        public static readonly string KW_SUN_BAKED = "SUN_BAKED";
        public static readonly int SUN_CUBEMAP = Shader.PropertyToID("_SunCubemap");
        public static readonly int SUN_ROTATION_MATRIX = Shader.PropertyToID("_SunRotationMatrix");

        public static readonly string KW_MOON = "MOON";
        public static readonly string KW_MOON_USE_TEXTURE = "MOON_USE_TEXTURE";
        public static readonly int MOON_TEX = Shader.PropertyToID("_MoonTex");
        public static readonly int MOON_COLOR = Shader.PropertyToID("_MoonColor");
        public static readonly int MOON_SIZE = Shader.PropertyToID("_MoonSize");
        public static readonly int MOON_SOFT_EDGE = Shader.PropertyToID("_MoonSoftEdge");
        public static readonly int MOON_GLOW = Shader.PropertyToID("_MoonGlow");
        public static readonly int MOON_DIRECTION = Shader.PropertyToID("_MoonDirection");
        public static readonly int MOON_TRANSFORM_MATRIX = Shader.PropertyToID("_PositionToMoonUV");
        public static readonly int MOON_LIGHT_COLOR = Shader.PropertyToID("_MoonLightColor");
        public static readonly int MOON_LIGHT_INTENSITY = Shader.PropertyToID("_MoonLightIntensity");

        public static readonly string KW_MOON_BAKED = "MOON_BAKED";
        public static readonly int MOON_CUBEMAP = Shader.PropertyToID("_MoonCubemap");
        public static readonly int MOON_ROTATION_MATRIX = Shader.PropertyToID("_MoonRotationMatrix");

        public static readonly string KW_HORIZON_CLOUD = "HORIZON_CLOUD";
        public static readonly int HORIZON_CLOUD_COLOR = Shader.PropertyToID("_HorizonCloudColor");
        public static readonly int HORIZON_CLOUD_START = Shader.PropertyToID("_HorizonCloudStartPosition");
        public static readonly int HORIZON_CLOUD_END = Shader.PropertyToID("_HorizonCloudEndPosition");
        public static readonly int HORIZON_CLOUD_SIZE = Shader.PropertyToID("_HorizonCloudSize");
        public static readonly int HORIZON_CLOUD_STEP = Shader.PropertyToID("_HorizonCloudStep");
        public static readonly int HORIZON_CLOUD_ANIMATION_SPEED = Shader.PropertyToID("_HorizonCloudAnimationSpeed");

        public static readonly string KW_OVERHEAD_CLOUD = "OVERHEAD_CLOUD";
        public static readonly int OVERHEAD_CLOUD_COLOR = Shader.PropertyToID("_OverheadCloudColor");
        public static readonly int OVERHEAD_CLOUD_ALTITUDE = Shader.PropertyToID("_OverheadCloudAltitude");
        public static readonly int OVERHEAD_CLOUD_SIZE = Shader.PropertyToID("_OverheadCloudSize");
        public static readonly int OVERHEAD_CLOUD_STEP = Shader.PropertyToID("_OverheadCloudStep");
        public static readonly int OVERHEAD_CLOUD_ANIMATION_SPEED = Shader.PropertyToID("_OverheadCloudAnimationSpeed");
        public static readonly int OVERHEAD_CLOUD_FLOW_X = Shader.PropertyToID("_OverheadCloudFlowDirectionX");
        public static readonly int OVERHEAD_CLOUD_FLOW_Z = Shader.PropertyToID("_OverheadCloudFlowDirectionZ");
        public static readonly int OVERHEAD_CLOUD_REMAP_MIN = Shader.PropertyToID("_OverheadCloudRemapMin");
        public static readonly int OVERHEAD_CLOUD_REMAP_MAX = Shader.PropertyToID("_OverheadCloudRemapMax");
        public static readonly int OVERHEAD_CLOUD_SHADOW_CLIP_MASK = Shader.PropertyToID("_OverheadCloudShadowClipMask");

        public static readonly string KW_DETAIL_OVERLAY = "DETAIL_OVERLAY";
        public static readonly string KW_DETAIL_OVERLAY_ROTATION = "DETAIL_OVERLAY_ROTATION";
        public static readonly int DETAIL_OVERLAY_COLOR = Shader.PropertyToID("_DetailOverlayTintColor");
        public static readonly int DETAIL_OVERLAY_CUBEMAP = Shader.PropertyToID("_DetailOverlayCubemap");
        public static readonly int DETAIL_OVERLAY_LAYER = Shader.PropertyToID("_DetailOverlayLayer");
        public static readonly int DETAIL_OVERLAY_ROTATION_SPEED = Shader.PropertyToID("_DetailOverlayRotationSpeed");

        public static readonly string KW_ALLOW_STEP_EFFECT = "ALLOW_STEP_EFFECT";

        private static Material activeMaterial;

        public static void SetActiveMaterial(Material mat)
        {
            activeMaterial = mat;
        }

        public static void GetColor(int prop, ref Color value)
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

        public static void GetFloat(int prop, ref float value)
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

        public static void GetVector(int prop, ref Vector4 value)
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

        public static void GetTexture(int prop, ref Texture value)
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

        public static void SetColor(int prop, Color value)
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

        public static void SetFloat(int prop, float value)
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

        public static void SetVector(int prop, Vector4 value)
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

        public static void SetTexture(int prop, Texture value)
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

        public static void SetMatrix(int prop, Matrix4x4 value)
        {
            try
            {
                if (activeMaterial.HasProperty(prop))
                {
                    activeMaterial.SetMatrix(prop, value);
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
            activeMaterial.SetInt("_ZWrite", value ? 1 : 0);
        }

        public static void SetBlend(bool value)
        {
            activeMaterial.SetInt("_Blend", value ? 1 : 0);
        }

        public static void SetShader(Shader shader)
        {
            activeMaterial.shader = shader;
        }
    }
}