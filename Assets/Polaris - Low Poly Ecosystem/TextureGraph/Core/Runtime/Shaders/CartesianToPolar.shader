Shader "Hidden/TextureGraph/CartesianToPolar"
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
				float2 uv = i.uv * 2 - 1;
				float radius = sqrt(uv.x * uv.x + uv.y * uv.y);
				float rad = atan2(uv.y, uv.x);
				rad = rad / PI;
				rad = rad * 0.5 + 0.5;
				uv = float2(radius, rad);
				uv = frac(uv);

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
