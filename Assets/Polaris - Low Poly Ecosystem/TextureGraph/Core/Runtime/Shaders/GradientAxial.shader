Shader "Hidden/TextureGraph/GradientAxial"
{
	Properties
	{
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature_local REFLECTED
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

			float4x4 _UvToLineMatrix;

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
				float2 lineUV = mul(_UvToLineMatrix, float4(uv.xy, 0, 1));
				float x = saturate(lineUV.x);
#if REFLECTED
				if (x < 0.5)
				{
					x = InverseLerpUnclamped(0, 0.5, x);
				}
				else 
				{
					x = InverseLerpUnclamped(1, 0.5, x);
				}
#endif
				float f = x;
				return float4(f, f, f, 1);
			}
			ENDCG
		}
	}
}
