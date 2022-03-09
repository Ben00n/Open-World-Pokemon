Shader "Hidden/TextureGraph/HSL"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_Hue("Hue", Float) = 0
		_Saturation("Saturation", Float) = 0
		_Lightness("Lightness", Float) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local RRR
			#include "./CGIncludes/ColorConversion.cginc"

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
			float _Hue;
			float _Saturation;
			float _Lightness;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif

				float3 hsl = RGBtoHSL(saturate(color.rgb));
				hsl.x = frac(hsl.x + _Hue);
				hsl.y = saturate(hsl.y + _Saturation);
				hsl.z = saturate(hsl.z + _Lightness);
				color.rgb = HSLtoRGB(hsl);

				return color;
			}
			ENDCG
		}
	}
}
