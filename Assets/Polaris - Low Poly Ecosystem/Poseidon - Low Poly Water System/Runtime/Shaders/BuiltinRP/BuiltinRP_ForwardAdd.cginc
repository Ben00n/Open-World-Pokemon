#ifndef BUILTIN_RP_FORWARD_ADD_INCLUDED
#define BUILTIN_RP_FORWARD_ADD_INCLUDED

#define _ALPHABLEND_ON 1

#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"
#include "Lighting.cginc"
#include "UnityPBSLighting.cginc"
#include "AutoLight.cginc"

#if AURA_LIGHTING || AURA_FOG
    #include "Assets/Aura 2/Core/Code/Shaders/Aura.cginc"
#endif

struct Varyings
{
    UNITY_POSITION(pos);
    float3 positionWS : TEXCOORD1;
    #if FLAT_LIGHTING
        float3 positionLS : TEXCOORD2;
    #endif
        float4 positionSS : TEXCOORD3;
        float3 normalWS : TEXCOORD4;
    #if WAVE
        float crestMask : TEXCOORD5;
    #endif
    UNITY_LIGHTING_COORDS(6, 7)
    UNITY_FOG_COORDS(8)
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
    
    #if AURA_LIGHTING || AURA_FOG
        float3 positionAura : TEXCOORD9;
    #endif
};

Varyings vert(appdata_full v)
{
    UNITY_SETUP_INSTANCE_ID(v);
    Varyings o;
    UNITY_INITIALIZE_OUTPUT(Varyings, o);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    #if MESH_NOISE && !defined(POSEIDON_RIVER)
        ApplyMeshNoise(v.vertex, v.texcoord, v.color);
    #endif
    #if WAVE
        ApplyWaveHQ(v.vertex, v.texcoord, v.color, o.crestMask);
    #endif
    #if defined(POSEIDON_RIVER)
        ApplyRipple(v.vertex, v.texcoord, v.color, v.texcoord1);
    #else
        ApplyRipple(v.vertex, v.texcoord, v.color);
    #endif
    CalculateNormal(v.vertex, v.texcoord, v.color, v.normal);

    o.pos = UnityObjectToClipPos(v.vertex);

    o.positionWS = mul(unity_ObjectToWorld, v.vertex).xyz;
    #if FLAT_LIGHTING
        float3 centerVertex = (v.vertex.xyz + v.texcoord.xyz + v.color.xyz) / 3.0;
        o.positionLS = mul(unity_ObjectToWorld, float4(centerVertex, 1)).xyz;
    #endif
    o.normalWS = UnityObjectToWorldNormal(v.normal);
    o.positionSS = ComputeScreenPos(o.pos);

    UNITY_TRANSFER_LIGHTING(o, i.texcoord1.xy);
    UNITY_TRANSFER_FOG(o, o.pos);

    
    #if AURA_LIGHTING || AURA_FOG
        o.positionAura = Aura2_GetFrustumSpaceCoordinates(v.vertex);
    #endif
    return o;
}

fixed4 frag(Varyings i) : SV_Target
{
    UNITY_SETUP_INSTANCE_ID(i);
    //Unpack data
    float3 positionWS = i.positionWS;
#if FLAT_LIGHTING
    float3 positionLS = i.positionLS;
#else
    float3 positionLS = i.positionWS;
#endif
    float4 positionSS = i.positionSS;
    float3 normalWS = i.normalWS;

    //Prepare and call SurfaceFunction
    SurfaceInput surfIn;
    UNITY_INITIALIZE_OUTPUT(SurfaceInput, surfIn);
    surfIn.positionWS = positionWS;
    surfIn.positionSS = positionSS;
    surfIn.normalWS = normalWS;
    #if WAVE
        surfIn.crestMask = i.crestMask;
    #endif

    #if LIGHTING_LAMBERT || LIGHTING_BLINN_PHONG
        SurfaceOutput surfOut = (SurfaceOutput)0;
        surfOut.Gloss = 0.5;
        surfOut.Specular = 0;
    #else 
        SurfaceOutputStandardSpecular surfOut = (SurfaceOutputStandardSpecular)0;
        surfOut.Smoothness = 0;
        surfOut.Specular = 0;
        surfOut.Occlusion = 1;
    #endif
    surfOut.Albedo = 0;
    surfOut.Emission = 0;
    surfOut.Alpha = 0;
    surfOut.Normal = normalWS;
    SURFACE_FUNCTION(surfIn, surfOut);
        
    #if AURA_LIGHTING
        Aura2_ApplyLighting(surfOut.Albedo, i.positionAura, _AuraLightingFactor);
    #endif

    //Calculate lighting
    fixed4 finalColor = 0;

    //Compute lighting & shadowing factor
    UNITY_LIGHT_ATTENUATION(lightAtten, i, positionLS);

    //Setup lighting environment
    #ifndef USING_DIRECTIONAL_LIGHT
        fixed3 lightDirection = normalize(UnityWorldSpaceLightDir(positionLS));
    #else
        fixed3 lightDirection = _WorldSpaceLightPos0.xyz;
    #endif

    float3 viewDirectionWS = normalize(UnityWorldSpaceViewDir(positionLS));

    UnityGI gi;
    UNITY_INITIALIZE_OUTPUT(UnityGI, gi);
    gi.indirect.diffuse = 0;
    gi.indirect.specular = 0;
    gi.light.color = _LightColor0;
    gi.light.dir = lightDirection;
    gi.light.color *= lightAtten;

    #if LIGHTING_LAMBERT
        finalColor += LightingLambert(surfOut, gi);
    #elif LIGHTING_BLINN_PHONG
        finalColor += LightingBlinnPhong(surfOut, viewDirectionWS, gi);
    #else
        finalColor += LightingStandardSpecular(surfOut, viewDirectionWS, gi);
    #endif

    UNITY_APPLY_FOG(i.fogCoord, finalColor);
    
    #if AURA_FOG
        Aura2_ApplyFog(finalColor, i.positionAura);
    #endif
    return finalColor;
}
#endif
