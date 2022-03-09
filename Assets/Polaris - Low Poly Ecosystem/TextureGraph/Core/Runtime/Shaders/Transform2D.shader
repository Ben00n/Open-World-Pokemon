Shader "Hidden/TextureGraph/Transform2D"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_BackgroundColor("Background Color", Color) = (0,0,0,1)
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
#pragma fragment frag
#pragma shader_feature_local RRR
#pragma shader_feature_local TILE_X
#pragma shader_feature_local TILE_Y

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
			float4x4 _TransformMatrix;
			float4 _BackgroundColor;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = mul(_TransformMatrix, float4(v.uv.xy, 0, 1));
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float2 isInRange01 = float2(1, 1);
#if TILE_X
				uv.x = frac(uv.x);
#else
				isInRange01.x = (uv.x >= 0) * (uv.x <= 1);
#endif //TILE_X

#if TILE_Y
				uv.y = frac(uv.y);
#else
				isInRange01.y = (uv.y >= 0) * (uv.y <= 1);
#endif //TILE_Y

				float4 color = tex2D(_MainTex, uv);
#if RRR
				color = float4(color.rrr, 1);
#endif //RRR

				float bgColorMask = 1 - isInRange01.x * isInRange01.y;
				color = color * (1 - bgColorMask) + _BackgroundColor * bgColorMask;

				return color;
			}
			ENDCG
		}
	}
}
