#ifndef PUNIFORMS_INCLUDED
#define PUNIFORMS_INCLUDED

uniform float _PoseidonTime;
uniform float _PoseidonSineTime;
uniform half _MeshNoise;

uniform half4 _Color;
uniform half4 _Specular;
uniform half _Smoothness;

uniform half4 _DepthColor;
uniform half _MaxDepth;

uniform half4 _FoamColor;
uniform half _FoamDistance;
uniform half _FoamNoiseScaleHQ;
uniform half _FoamNoiseSpeedHQ;
uniform half _ShorelineFoamStrength;
uniform half _CrestFoamStrength;
uniform half _CrestMaxDepth;
#if defined(POSEIDON_RIVER)
uniform half _SlopeFoamStrength;
uniform half _SlopeFoamFlowSpeed;
uniform half _SlopeFoamDistance;
#endif

uniform float _RippleHeight;
uniform float _RippleNoiseScale;
uniform float _RippleSpeed;

uniform float2 _WaveDirection;
uniform half _WaveSpeed;
uniform half _WaveHeight;
uniform half _WaveLength;
uniform half _WaveSteepness;
uniform half _WaveDeform;
#if WAVE_MASK
uniform sampler2D _WaveMask;
uniform float4 _WaveMaskBounds;
#endif

uniform half _FresnelStrength;
uniform half _FresnelBias;

#if defined(POSEIDON_WATER_ADVANCED)
	uniform sampler2D _ReflectionTex;
	uniform half4 _ReflectionTex_TexelSize;
	uniform half _ReflectionDistortionStrength;
#endif
		
#if defined(POSEIDON_WATER_ADVANCED) || defined(POSEIDON_RIVER)
	uniform sampler2D _RefractionTex;
	uniform half4 _RefractionTex_TexelSize;
	uniform half _RefractionDistortionStrength;

	uniform sampler2D _CausticTex;
	uniform half _CausticSize;
	uniform half _CausticStrength;
	uniform half _CausticDistortionStrength;
#endif

uniform half _AuraLightingFactor;

#endif
