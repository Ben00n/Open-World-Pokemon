Shader "Hidden/TextureGraph/Mirror"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_OffsetX("Offset X", Float) = 0.5
		_OffsetY("Offset Y", Float) = 0.5
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local MIRROR_X
			#pragma shader_feature_local FLIP_X
			#pragma shader_feature_local MIRROR_Y
			#pragma shader_feature_local FLIP_Y
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
			float _OffsetX;
			float _OffsetY;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float Mirror(float value, float axis)
			{
				return axis * 2 - value;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
#if MIRROR_X
#if FLIP_X
				float mirrorXMul = uv.x < _OffsetX;
#else
				float mirrorXMul = uv.x > _OffsetX;
#endif //FLIP_X
				uv.x = Mirror(uv.x, _OffsetX) * mirrorXMul + uv.x * (1 - mirrorXMul);
#endif //MIRROR_X

#if MIRROR_Y
#if FLIP_Y
				float mirrorYMul = uv.y < _OffsetY;
#else
				float mirrorYMul = uv.y > _OffsetY;
#endif //FLIP_Y
				uv.y = Mirror(uv.y, _OffsetY) * mirrorYMul + uv.y * (1 - mirrorYMul);
#endif //MIRROR_Y

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
