Shader "Hidden/Poseidon/WetLens"
{	
	Properties
	{
		//_MainTex("Main Texture", 2D) = "white"{}
		_WetLensTex("Distortion Map", 2D) = "bump"{}
		_Strength("Strength", Float) = 1
	}

	HLSLINCLUDE

	#undef POSEIDON_URP
	#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"
	#include "../CGIncludes/PPostProcessingCommon.cginc"
	#include "../CGIncludes/PWetLensCommon.cginc"

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 uv = i.texcoordStereo;
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

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}