Shader "Hidden/Poseidon/WetLensURP"
{	
	Properties
	{
		_MainTex("Main Texture", 2D) = "white"{}
		_WetLensTex("Distortion Map", 2D) = "bump"{}
		_Strength("Strength", Float) = 1
	}

	HLSLINCLUDE

	#define POSEIDON_SRP
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
	#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Filtering.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	#include "Packages/com.unity.render-pipelines.universal/Shaders/PostProcessing/Common.hlsl"
	#include "../CGIncludes/PPostProcessingCommon.cginc"
	#include "../CGIncludes/PWetLensCommon.cginc"

	float4 Frag(Varyings i) : SV_Target
	{
		float2 uv = i.uv;
		float4 color = ApplyWetLens(uv);

		return color;
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