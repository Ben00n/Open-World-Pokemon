Shader "Jupiter/SkyShadow"
{
    Properties
    {
		[HideInInspector] _OverheadCloudColor("Overhead Cloud Color", Color) = (1, 1, 1, 0.5)
		[HideInInspector] _OverheadCloudAltitude("Overhead Cloud Altitude", Float) = 1000
		[HideInInspector] _OverheadCloudSize("Overhead Cloud Size", Float) = 100
		[HideInInspector] _OverheadCloudStep("Overhead Cloud Step", Float) = 25
		[HideInInspector] _OverheadCloudAnimationSpeed("Overhead Cloud Animation Speed", Float) = 1
		[HideInInspector] _OverheadCloudFlowDirectionX("Overhead Cloud Flow X", Float) = 1
		[HideInInspector] _OverheadCloudFlowDirectionZ("Overhead Cloud Flow X", Float) = 1
		[HideInInspector] _OverheadCloudRemapMin("Overhead Cloud Remap Min", Float) = 0
		[HideInInspector] _OverheadCloudRemapMax("Overhead Cloud Remap Max", Float) = 1
		[HideInInspector] _OverheadCloudShadowClipMask("Overhead Cloud Shadow Clip Mask", Float) = 0.1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100
		
		Pass
		{
			Name "ShadowCaster"
			Tags { "LightMode" = "ShadowCaster" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile_instancing // allow instanced shadow pass for most of the shaders
			#include "UnityCG.cginc"

			#pragma shader_feature_local ALLOW_STEP_EFFECT

			#include "./CGIncludes/JCommon.cginc"
			#include "./CGIncludes/JOverheadCloud.cginc"

			struct v2f {
				V2F_SHADOW_CASTER;
				UNITY_VERTEX_OUTPUT_STEREO
				float3 localPos : TEXCOORD1;
			};

			uniform fixed4 _OverheadCloudColor;
			uniform fixed _OverheadCloudAltitude;
			uniform fixed _OverheadCloudSize;
			uniform fixed _OverheadCloudStep;
			uniform fixed _OverheadCloudAnimationSpeed;
			uniform fixed _OverheadCloudFlowDirectionX;
			uniform fixed _OverheadCloudFlowDirectionZ; 
			uniform fixed _OverheadCloudRemapMin;
			uniform fixed _OverheadCloudRemapMax;
			uniform fixed _OverheadCloudShadowClipMask;

			v2f vert(appdata_base v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
					o.localPos = v.vertex;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{ 
				float4 localPos = float4(i.localPos.xyz, 1);			
				float4 normalizedLocalPos = float4(normalize(localPos.xyz), 1);
				fixed4 overheadCloudColor;
				CalculateOverheadCloudColor(
					normalizedLocalPos,
					_OverheadCloudColor,
					_OverheadCloudAltitude,
					_OverheadCloudSize, _OverheadCloudStep,
					_OverheadCloudAnimationSpeed,
					_OverheadCloudFlowDirectionX, _OverheadCloudFlowDirectionZ,
					_OverheadCloudRemapMin, _OverheadCloudRemapMax,
					overheadCloudColor);
				clip(overheadCloudColor.a - _OverheadCloudShadowClipMask);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}
    }
}
