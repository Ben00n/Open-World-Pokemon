#ifndef UNIVERSAL_RP_FORWARD_INCLUDED
#define UNIVERSAL_RP_FORWARD_INCLUDED

#include "UniversalRP_Lighting.cginc"

struct Attributes
{
    float4 positionOS: POSITION;
    float4 texcoord: TEXCOORD0;
    float4 color : COLOR;
#if defined(POSEIDON_RIVER)
    float4 texcoord1 : TEXCOORD1;
#endif
#if UNITY_ANY_INSTANCING_ENABLED
    uint instanceID: INSTANCEID_SEMANTIC;
#endif
};

struct Varyings
{
    float4 positionCS : SV_POSITION;
    float3 positionWS : TEXCOORD0;
#if FLAT_LIGHTING
    float3 positionLS : TEXCOORD1;
#endif
    float3 normalWS : NORMAL;
    float3 viewDirectionWS : TEXCOORD2;
    float4 positionSS : TEXCOORD3;
    float3 sh : TEXCOORD4;
    float4 fogFactorAndVertexLight : TEXCOORD5;
    float4 shadowCoord : TEXCOORD6;
#if WAVE
    float crestMask : TEXCOORD7;
#endif
#if UNITY_ANY_INSTANCING_ENABLED
    uint instanceID: CUSTOM_INSTANCE_ID;
#endif
#if (defined(UNITY_STEREO_INSTANCING_ENABLED))
    uint stereoTargetEyeIndexAsRTArrayIdx: SV_RenderTargetArrayIndex;
#endif
#if(defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
    uint stereoTargetEyeIndexAsBlendIdx0: BLENDINDICES0;
#endif
};

Varyings vert(Attributes v)
{
    Varyings o = (Varyings)0;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

#if MESH_NOISE && !defined(POSEIDON_RIVER)
    ApplyMeshNoise(v.positionOS, v.texcoord, v.color);
#endif
#if WAVE
    ApplyWaveHQ(v.positionOS, v.texcoord, v.color, o.crestMask);
#endif
#if defined(POSEIDON_RIVER)
    ApplyRipple(v.positionOS, v.texcoord, v.color, v.texcoord1);
#else
    ApplyRipple(v.positionOS, v.texcoord, v.color);
#endif
    float3 normalOS;
    CalculateNormal(v.positionOS, v.texcoord, v.color, normalOS);

    o.positionWS = TransformObjectToWorld(v.positionOS.xyz);
#if FLAT_LIGHTING
    float3 centerVertex = (v.positionOS.xyz + v.texcoord.xyz + v.color.xyz) / 3.0;
    o.positionLS = TransformObjectToWorld(centerVertex);
#endif
    o.normalWS = TransformObjectToWorldNormal(normalOS);
    o.positionSS = ComputeScreenPos(o.positionCS); 
    o.positionCS = TransformWorldToHClip(o.positionWS);

#if FLAT_LIGHTING
    float3 positionLS = o.positionLS;
#else
    float3 positionLS = o.positionWS;
#endif

    o.viewDirectionWS = _WorldSpaceCameraPos.xyz - positionLS;
    o.positionSS = ComputeScreenPos(o.positionCS, _ProjectionParams.x);

    OUTPUT_SH(o.normalWS, o.sh);

    half3 vertexLight = VertexLighting(positionLS, o.normalWS);
    half fogFactor = ComputeFogFactor(o.positionCS.z);
    o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

    VertexPositionInputs vertexInput = GetVertexPositionInputs(v.positionOS.xyz);
    o.shadowCoord = GetShadowCoord(vertexInput);
    return o;
}

void BuildInputData(Varyings input, out InputData inputData)
{
    inputData = (InputData)0;
#if FLAT_LIGHTING
    inputData.positionWS = input.positionLS;
#else
    inputData.positionWS = input.positionWS;
#endif

    inputData.normalWS = NormalizeNormalPerPixel(input.normalWS);
    inputData.viewDirectionWS = SafeNormalize(input.viewDirectionWS);

#if defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
#else
    inputData.shadowCoord = float4(0, 0, 0, 0);
#endif
    inputData.fogCoord = input.fogFactorAndVertexLight.x;
    inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
    inputData.bakedGI = SAMPLE_GI(0, input.sh, inputData.normalWS);
}

half4 frag(Varyings i) : SV_TARGET
{
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    
    SurfaceInput surfIn = (SurfaceInput)0;
    surfIn.positionWS = i.positionWS;
    surfIn.positionSS = i.positionSS;
    surfIn.normalWS = i.normalWS;
#if WAVE
    surfIn.crestMask = i.crestMask;
#endif

    SurfaceOutput surfOut = (SurfaceOutput)0;
    surfOut.Albedo = 0;
    surfOut.Specular = 0;
    surfOut.Smoothness = 0;
    surfOut.Alpha = 1;
    SURFACE_FUNCTION(surfIn, surfOut);

    InputData inputData;
    BuildInputData(i, inputData);

    half4 color;
#if LIGHTING_BLINN_PHONG
    color = PoseidonFragmentBlinnPhong(
        inputData,
        surfOut.Albedo,
        half4(surfOut.Specular, 1),
        surfOut.Smoothness,
        surfOut.Alpha);
#elif LIGHTING_LAMBERT
    color = PoseidonFragmentLambert(
        inputData,
        surfOut.Albedo,
        surfOut.Alpha);
#else
    color = PoseidonFragmentPBR(
        inputData,
        surfOut.Albedo,
        surfOut.Specular,
        surfOut.Smoothness,
        surfOut.Alpha);
#endif

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    return color;
}

#endif
