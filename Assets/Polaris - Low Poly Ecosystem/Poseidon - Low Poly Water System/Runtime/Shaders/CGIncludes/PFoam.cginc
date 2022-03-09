#ifndef PFOAM_INCLUDED
#define PFOAM_INCLUDED

#include "PUniforms.cginc"
#include "PCommon.cginc"
#include "PDepth.cginc"

void CalculateFoamColor(float sceneDepth, float surfaceDepth, float3 worldPos, float3 normal, float crestMask, out half4 foamColor)
{
	float waterDepth = sceneDepth - surfaceDepth;
	float depthClip = waterDepth <= _ShorelineFoamStrength * _FoamDistance;

	foamColor = depthClip * _FoamColor * (_ShorelineFoamStrength > 0);

#if WAVE && FOAM_CREST
	float dt = waterDepth / (_WaveHeight + _CrestMaxDepth);
	float crestDepthFade = 1 - dt * dt * dt * dt * dt * dt * dt * dt; //1-x^8
	float crestFade = saturate(InverseLerpUnclamped(0, 1 - _CrestFoamStrength, crestMask));
	crestFade = crestFade * 1.2 * crestDepthFade;
	float crestClip = crestFade >= 0.5;
	float crestDepthClip = waterDepth <= (_WaveHeight + _CrestMaxDepth);
	float4 crestColor = _FoamColor * crestClip * (_CrestFoamStrength > 0);

	foamColor = max(foamColor, crestColor);
#endif

#if defined(POSEIDON_RIVER)
#if FOAM_SLOPE
	half normalDotUp = (1 - normal.y * normal.y);
	half slopeFade = 1 - frac((worldPos.y + _PoseidonTime * _SlopeFoamFlowSpeed) / _SlopeFoamDistance);
	
	slopeFade *= _SlopeFoamStrength;
	slopeFade *= normalDotUp;
	half slopeClip = slopeFade >= 0.1;
	half4 slopeColor = _FoamColor * slopeClip;
	foamColor = max(foamColor, slopeColor);
#endif
#endif
}

void CalculateFoamColorHQ(float sceneDepth, float surfaceDepth, float3 worldPos, float3 normal, float crestMask, out half4 foamColor)
{
	float waterDepth = sceneDepth - surfaceDepth;
	float depthClip = waterDepth <= _FoamDistance;

	half noiseScale = _FoamNoiseScaleHQ;
	half noiseSpeed = _FoamNoiseSpeedHQ;
	noiseScale *= 0.1;
	noiseSpeed *= 0.01;

	half noiseBase = SampleFragmentNoise(worldPos.xz * noiseScale - noiseSpeed.xx * _PoseidonTime * 0.2) * 0.5 + 0.5;
	half noiseFade = SampleFragmentNoise(worldPos.xz * noiseScale + noiseSpeed.xx * _PoseidonTime) * 0.5 + 0.5;
	half noise = noiseBase * noiseFade;
	half depthFade = saturate(InverseLerpUnclamped(0, _ShorelineFoamStrength * _FoamDistance * (1 + noise), waterDepth));
	half noiseClip = noise >= depthFade;

	foamColor = depthClip * noiseClip * _FoamColor * (_ShorelineFoamStrength > 0);

#if WAVE && FOAM_CREST
	float dt = waterDepth / (_WaveHeight + _CrestMaxDepth);
	float crestDepthFade = 1 - dt * dt * dt * dt * dt * dt * dt * dt; //1-x^8
	float crestFade = saturate(InverseLerpUnclamped(0, 1 - _CrestFoamStrength, crestMask));
	crestFade = crestFade * (1 + noise) * crestDepthFade;
	float crestClip = crestFade >= 0.5;
	float crestDepthClip = waterDepth <= (_WaveHeight + _CrestMaxDepth);
	float4 crestColor = _FoamColor * crestClip * (_CrestFoamStrength > 0);
	foamColor = max(foamColor, crestColor);
#endif

#if defined(POSEIDON_RIVER)
#if FOAM_SLOPE
	half normalDotUp = (1 - normal.y * normal.y);
	half slopeFade = 1 - frac((worldPos.y + _PoseidonTime * _SlopeFoamFlowSpeed) / _SlopeFoamDistance);
	half midPoint = 0.9;
	slopeFade = (slopeFade < midPoint) * (slopeFade / midPoint) + (slopeFade >= midPoint) * (lerp(1, 0, InverseLerpUnclamped(midPoint, 1, slopeFade)));
	slopeFade *= _SlopeFoamStrength;
	slopeFade -= noise;
	slopeFade *= normalDotUp;
	half slopeClip = slopeFade >= 0.1;
	half4 slopeColor = _FoamColor * slopeClip;
	foamColor = max(foamColor, slopeColor);
#endif
#endif
}

#endif
