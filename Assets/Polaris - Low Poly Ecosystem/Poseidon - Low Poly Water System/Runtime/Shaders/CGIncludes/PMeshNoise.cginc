#ifndef PMESHNOISE_INCLUDED
#define PMESHNOISE_INCLUDED

#include "PUniforms.cginc"
#include "PCommon.cginc"

void ApplyMeshNoise(half amount, half noiseFrequency, inout float4 localPos)
{
	float4 worldPos = mul(unity_ObjectToWorld, localPos);
	half offsetX = NoiseTexVert(worldPos.xz/noiseFrequency)*amount* _PoseidonSineTime;
	half offsetZ = NoiseTexVert(-worldPos.xz/noiseFrequency)*amount* _PoseidonSineTime;

	worldPos += float4(offsetX, 0, offsetZ, 0);
	localPos = mul(unity_WorldToObject, worldPos);
}

void ApplyMeshNoise(inout float4 v0, inout float4 v1, inout float4 v2)
{
	ApplyMeshNoise(_MeshNoise, 100, v0);
	ApplyMeshNoise(_MeshNoise, 100, v1);
	ApplyMeshNoise(_MeshNoise, 100, v2);
}
#endif
