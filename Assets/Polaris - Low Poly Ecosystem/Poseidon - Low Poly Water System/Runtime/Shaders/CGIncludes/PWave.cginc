#ifndef PWAVE_INCLUDED
	#define PWAVE_INCLUDED

	#include "PUniforms.cginc"
	#include "PCommon.cginc"

	float2 SampleSpline(float t, float2 anchor0, float2 anchor1, float2 tangent0, float2 tangent1)
	{
		float oneMinusT = 1 - t;
		float2 p = oneMinusT * oneMinusT * oneMinusT * anchor0 +
		3 * oneMinusT * oneMinusT * t * tangent0 +
		3 * oneMinusT * t * t * tangent1 +
		t * t * t * anchor1;
		return p;
	}

	half2 SampleWaveCurve(float t, half waveSteepness)
	{
		half2 left = half2(0, 0);
		half2 right = half2(1, 0);
		half2 middle = half2(lerp(0.5, 0.95, waveSteepness), 1);

		half2 anchor0 = left * (t < middle.x) + middle * (t >= middle.x);
		half2 anchor1 = middle * (t < middle.x) + right * (t >= middle.x);
		half tangentLength = 0.5;
		half2 tangent0 = float2(tangentLength, 0) * (t < middle.x) + float2(middle.x + tangentLength, middle.y) * (t >= middle.x);
		half2 tangent1 = float2(middle.x - tangentLength, middle.y) * (t < middle.x) + float2(1 - tangentLength, 0) * (t >= middle.x);
		half splineT = InverseLerpUnclamped(anchor0.x, anchor1.x, t);

		half2 p = SampleSpline(splineT, anchor0, anchor1, tangent0, tangent1);
		return p;
	}

	float4 CalculateWaveOffsetHQ(float4 vertex, float2 waveDirection, inout float crestMask)
	{
		float4 worldPos = mul(unity_ObjectToWorld, float4(vertex.xyz, 1));
		#if WAVE_MASK
			float maskU = InverseLerpUnclamped(_WaveMaskBounds.x, _WaveMaskBounds.z, worldPos.x);
			float maskV = InverseLerpUnclamped(_WaveMaskBounds.y, _WaveMaskBounds.w, worldPos.z);
			float4 mask = tex2Dlod(_WaveMask, float4(maskU, maskV, 0, 0));
			float inbounds = (maskU >= 0) * (maskU <= 1) * (maskV >= 0) * (maskV <= 1);

			//waveDirection = lerp(waveDirection, float2(mask.r * 2 - 1, mask.g * 2 - 1), inbounds);
		#endif

		half2 noisePos = worldPos.xz - 2 * _WaveSpeed * waveDirection * _PoseidonTime;
		half noise = SampleVertexNoise(noisePos * 0.001) * _WaveDeform;

		half dotValue = dot(worldPos.xz, waveDirection);
		half t = frac((dotValue - _WaveSpeed * _PoseidonTime) / _WaveLength - noise * _WaveDeform * 0.25);

		half2 p = SampleWaveCurve(t, _WaveSteepness);
		p.y -= lerp(0.5, 1, noise * 0.5 + 0.5);
		crestMask = saturate(p.y / _WaveHeight);
		float4 offset = float4(0, _WaveHeight * p.y, 0, 0);

		#if WAVE_MASK
			float heightMask = lerp(1, mask.a, inbounds);
			offset.y *= heightMask;
			float crestMaskMask = lerp(1, mask.b, inbounds);
			crestMask *= crestMaskMask;
		#endif

		return offset;
	}

	void ApplyWaveHQ(inout float4 v0, inout float4 v1, inout float4 v2, inout float crestMask)
	{
		float4 offset0 = CalculateWaveOffsetHQ(v0, _WaveDirection, crestMask);
		v0 += offset0;
		float tmp;
		v1 += CalculateWaveOffsetHQ(v1, _WaveDirection, tmp);
		v2 += CalculateWaveOffsetHQ(v2, _WaveDirection, tmp);
	}

	void ApplyWaveHQ(inout float4 vertex, inout float4 texcoord, inout float4 color, float4 texcoord1, inout float crestMask)
	{
		float2 flow0 = mul(unity_ObjectToWorld, float4(texcoord.w, 0, color.w, 0)).xz;
		float2 flow1 = mul(unity_ObjectToWorld, float4(texcoord1.x, 0, texcoord1.y, 0)).xz;
		float2 flow2 = mul(unity_ObjectToWorld, float4(texcoord1.z, 0, texcoord1.w, 0)).xz;

		float4 offset0 = CalculateWaveOffsetHQ(vertex, flow0, crestMask);
		vertex += offset0;
		float tmp;
		texcoord += CalculateWaveOffsetHQ(texcoord, flow1, tmp);
		color += CalculateWaveOffsetHQ(color, flow2, tmp);
	}
#endif
