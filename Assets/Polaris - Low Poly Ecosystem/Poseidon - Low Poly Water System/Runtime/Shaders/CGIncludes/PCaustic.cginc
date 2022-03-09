#ifndef CAUSTIC_INCLUDED
#define CAUSTIC_INCLUDED

#include "PCommon.cginc"
#include "PDepth.cginc"

void SampleCausticTexture(float sceneDepth, float surfaceDepth, float3 fragWorldPos, float3 worldNormal, out half4 causticColor)
{
	fragWorldPos -= worldNormal * _CausticDistortionStrength;
	float fragToCamSqrDistance = SqrDistance(fragWorldPos, _WorldSpaceCameraPos.xyz);
	float refWorldPosToCamSqrDistance = (sceneDepth * sceneDepth * fragToCamSqrDistance) / (surfaceDepth * surfaceDepth);

	float3 fragWorldDir = normalize(fragWorldPos - _WorldSpaceCameraPos.xyz);
	float3 refWorldPos = fragWorldDir * sqrt(refWorldPosToCamSqrDistance) + _WorldSpaceCameraPos.xyz;

	float2 uv = refWorldPos.xz / (_CausticSize + 0.0000001);
	causticColor = tex2D(_CausticTex, uv + _PoseidonTime * 0.0125).r;
	causticColor *= _CausticStrength;
	float fade = lerp(0.25, 1, NoiseTexFrag(uv * 0.05 - _PoseidonTime * 0.0125));

	//#if LIGHT_ABSORPTION
	//	float waterDepth = sceneDepth - surfaceDepth;
	//	float depthFade = 1 - saturate(InverseLerpUnclamped(0, _MaxDepth, waterDepth));
	//	fade *= depthFade;
	//#endif

	causticColor *= fade;
	causticColor = saturate(causticColor);
}

#endif
