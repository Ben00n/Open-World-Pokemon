#ifndef PFRESNEL_INCLUDED
#define PFRESNEL_INCLUDED

#include "PUniforms.cginc"

void CalculateFresnelFactor(float3 worldPos, float3 worldNormal, out half fresnel)
{
	float3 worldViewDir = normalize(_WorldSpaceCameraPos.xyz  - worldPos);
	half vDotN = dot(worldViewDir, worldNormal);
	fresnel = saturate(pow(max(0, 1 - vDotN), _FresnelStrength)) - _FresnelBias;
}

#endif
