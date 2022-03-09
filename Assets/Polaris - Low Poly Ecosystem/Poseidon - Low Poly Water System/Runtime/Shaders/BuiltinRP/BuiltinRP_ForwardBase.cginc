#ifndef BUILTIN_RP_FORWARD_BASE_INCLUDED
#define BUILTIN_RP_FORWARD_BASE_INCLUDED

#define _ALPHABLEND_ON 1

#include "HLSLSupport.cginc"
#include "UnityShaderVariables.cginc"
#include "UnityShaderUtilities.cginc"
#include "UnityCG.cginc"
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
    #if UNITY_SHOULD_SAMPLE_SH
        fixed3 lightSH : TEXCOORD6;
    #endif
    UNITY_LIGHTING_COORDS(7,8)
    UNITY_FOG_COORDS(9)
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
    #if AURA_LIGHTING || AURA_FOG
        float3 positionAura : TEXCOORD10;
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

    #if FLAT_LIGHTING
        float3 positionLS = o.positionLS;
    #else
        float3 positionLS = o.positionWS;
    #endif

    //Vertex lights and SH
    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
        o.lightSH = 0;
    #ifdef VERTEXLIGHT_ON
        o.lightSH += Shade4PointLights(
            unity_4LightPosX0, unity_4LightPosY0, unity_4LightPosZ0,
            unity_LightColor[0].rgb, unity_LightColor[1].rgb, unity_LightColor[2].rgb, unity_LightColor[3].rgb,
            unity_4LightAtten0, positionLS, o.normalWS);
    #endif
        o.lightSH = ShadeSHPerVertex(o.normalWS, o.lightSH);
    #endif

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
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
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
    gi.light.color = _LightColor0.rgb;
    gi.light.dir = lightDirection;

    UnityGIInput giInput;
    UNITY_INITIALIZE_OUTPUT(UnityGIInput, giInput);
    giInput.light = gi.light;
    giInput.worldPos = positionLS;
    giInput.worldViewDir = viewDirectionWS;
    giInput.atten = lightAtten;
    giInput.lightmapUV = 0;

    #if UNITY_SHOULD_SAMPLE_SH && !UNITY_SAMPLE_FULL_SH_PER_PIXEL
        giInput.ambient = i.lightSH;
    #else
        giInput.ambient = 0;
    #endif
    giInput.probeHDR[0] = unity_SpecCube0_HDR;
    giInput.probeHDR[1] = unity_SpecCube1_HDR;
    #if defined(UNITY_SPECCUBE_BLENDING) || defined(UNITY_SPECCUBE_BOX_PROJECTION)
        giInput.boxMin[0] = unity_SpecCube0_BoxMin;
    #endif
    #ifdef UNITY_SPECCUBE_BOX_PROJECTION
        giInput.boxMax[0] = unity_SpecCube0_BoxMax;
        giInput.probePosition[0] = unity_SpecCube0_ProbePosition;
        giInput.boxMin[1] = unity_SpecCube1_BoxMin;
        giInput.boxMax[1] = unity_SpecCube1_BoxMax;
        giInput.probePosition[1] = unity_SpecCube1_ProbePosition;
    #endif

    #if LIGHTING_LAMBERT
        LightingLambert_GI(surfOut, giInput, gi);
        finalColor += LightingLambert(surfOut, gi);
    #elif LIGHTING_BLINN_PHONG
        LightingBlinnPhong_GI(surfOut, giInput, gi);
        finalColor += LightingBlinnPhong(surfOut, viewDirectionWS, gi);
    #else
        LightingStandardSpecular_GI(surfOut, giInput, gi);
        finalColor += LightingStandardSpecular(surfOut, viewDirectionWS, gi);
    #endif

    UNITY_APPLY_FOG(i.fogCoord, finalColor);

    #if AURA_FOG
        Aura2_ApplyFog(finalColor, i.positionAura);
    #endif

    return finalColor;
}
#endif
