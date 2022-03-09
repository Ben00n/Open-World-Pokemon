Shader "Hidden/Poseidon/UnderwaterURP"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white"{}
		_WaterLevel("Water Level", Float) = 0
		_MaxDepth("Max Depth", Float) = 30
		_SurfaceColorBoost("Surface Color Boost", Float) = 1
		
		_ShallowFogColor("Shallow Fog", Color) = (0,0,0,0)
		_DeepFogColor("Deep Fog", Color) = (0,0,0,0)
		_ViewDistance("ViewDistance", Float) = 10
		
		_CausticTex("Caustic Texture", 2D) = "black"{}
		_CausticSize("Caustic Size", Float) = 10
		_CausticStrength("Caustic Strength", Float) = 1
		
		_DistortionTex("Distortion Texture", 2D) = "bump"{}
		_DistortionStrength("Distortion Strength", Float) = 1
		_WaterFlowSpeed("Water Flow Speed", Float) = 1

		_NoiseTex("Noise Texture", 2D) = "black" {}

		_CameraViewDir("Camera View Dir", Vector) = (0,0,1,0)
		_CameraFov("Camera FOV", Float) = 60
		_Intensity("Intensity", Float) = 1
	}

    HLSLINCLUDE

	#pragma shader_feature_local CAUSTIC
	#pragma shader_feature_local DISTORTION

	#define POSEIDON_SRP
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "../CGIncludes/PPostProcessingCommon.cginc"
	#include "../CGIncludes/PUnderwaterCommon.cginc"

    float4 Frag(Varyings i) : SV_Target
    {
		float2 uv = i.uv;
		float4 result = ApplyUnderwater(uv);
        return result;
    }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex Vert
                #pragma fragment Frag

            ENDHLSL
        }
    }
}