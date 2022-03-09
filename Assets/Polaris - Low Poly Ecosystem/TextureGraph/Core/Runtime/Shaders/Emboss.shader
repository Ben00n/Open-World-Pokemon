Shader "Hidden/TextureGraph/Emboss"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_HeightMap("Height Map", 2D) = "black"{}
		_Intensity("Intensity", Float) = 0.1
		_LightDirection("Light Direction", Vector) = (0,0,0,0)
		_HighlightColor("Highlight Color", Color) = (1,1,1,1)
		_ShadowColor("Shadow Color", Color) = (0,0,0,1)
	}
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma shader_feature_local RRR
				#include "./CGIncludes/EffectCommon.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : SV_POSITION;
					float2 uv : TEXCOORD0;
				};

				sampler2D _MainTex;
				sampler2D _HeightMap;
				float4 _HeightMap_TexelSize;
				float _Intensity;
				float4 _LightDirection;
				float4 _HighlightColor;
				float4 _ShadowColor;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				float4 frag(v2f i) : SV_Target
				{
					float3 normal = ExtractNormal(_HeightMap, _HeightMap_TexelSize, i.uv).xyz;
					float nDotL = dot(normal, _LightDirection.xyz);

					float4 color = tex2D(_MainTex, i.uv);
	#if RRR
					color = float4(color.rrr, 1);
	#endif

					float4 tint = _HighlightColor * (nDotL > 0) + _ShadowColor * (nDotL <= 0);
					float4 result = lerp(color, tint, abs(nDotL)*_Intensity);
					result.a = color.a;

					return result;
				}
				ENDCG
			}
		}
}
