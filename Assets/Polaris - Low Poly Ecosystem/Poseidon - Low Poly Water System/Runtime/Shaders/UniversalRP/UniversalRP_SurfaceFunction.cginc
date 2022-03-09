#ifndef UNIVERSAL_RP_SURFACE_FUNCTION_INCLUDED
#define UNIVERSAL_RP_SURFACE_FUNCTION_INCLUDED

struct SurfaceInput
{
    float3 positionWS;
    float4 positionSS;
    float3 normalWS;
    float crestMask;
};

struct SurfaceOutput
{
    float3 Albedo;
    float3 Specular;
    float Smoothness;
    float Alpha;
};

#if defined(POSEIDON_WATER_ADVANCED)
    #if defined (POSEIDON_BACK_FACE)
        #define SURFACE_FUNCTION(i, o) SurfBackFace(i, o);
    #else
        #define SURFACE_FUNCTION(i, o) SurfAdvanced(i, o);
    #endif
#elif defined(POSEIDON_RIVER)
    #define SURFACE_FUNCTION(i, o) SurfRiver(i, o);
#else
    #define SURFACE_FUNCTION(i, o) SurfBasic(i, o);
#endif

#ifndef POSEIDON_WATER_ADVANCED
void SurfBasic(SurfaceInput i, inout SurfaceOutput o)
{
    half fresnel;
    CalculateFresnelFactor(i.positionWS, i.normalWS, fresnel);

    #if LIGHT_ABSORPTION || FOAM
        float sceneDepth = GetSceneDepth(i.positionSS);
        float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
    #endif

    half4 tintColor = _Color;
    #if LIGHT_ABSORPTION
        CalculateDeepWaterColor(sceneDepth, surfaceDepth, tintColor);
    #endif

    half4 waterColor = lerp(_Color, tintColor, fresnel);
    waterColor = saturate(waterColor);

    half4 foamColor = float4(0, 0, 0, 0);
    #if FOAM
        #if FOAM_HQ
            CalculateFoamColorHQ(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #else
            CalculateFoamColor(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #endif
    #endif 

    half3 Albedo = lerp(waterColor.rgb, foamColor.rgb * 1.5, foamColor.a);
    half3 Specular = _Specular.rgb;
    half Smoothness = saturate(_Smoothness - foamColor.a);
    half Alpha = lerp(waterColor.a, foamColor.a, foamColor.a);

    o.Albedo = IsGammaSpace() ? Albedo : SRGBToLinear(Albedo);
    o.Specular = IsGammaSpace() ? Specular : SRGBToLinear(Specular);
    o.Smoothness = Smoothness;
    o.Alpha = Alpha;
}
#endif //!POSEIDON_WATER_ADVANCED

#if defined(POSEIDON_WATER_ADVANCED)
void SurfAdvanced(SurfaceInput i, inout SurfaceOutput o)
{
    half fresnel;
    CalculateFresnelFactor(i.positionWS, i.normalWS, fresnel);

    #if LIGHT_ABSORPTION || FOAM || CAUSTIC
        float sceneDepth = GetSceneDepth(i.positionSS);
        float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
        #if LIGHT_ABSORPTION
			float depthFade = GetDepthFade(sceneDepth, surfaceDepth, _MaxDepth);
		#endif
    #endif

    half4 waterColor;
    half4 tintColor = _Color;
    #if LIGHT_ABSORPTION
        CalculateDeepWaterColor(sceneDepth, surfaceDepth, tintColor);
    #endif

    half4 reflColor = _Color;
    #if REFLECTION && !UNITY_SINGLE_PASS_STEREO && !STEREO_INSTANCING_ON && !UNITY_STEREO_MULTIVIEW_ENABLED
        SampleReflectionTexture(i.positionSS, i.normalWS, reflColor);
    #endif

    half4 refrColor = _DepthColor;
    #if REFRACTION
        SampleRefractionTexture(i.positionSS, i.normalWS, refrColor);
        #if LIGHT_ABSORPTION
		    refrColor = lerp(_DepthColor, refrColor, depthFade);
		#endif
    #endif

    half4 causticColor = half4(0, 0, 0, 0);
    #if CAUSTIC
        SampleCausticTexture(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, causticColor);
        #if LIGHT_ABSORPTION
			causticColor *= depthFade;
		#endif
    #endif
    refrColor += causticColor;

    waterColor = tintColor * lerp(refrColor, reflColor, fresnel);
    waterColor = waterColor * tintColor.a + (1 - tintColor.a) * refrColor;
    waterColor = saturate(waterColor);

    half4 foamColor = float4(0, 0, 0, 0);
    #if FOAM
        #if FOAM_HQ
            CalculateFoamColorHQ(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #else
            CalculateFoamColor(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #endif
    #endif 

    half3 Albedo = lerp(waterColor.rgb, foamColor.rgb * 1.5, foamColor.a);
    half3 Specular = _Specular.rgb;
    half Smoothness = saturate(_Smoothness - foamColor.a);

    o.Albedo = IsGammaSpace() ? Albedo : SRGBToLinear(Albedo);
    o.Specular = IsGammaSpace() ? Specular : SRGBToLinear(Specular);
    o.Smoothness = Smoothness;
    o.Alpha = 1;
}

void SurfBackFace(SurfaceInput i, inout SurfaceOutput o)
{
    half fresnel;
    CalculateFresnelFactor(i.positionWS, -i.normalWS, fresnel);

    #if FOAM 
        float sceneDepth = GetSceneDepth(i.positionSS);
        float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
    #endif

    half4 waterColor;
    half4 tintColor = _Color;
    half4 refrColor = _Color;
    SampleRefractionTexture(i.positionSS, i.normalWS, refrColor);

    waterColor = lerp(refrColor, _Color, fresnel);
    waterColor = waterColor * tintColor.a + (1 - tintColor.a) * refrColor;
    waterColor = saturate(waterColor);

    half4 foamColor = float4(0, 0, 0, 0);
    #if FOAM
        #if FOAM_HQ
            CalculateFoamColorHQ(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #else
            CalculateFoamColor(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #endif
            foamColor.a *= 0.5;
    #endif 

    half3 Albedo = lerp(waterColor.rgb, foamColor.rgb * 1.5, foamColor.a);
    half3 Specular = _Specular.rgb;
    half Smoothness = saturate(_Smoothness - foamColor.a);

    o.Albedo = IsGammaSpace() ? Albedo : SRGBToLinear(Albedo);
    o.Specular = IsGammaSpace() ? Specular : SRGBToLinear(Specular);
    o.Smoothness = Smoothness;
    o.Alpha = 1;
}

#endif //POSEIDON_WATER_ADVANCED

#if defined(POSEIDON_RIVER)
void SurfRiver(SurfaceInput i, inout SurfaceOutput o)
{
    half fresnel;
    CalculateFresnelFactor(i.positionWS, i.normalWS, fresnel);

    #if LIGHT_ABSORPTION || FOAM || CAUSTIC
        float sceneDepth = GetSceneDepth(i.positionSS);
        float surfaceDepth = GetSurfaceDepth(float4(i.positionWS, 1));
        #if LIGHT_ABSORPTION
            float depthFade = GetDepthFade(sceneDepth, surfaceDepth, _MaxDepth);
        #endif
    #endif

    half4 waterColor;
    half4 tintColor = _Color;
    #if LIGHT_ABSORPTION
        CalculateDeepWaterColor(sceneDepth, surfaceDepth, tintColor);
    #endif

    half4 refrColor = _DepthColor;
    SampleRefractionTexture(i.positionSS, i.normalWS, refrColor);
    #if LIGHT_ABSORPTION
		refrColor = lerp(_DepthColor, refrColor, depthFade);
	#endif

    half4 causticColor = half4(0, 0, 0, 0);
    #if CAUSTIC
        SampleCausticTexture(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, causticColor);
        #if LIGHT_ABSORPTION
			causticColor *= depthFade;
		#endif
    #endif
    refrColor += causticColor;

    waterColor = lerp(refrColor, tintColor, tintColor.a * fresnel);

    half4 foamColor = float4(0, 0, 0, 0);
    #if FOAM
        #if FOAM_HQ
            CalculateFoamColorHQ(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #else
            CalculateFoamColor(sceneDepth, surfaceDepth, i.positionWS, i.normalWS, i.crestMask, foamColor);
        #endif
    #endif 

    half3 Albedo = lerp(waterColor.rgb, foamColor.rgb * 1.5, foamColor.a);
    half3 Specular = _Specular.rgb;
    half Smoothness = saturate(_Smoothness - foamColor.a);

    o.Albedo = IsGammaSpace() ? Albedo : SRGBToLinear(Albedo);
    o.Specular = IsGammaSpace() ? Specular : SRGBToLinear(Specular);
    o.Smoothness = Smoothness;
    o.Alpha = 1;
}
#endif //POSEIDON_RIVER

#endif //UNIVERSAL_RP_SURFACE_FUNCTION_INCLUDED
