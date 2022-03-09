#ifndef PREFLECTION_INCLUDED
#define PREFLECTION_INCLUDED

#include "PUniforms.cginc"

void SampleReflectionTexture(float4 screenPos, float3 worldNormal, out half4 color)
{
	float4 n = float4(worldNormal.x, worldNormal.z, 1, 1);
	half4 offset = half4(n.xy * _ReflectionTex_TexelSize.xy * _ReflectionDistortionStrength, 0, 0);

	float2 uv = float2(screenPos.xy / screenPos.w);
	//uv = UnityStereoTransformScreenSpaceTex(uv);
	uv += offset.xy;
	color = tex2D(_ReflectionTex, uv.xy);
}
#endif
