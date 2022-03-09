#ifndef PDEPTH_INCLUDED
#define PDEPTH_INCLUDED
	
#include "PCommon.cginc"

#if !defined(POSEIDON_SRP)
	UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);
	float4 _CameraDepthTexture_TexelSize;
#endif

float GetSceneDepth(float4 screenPos)
{
	#if !defined(POSEIDON_SRP)
		screenPos = float4(screenPos.xyz, screenPos.w + 0.00000000001);
		float depth01 = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(screenPos));
		float perpsDepth = LinearEyeDepth(depth01);
	#else
		float2 uv = float2(screenPos.xy / screenPos.w);
		uv = UnityStereoTransformScreenSpaceTex(uv);

		float depth01 = SHADERGRAPH_SAMPLE_SCENE_DEPTH(uv);
		float perpsDepth = LinearEyeDepth(depth01, _ZBufferParams);
	#endif

	#if defined(UNITY_REVERSED_Z)
		depth01 = 1 - depth01;
	#endif
		
	float orthoDepth = lerp(GetNearPlane(), GetFarPlane(), depth01);
	float depth = lerp(perpsDepth, orthoDepth, IsOrtho());
	return depth;
}

float GetSurfaceDepth(float4 worldPos)
{
	float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
	return - viewPos.z;
}

float GetDepthFade(float sceneDepth, float surfaceDepth, float maxDepth)
{
	float waterDepth = sceneDepth - surfaceDepth;
	float depthFade = 1 - saturate(InverseLerpUnclamped(0, maxDepth, waterDepth));
	return depthFade;
}
#endif
