Shader "Hidden/TextureGraph/Sharpen"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_Intensity("Intensity", Float) = 1
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local RRR

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
			float4 _MainTex_TexelSize;
			float _Intensity;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float iSqrt2 = 1.0 / sqrt(2);
				float2 texel = _MainTex_TexelSize.xy;

				float2 uv = i.uv;
				float2 uvLeft = uv + float2(-texel.x, 0);
				float2 uvLeftTop = uv + float2(-texel.x * iSqrt2, texel.y * iSqrt2);
				float2 uvTop = uv + float2(0, texel.y);
				float2 uvTopRight = uv + float2(texel.x * iSqrt2, texel.y * iSqrt2);
				float2 uvRight = uv + float2(texel.x, 0);
				float2 uvRightBottom = uv + float2(texel.x * iSqrt2, -texel.y * iSqrt2);
				float2 uvBottom = uv + float2(0, -texel.y);
				float2 uvBottomLeft = uv + float2(-texel.x * iSqrt2, -texel.y * iSqrt2);

				float4 center = tex2D(_MainTex, uv);
				float4 left = tex2D(_MainTex, uvLeft);
				float4 leftTop = tex2D(_MainTex, uvLeftTop);
				float4 top = tex2D(_MainTex, uvTop);
				float4 topRight = tex2D(_MainTex, uvTopRight);
				float4 right = tex2D(_MainTex, uvRight);
				float4 rightBottom = tex2D(_MainTex, uvRightBottom);
				float4 bottom = tex2D(_MainTex, uvBottom);
				float4 bottomLeft = tex2D(_MainTex, uvBottomLeft);

				float4 color = center + (center * 8 - left - top - right - bottom - leftTop - topRight - rightBottom - bottomLeft) * _Intensity;
				color.a = center.a;
#if RRR
				color = float4(color.rrr, 1);
#endif
				return color;
			}
			ENDCG
		}
	}
}
