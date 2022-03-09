using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Poseidon
{
    [CreateAssetMenu(menuName = "Poseidon/Water Profile")]
    public class PWaterProfile : ScriptableObject
    {
        [SerializeField]
        private PLightingModel lightingModel;
        public PLightingModel LightingModel
        {
            get
            {
                return lightingModel;
            }
            set
            {
                lightingModel = value;
            }
        }

        [SerializeField]
        private bool useFlatLighting;
        public bool UseFlatLighting
        {
            get
            {
                return useFlatLighting;
            }
            set
            {
                useFlatLighting = value;
            }
        }

        [SerializeField]
        private int renderQueueIndex;
        public int RenderQueueIndex
        {
            get
            {
                return renderQueueIndex;
            }
            set
            {
                renderQueueIndex = value;
            }
        }

        [SerializeField]
        private Color color;
        public Color Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        [SerializeField]
        private Color specColor;
        public Color SpecColor
        {
            get
            {
                return specColor;
            }
            set
            {
                specColor = value;
            }
        }

        [SerializeField]
        private float smoothness;
        public float Smoothness
        {
            get
            {
                return smoothness;
            }
            set
            {
                smoothness = Mathf.Clamp01(value);
            }
        }

        [FormerlySerializedAs("enableLightAbsorbtion")]
        [SerializeField]
        private bool enableLightAbsorption;
        public bool EnableLightAbsorption
        {
            get
            {
                return enableLightAbsorption;
            }
            set
            {
                enableLightAbsorption = value;
            }
        }

        [SerializeField]
        private Color depthColor;
        public Color DepthColor
        {
            get
            {
                return depthColor;
            }
            set
            {
                depthColor = value;
            }
        }

        [SerializeField]
        private float maxDepth;
        public float MaxDepth
        {
            get
            {
                return maxDepth;
            }
            set
            {
                maxDepth = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private bool enableFoam;
        public bool EnableFoam
        {
            get
            {
                return enableFoam;
            }
            set
            {
                enableFoam = value;
            }
        }

        [SerializeField]
        private Color foamColor;
        public Color FoamColor
        {
            get
            {
                return foamColor;
            }
            set
            {
                foamColor = value;
            }
        }

        [SerializeField]
        private float foamDistance;
        public float FoamDistance
        {
            get
            {
                return foamDistance;
            }
            set
            {
                foamDistance = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private bool enableFoamHQ;
        public bool EnableFoamHQ
        {
            get
            {
                return enableFoamHQ;
            }
            set
            {
                enableFoamHQ = value;
            }
        }

        [SerializeField]
        private float foamNoiseScaleHQ;
        public float FoamNoiseScaleHQ
        {
            get
            {
                return foamNoiseScaleHQ;
            }
            set
            {
                foamNoiseScaleHQ = value;
            }
        }

        [SerializeField]
        private float foamNoiseSpeedHQ;
        public float FoamNoiseSpeedHQ
        {
            get
            {
                return foamNoiseSpeedHQ;
            }
            set
            {
                foamNoiseSpeedHQ = value;
            }
        }

        [SerializeField]
        private float shorelineFoamStrength;
        public float ShorelineFoamStrength
        {
            get
            {
                return shorelineFoamStrength;
            }
            set
            {
                shorelineFoamStrength = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float crestFoamStrength;
        public float CrestFoamStrength
        {
            get
            {
                return crestFoamStrength;
            }
            set
            {
                crestFoamStrength = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float crestMaxDepth;
        public float CrestMaxDepth
        {
            get
            {
                return crestMaxDepth;
            }
            set
            {
                crestMaxDepth = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float slopeFoamStrength;
        public float SlopeFoamStrength
        {
            get
            {
                return slopeFoamStrength;
            }
            set
            {
                slopeFoamStrength = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float slopeFoamFlowSpeed;
        public float SlopeFoamFlowSpeed
        {
            get
            {
                return slopeFoamFlowSpeed;
            }
            set
            {
                slopeFoamFlowSpeed = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float slopeFoamDistance;
        public float SlopeFoamDistance
        {
            get
            {
                return slopeFoamDistance;
            }
            set
            {
                slopeFoamDistance = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float rippleHeight;
        public float RippleHeight
        {
            get
            {
                return rippleHeight;
            }
            set
            {
                rippleHeight = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float rippleSpeed;
        public float RippleSpeed
        {
            get
            {
                return rippleSpeed;
            }
            set
            {
                rippleSpeed = value;
            }
        }

        [SerializeField]
        private float rippleNoiseScale;
        public float RippleNoiseScale
        {
            get
            {
                return rippleNoiseScale;
            }
            set
            {
                rippleNoiseScale = value;
            }
        }

        [SerializeField]
        private bool enableWave;
        public bool EnableWave
        {
            get
            {
                return enableWave;
            }
            set
            {
                enableWave = value;
            }
        }

        [SerializeField]
        private float waveDirection;
        public float WaveDirection
        {
            get
            {
                return waveDirection;
            }
            set
            {
                waveDirection = Mathf.Clamp(value, 0f, 360f);
            }
        }

        [SerializeField]
        private float waveSpeed;
        public float WaveSpeed
        {
            get
            {
                return waveSpeed;
            }
            set
            {
                waveSpeed = value;
            }
        }

        [SerializeField]
        private float waveHeight;
        public float WaveHeight
        {
            get
            {
                return waveHeight;
            }
            set
            {
                waveHeight = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float waveLength;
        public float WaveLength
        {
            get
            {
                return waveLength;
            }
            set
            {
                waveLength = Mathf.Max(1, value);
            }
        }

        [SerializeField]
        private float waveSteepness;
        public float WaveSteepness
        {
            get
            {
                return waveSteepness;
            }
            set
            {
                waveSteepness = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float waveDeform;
        public float WaveDeform
        {
            get
            {
                return waveDeform;
            }
            set
            {
                waveDeform = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float fresnelStrength;
        public float FresnelStrength
        {
            get
            {
                return fresnelStrength;
            }
            set
            {
                fresnelStrength = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float fresnelBias;
        public float FresnelBias
        {
            get
            {
                return fresnelBias;
            }
            set
            {
                fresnelBias = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private bool enableReflection;
        public bool EnableReflection
        {
            get
            {
                return enableReflection;
            }
            set
            {
                enableReflection = value;
            }
        }

        [SerializeField]
        private int reflectionTextureResolution;
        public int ReflectionTextureResolution
        {
            get
            {
                return reflectionTextureResolution;
            }
            set
            {
                reflectionTextureResolution = Mathf.Clamp(Mathf.ClosestPowerOfTwo(value), 32, 2048);
            }
        }

        [SerializeField]
        private bool enableReflectionPixelLight;
        public bool EnableReflectionPixelLight
        {
            get
            {
                return enableReflectionPixelLight;
            }
            set
            {
                enableReflectionPixelLight = value;
            }
        }

        [SerializeField]
        private float reflectionClipPlaneOffset;
        public float ReflectionClipPlaneOffset
        {
            get
            {
                return reflectionClipPlaneOffset;
            }
            set
            {
                reflectionClipPlaneOffset = value;
            }
        }

        [SerializeField]
        private LayerMask reflectionLayers;
        public LayerMask ReflectionLayers
        {
            get
            {
                return reflectionLayers;
            }
            set
            {
                reflectionLayers = value;
            }
        }

        [SerializeField]
        private bool reflectCustomSkybox;
        public bool ReflectCustomSkybox
        {
            get
            {
                return reflectCustomSkybox;
            }
            set
            {
                reflectCustomSkybox = value;
            }
        }

        [SerializeField]
        private float reflectionDistortionStrength;
        public float ReflectionDistortionStrength
        {
            get
            {
                return reflectionDistortionStrength;
            }
            set
            {
                reflectionDistortionStrength = value;
            }
        }

        [SerializeField]
        private bool enableRefraction;
        public bool EnableRefraction
        {
            get
            {
                return enableRefraction;
            }
            set
            {
                enableRefraction = value;
            }
        }

        [SerializeField]
        private float refractionDistortionStrength;
        public float RefractionDistortionStrength
        {
            get
            {
                return refractionDistortionStrength;
            }
            set
            {
                refractionDistortionStrength = value;
            }
        }

        [SerializeField]
        private bool enableCaustic;
        public bool EnableCaustic
        {
            get
            {
                return enableCaustic;
            }
            set
            {
                enableCaustic = value;
            }
        }

        [SerializeField]
        private Texture causticTexture;
        public Texture CausticTexture
        {
            get
            {
                return causticTexture;
            }
            set
            {
                causticTexture = value;
            }
        }

        [SerializeField]
        private float causticSize;
        public float CausticSize
        {
            get
            {
                return causticSize;
            }
            set
            {
                causticSize = value;
            }
        }

        [SerializeField]
        private float causticStrength;
        public float CausticStrength
        {
            get
            {
                return causticStrength;
            }
            set
            {
                causticStrength = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float causticDistortionStrength;
        public float CausticDistortionStrength
        {
            get
            {
                return causticDistortionStrength;
            }
            set
            {
                causticDistortionStrength = value;
            }
        }

#if AURA_IN_PROJECT
        [SerializeField]
        private bool applyAuraLighting;
        public bool ApplyAuraLighting
        {
            get
            {
                return applyAuraLighting;
            }
            set
            {
                applyAuraLighting = value;
            }
        }

        [SerializeField]
        private float auraLightingFactor;
        public float AuraLightingFactor
        {
            get
            {
                return auraLightingFactor;
            }
            set
            {
                auraLightingFactor = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private bool applyAuraFog;
        public bool ApplyAuraFog
        {
            get
            {
                return applyAuraFog;
            }
            set
            {
                applyAuraFog = value;
            }
        }
#endif

        public void Reset()
        {
            PWaterProfile defaultProfile = PPoseidonSettings.Instance.CalmWaterProfile;
            if (defaultProfile != null)
            {
                CopyFrom(defaultProfile);
            }
        }

        public void UpdateMaterialProperties(Material mat)
        {
            if (mat == null)
                return;
            PMat.SetActiveMaterial(mat);

            PMat.SetKeywordEnable(PMat.KW_LIGHTING_BLINN_PHONG, lightingModel == PLightingModel.BlinnPhong);
            PMat.SetKeywordEnable(PMat.KW_LIGHTING_LAMBERT, lightingModel == PLightingModel.Lambert);

            PMat.SetKeywordEnable(PMat.KW_FLAT_LIGHTING, useFlatLighting);

            PMat.SetRenderQueue(PMat.QUEUE_TRANSPARENT + RenderQueueIndex);

            PMat.SetColor(PMat.COLOR, color);
            PMat.SetColor(PMat.SPEC_COLOR, specColor);
            PMat.SetColor(PMat.SPEC_COLOR_BLINN_PHONG, specColor);
            PMat.SetFloat(PMat.SMOOTHNESS, smoothness);

            PMat.SetKeywordEnable(PMat.KW_LIGHT_ABSORPTION, enableLightAbsorption);
            PMat.SetColor(PMat.DEPTH_COLOR, depthColor);
            PMat.SetFloat(PMat.MAX_DEPTH, maxDepth);

            PMat.SetKeywordEnable(PMat.KW_FOAM, enableFoam);
            PMat.SetKeywordEnable(PMat.KW_FOAM_HQ, enableFoamHQ);
            PMat.SetKeywordEnable(PMat.KW_FOAM_CREST, crestFoamStrength > 0);
            PMat.SetKeywordEnable(PMat.KW_FOAM_SLOPE, slopeFoamStrength > 0);
            PMat.SetColor(PMat.FOAM_COLOR, foamColor);
            PMat.SetFloat(PMat.FOAM_DISTANCE, foamDistance);
            PMat.SetFloat(PMat.FOAM_NOISE_SCALE_HQ, foamNoiseScaleHQ);
            PMat.SetFloat(PMat.FOAM_NOISE_SPEED_HQ, foamNoiseSpeedHQ);
            PMat.SetFloat(PMat.SHORELINE_FOAM_STRENGTH, shorelineFoamStrength);
            PMat.SetFloat(PMat.CREST_FOAM_STRENGTH, crestFoamStrength);
            PMat.SetFloat(PMat.CREST_MAX_DEPTH, crestMaxDepth);
            PMat.SetFloat(PMat.SLOPE_FOAM_STRENGTH, slopeFoamStrength);
            PMat.SetFloat(PMat.SLOPE_FOAM_FLOW_SPEED, slopeFoamFlowSpeed);
            PMat.SetFloat(PMat.SLOPE_FOAM_DISTANCE, slopeFoamDistance);

            PMat.SetFloat(PMat.RIPPLE_HEIGHT, rippleHeight);
            PMat.SetFloat(PMat.RIPPLE_NOISE_SCALE, rippleNoiseScale);
            PMat.SetFloat(PMat.RIPPLE_SPEED, rippleSpeed);

            PMat.SetKeywordEnable(PMat.KW_WAVE, enableWave);
            PMat.SetVector(PMat.WAVE_DIRECTION, new Vector4(Mathf.Cos(waveDirection * Mathf.Deg2Rad), Mathf.Sin(waveDirection * Mathf.Deg2Rad), 0, 0));
            PMat.SetFloat(PMat.WAVE_SPEED, waveSpeed);
            PMat.SetFloat(PMat.WAVE_HEIGHT, waveHeight);
            PMat.SetFloat(PMat.WAVE_LENGTH, waveLength);
            PMat.SetFloat(PMat.WAVE_STEEPNESS, waveSteepness);
            PMat.SetFloat(PMat.WAVE_DEFORM, waveDeform);

            PMat.SetFloat(PMat.FRESNEL_STRENGTH, fresnelStrength);
            PMat.SetFloat(PMat.FRESNEL_BIAS, fresnelBias);

            PMat.SetKeywordEnable(PMat.KW_REFLECTION, enableReflection);
            PMat.SetFloat(PMat.REFLECTION_DISTORTION_STRENGTH, reflectionDistortionStrength);

            PMat.SetKeywordEnable(PMat.KW_REFRACTION, enableRefraction);
            PMat.SetFloat(PMat.REFRACTION_DISTORTION_STRENGTH, enableRefraction ? refractionDistortionStrength : 0);

            PMat.SetKeywordEnable(PMat.KW_CAUSTIC, enableCaustic);
            PMat.SetTexture(PMat.CAUSTIC_TEX, causticTexture);
            PMat.SetFloat(PMat.CAUSTIC_SIZE, causticSize);
            PMat.SetFloat(PMat.CAUSTIC_STRENGTH, causticStrength);
            PMat.SetFloat(PMat.CAUSTIC_DISTORTION_STRENGTH, causticDistortionStrength);

#if AURA_IN_PROJECT
            PMat.SetKeywordEnable(PMat.KW_AURA_LIGHTING, applyAuraLighting);
            PMat.SetFloat(PMat.AURA_LIGHTING_FACTOR, auraLightingFactor);
            PMat.SetKeywordEnable(PMat.KW_AURA_FOG, applyAuraFog);
#else
            PMat.SetKeywordEnable(PMat.KW_AURA_LIGHTING, false);
            PMat.SetKeywordEnable(PMat.KW_AURA_FOG, false);
#endif

            PMat.SetActiveMaterial(null);
        }

        public void CopyFrom(PWaterProfile p)
        {
            LightingModel = p.LightingModel;
            RenderQueueIndex = p.RenderQueueIndex;
            UseFlatLighting = p.UseFlatLighting;

            Color = p.Color;
            SpecColor = p.SpecColor;
            Smoothness = p.Smoothness;

            EnableLightAbsorption = p.EnableLightAbsorption;
            DepthColor = p.DepthColor;
            MaxDepth = p.MaxDepth;

            EnableFoam = p.EnableFoam;
            FoamColor = p.FoamColor;
            FoamDistance = p.FoamDistance;
            EnableFoamHQ = p.EnableFoamHQ;
            FoamNoiseScaleHQ = p.FoamNoiseScaleHQ;
            FoamNoiseSpeedHQ = p.FoamNoiseSpeedHQ;
            ShorelineFoamStrength = p.ShorelineFoamStrength;
            CrestFoamStrength = p.CrestFoamStrength;
            CrestMaxDepth = p.CrestMaxDepth;
            SlopeFoamStrength = p.SlopeFoamStrength;
            SlopeFoamFlowSpeed = p.SlopeFoamFlowSpeed;
            SlopeFoamDistance = p.SlopeFoamDistance;

            RippleHeight = p.RippleHeight;
            RippleSpeed = p.RippleSpeed;
            RippleNoiseScale = p.RippleNoiseScale;

            EnableWave = p.EnableWave;
            WaveDirection = p.WaveDirection;
            WaveSpeed = p.WaveSpeed;
            WaveHeight = p.WaveHeight;
            WaveLength = p.WaveLength;
            WaveSteepness = p.WaveSteepness;
            WaveDeform = p.WaveDeform;

            FresnelStrength = p.FresnelStrength;
            FresnelBias = p.FresnelBias;

            EnableReflection = p.EnableReflection;
            ReflectionTextureResolution = p.ReflectionTextureResolution;
            EnableReflectionPixelLight = p.EnableReflectionPixelLight;
            ReflectionClipPlaneOffset = p.ReflectionClipPlaneOffset;
            ReflectionLayers = p.ReflectionLayers;
            ReflectCustomSkybox = p.ReflectCustomSkybox;
            ReflectionDistortionStrength = p.ReflectionDistortionStrength;

            EnableRefraction = p.EnableRefraction;
            RefractionDistortionStrength = p.RefractionDistortionStrength;

            EnableCaustic = p.EnableCaustic;
            CausticTexture = p.CausticTexture;
            CausticSize = p.CausticSize;
            CausticStrength = p.CausticStrength;
            CausticDistortionStrength = p.CausticDistortionStrength;

#if AURA_IN_PROJECT
            ApplyAuraLighting = p.ApplyAuraLighting;
            AuraLightingFactor = p.AuraLightingFactor;
            ApplyAuraFog = p.ApplyAuraFog;
#endif
        }
    }
}
