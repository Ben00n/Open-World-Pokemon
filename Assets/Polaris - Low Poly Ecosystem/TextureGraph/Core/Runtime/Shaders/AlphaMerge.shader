Shader "Hidden/TextureGraph/AlphaMerge"
{
	Properties
	{
		_RgbTex ("RGB", 2D) = "black"{}
		_AlphaTex("Alpha", 2D) = "white"{}
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

			sampler2D _RgbTex;
			sampler2D _AlphaTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float3 rgb = tex2D(_RgbTex, i.uv).rgb;
#if RRR 
				rgb = float4(rgb.rrr, 1);
#endif
				float a = tex2D(_AlphaTex, i.uv).r;
				float4 color = float4(rgb, a);
				return color;
			}
			ENDCG
		}
	}
}
