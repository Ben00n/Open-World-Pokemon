#ifndef PREFRACTION_INCLUDED
#define PREFRACTION_INCLUDED

void SampleRefractionTexture(float4 screenPos, float3 worldNormal, out half4 color)
{
	float2 texel = _RefractionTex_TexelSize.xy;
	//#if defined(POSEIDON_SRP)
		texel = 1/_ScreenParams.xy;
	//#endif

	float4 n = float4(worldNormal.x, worldNormal.z, 1, 1);
	half4 offset = half4(n.xy*texel*_RefractionDistortionStrength, 0, 0);

	float2 uv = float2(screenPos.xy / screenPos.w);
	uv = UnityStereoTransformScreenSpaceTex(uv);
	uv -= offset.xy;
	
	#if defined(POSEIDON_SRP)
		color = float4(SHADERGRAPH_SAMPLE_SCENE_COLOR(uv.xy), 1);
	#else
		color = tex2D(_RefractionTex, uv.xy);
	#endif
}

#endif
