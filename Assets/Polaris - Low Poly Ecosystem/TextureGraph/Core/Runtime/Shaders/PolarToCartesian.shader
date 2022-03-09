Shader "Hidden/TextureGraph/PolarToCartesian"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local RRR
			#include "./CGIncludes/MathCommon.cginc"

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float radius = uv.x;
				float rad = uv.y;
				rad = rad * 2 - 1;
				rad = rad * PI;

				float x = radius * cos(rad);
				float y = radius * sin(rad);
				uv = float2(x, y);
				uv = uv * 0.5 + 0.5;

				float4 color = tex2D(_MainTex, uv);
#if RRR
				color = float4(color.rrr, 1);
#endif

				return color;
			}
			ENDCG
		}
	}
}
