Shader "Poseidon/Default/River"
{
    Properties
    {
        [HideInInspector] _Color("Color", Color) = (0.0, 0.8, 1.0, 0.5)
        [HideInInspector] _Specular("Specular Color", Color) = (0.1, 0.1, 0.1, 1)
        [HideInInspector] _Smoothness("Smoothness", Range(0.0, 1.0)) = 1

        [HideInInspector] _DepthColor("Depth Color", Color) = (0.0, 0.45, 0.65, 0.85)
        [HideInInspector] _MaxDepth("Max Depth", Float) = 5

        [HideInInspector] _FoamColor("Foam Color", Color) = (1, 1, 1, 1)
        [HideInInspector] _FoamDistance("Foam Distance", Float) = 1.2
        [HideInInspector] _FoamNoiseScaleHQ("Foam Noise Scale HQ", Float) = 3
        [HideInInspector] _FoamNoiseSpeedHQ("Foam Noise Speed HQ", Float) = 1
        [HideInInspector] _ShorelineFoamStrength("Shoreline Foam Strength", Float) = 1
        [HideInInspector] _CrestFoamStrength("Crest Foam Strength", Float) = 1
        [HideInInspector] _CrestMaxDepth("Crest Max Depth", Float) = 1
        [HideInInspector] _SlopeFoamStrength("Slope Foam Strength", Float) = 0.5
        [HideInInspector] _SlopeFoamFlowSpeed("Slope Foam Flow Speed", Float) = 20
        [HideInInspector] _SlopeFoamDistance("Slope Foam Distance", Float) = 100

        [HideInInspector] _RippleHeight("Ripple Height", Range(0, 1)) = 0.1
        [HideInInspector] _RippleSpeed("Ripple Speed", Float) = 5
        [HideInInspector] _RippleNoiseScale("Ripple Noise Scale", Float) = 1

        [HideInInspector] _WaveDirection("Wave Direction", Vector) = (1, 0, 0, 0)
        [HideInInspector] _WaveSpeed("Wave Speed", Float) = 1
        [HideInInspector] _WaveHeight("Wave Height", Float) = 1
        [HideInInspector] _WaveLength("Wave Length", Float) = 1
        [HideInInspector] _WaveSteepness("Wave Steepness", Float) = 1
        [HideInInspector] _WaveDeform("Wave Deform", Float) = 0.3
        [HideInInspector] _WaveMask("Wave Mask", 2D) = "white" {}
        [HideInInspector] _WaveMaskBounds("Wave Mask", Vector) = (0, 0, 0, 0)

        [HideInInspector] _FresnelStrength("Fresnel Strength", Range(0.0, 5.0)) = 1
        [HideInInspector] _FresnelBias("Fresnel Bias", Range(0.0, 1.0)) = 0

        [HideInInspector] _RefractionDistortionStrength("Refraction Distortion Strength", Float) = 1

        [HideInInspector] _CausticTex("Caustic Texture", 2D) = "black" { }
        [HideInInspector] _CausticSize("Caustic Size", Float) = 1
        [HideInInspector] _CausticStrength("Caustic Strength", Range(0.0, 1.0)) = 1
        [HideInInspector] _CausticDistortionStrength("Caustic Distortion Strength", Float) = 1

        [HideInInspector] _AuraLightingFactor("Aura Lighting Factor", Float) = 1
    }

    SubShader
    {
        LOD 200
        GrabPass
        {
            "_RefractionTex"
        }
        Pass
        {
            Name "FORWARD"
            Tags {  "LightMode" = "ForwardBase" 
                    "RenderType" = "Transparent" 
                    "Queue" = "Transparent+0" }

            Cull Back
            ZWrite On
            ZTest LEqual

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #include "Lighting.cginc"           
            #include "UnityPBSLighting.cginc"

            #define UNITY_INSTANCED_LOD_FADE
            #define UNITY_INSTANCED_SH
            #define UNITY_INSTANCED_LIGHTMAPSTS

            #pragma shader_feature_local WAVE
            #pragma shader_feature_local WAVE_MASK
            #pragma shader_feature_local LIGHT_ABSORPTION
            #pragma shader_feature_local FOAM
            #pragma shader_feature_local FOAM_HQ
            #pragma shader_feature_local FOAM_CREST
            #pragma shader_feature_local FOAM_SLOPE
            #pragma shader_feature_local CAUSTIC
            #pragma shader_feature_local LIGHTING_BLINN_PHONG
            #pragma shader_feature_local LIGHTING_LAMBERT
            #pragma shader_feature_local FLAT_LIGHTING
            #pragma shader_feature_local AURA_LIGHTING
            #pragma shader_feature_local AURA_FOG

            #define POSEIDON_RIVER
            #undef POSEIDON_WATER_ADVANCED
            #undef POSEIDON_BACK_FACE
            #undef POSEIDON_SRP
            #include "../CGIncludes/PUniforms.cginc"
            #include "../CGIncludes/PWave.cginc"
            #include "../CGIncludes/PLightAbsorption.cginc"
            #include "../CGIncludes/PFoam.cginc"
            #include "../CGIncludes/PRipple.cginc"
            #include "../CGIncludes/PFresnel.cginc"
            #include "../CGIncludes/PRefraction.cginc"
            #include "../CGIncludes/PCaustic.cginc"

            #include "BuiltinRP_SurfaceFunction.cginc"
            #include "BuiltinRP_ForwardBase.cginc"
            ENDCG
        }

        Pass
        {
            Name "FORWARD_ADD"
            Tags {  "LightMode" = "ForwardAdd"
                    "RenderType" = "Transparent"
                    "Queue" = "Transparent+0" }
            Blend SrcAlpha One
            Cull Back
            ZTest LEqual
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma multi_compile_instancing
            #pragma multi_compile_fog
            #pragma skip_variants INSTANCING_ON
            #pragma multi_compile_fwdadd_fullshadows noshadow
            #include "Lighting.cginc"           
            #include "UnityPBSLighting.cginc"

            #define UNITY_INSTANCED_LOD_FADE
            #define UNITY_INSTANCED_SH
            #define UNITY_INSTANCED_LIGHTMAPSTS

            #pragma shader_feature_local WAVE
            #pragma shader_feature_local WAVE_MASK
            #pragma shader_feature_local LIGHT_ABSORPTION
            #pragma shader_feature_local FOAM
            #pragma shader_feature_local FOAM_HQ
            #pragma shader_feature_local FOAM_CREST
            #pragma shader_feature_local FOAM_SLOPE
            #pragma shader_feature_local CAUSTIC
            #pragma shader_feature_local LIGHTING_BLINN_PHONG
            #pragma shader_feature_local LIGHTING_LAMBERT
            #pragma shader_feature_local FLAT_LIGHTING

            #define POSEIDON_RIVER
            #undef POSEIDON_WATER_ADVANCED
            #undef POSEIDON_BACK_FACE
            #undef POSEIDON_SRP
            #include "../CGIncludes/PUniforms.cginc"
            #include "../CGIncludes/PWave.cginc"
            #include "../CGIncludes/PLightAbsorption.cginc"
            #include "../CGIncludes/PFoam.cginc"
            #include "../CGIncludes/PRipple.cginc"
            #include "../CGIncludes/PFresnel.cginc"
            #include "../CGIncludes/PRefraction.cginc"
            #include "../CGIncludes/PCaustic.cginc"

            #include "BuiltinRP_SurfaceFunction.cginc"
            #include "BuiltinRP_ForwardAdd.cginc"
            ENDCG
        }
    }
    Fallback "Diffuse"
}
