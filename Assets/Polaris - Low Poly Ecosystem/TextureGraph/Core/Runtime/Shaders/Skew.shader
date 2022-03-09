﻿Shader "Hidden/TextureGraph/Skew"
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
			float4x4 _SkewMatrix;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = mul(_SkewMatrix, float4(v.uv, 0, 1)).xy;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv); 
#if RRR
				color = float4(color.rrr, 1);
#endif
				return color;
			}
			ENDCG
		}
	}
}
