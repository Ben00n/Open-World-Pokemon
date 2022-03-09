#ifndef PRIPPLE_INCLUDED
#define PRIPPLE_INCLUDED

#include "PUniforms.cginc"
#include "PCommon.cginc"

float4 CalculateRippleVertexOffset(float4 localPos, float rippleHeight, float rippleSpeed, float rippleScale, float3 flowDir)
{
	float4 worldPos = mul(unity_ObjectToWorld, float4(localPos.xyz, 1));
	float2 noisePos = float2(worldPos.x, worldPos.z);

	rippleScale *= 0.01;
	rippleSpeed *= 0.1;

	float noiseBase = SampleVertexNoise((noisePos - flowDir.xz * _PoseidonTime * rippleSpeed) * rippleScale) * 0.5 + 0.5;
	float noiseFade = lerp(0.5, 1, SampleVertexNoise((noisePos + flowDir.xz * _PoseidonTime * rippleSpeed * 3) * rippleScale) * 0.5 + 0.5);

	float noise = abs(noiseBase - noiseFade);

	float4 offset = float4(0, noise * rippleHeight, 0, 0);
	return offset;
}

void ApplyRipple(inout float4 v0, inout float4 v1, inout float4 v2)
{
	float3 flowDir = float3(1, 1, 1);
	v0 += CalculateRippleVertexOffset(v0, _RippleHeight, _RippleSpeed, _RippleNoiseScale, flowDir);
	v1 += CalculateRippleVertexOffset(v1, _RippleHeight, _RippleSpeed, _RippleNoiseScale, flowDir);
	v2 += CalculateRippleVertexOffset(v2, _RippleHeight, _RippleSpeed, _RippleNoiseScale, flowDir);
}

void ApplyRipple(inout float4 vertex,inout float4 texcoord, inout float4 color, float4 texcoord1)
{
	float3 flow0 = float3(texcoord.w, 0, color.w);
	float3 flow1 = float3(texcoord1.x, 0, texcoord1.y);
	float3 flow2 = float3(texcoord1.z, 0, texcoord1.w);

	vertex += CalculateRippleVertexOffset(vertex, _RippleHeight, _RippleSpeed, _RippleNoiseScale, flow0);
	texcoord += CalculateRippleVertexOffset(texcoord, _RippleHeight, _RippleSpeed, _RippleNoiseScale, flow1);
	color += CalculateRippleVertexOffset(color, _RippleHeight, _RippleSpeed, _RippleNoiseScale, flow2);
}

#endif
