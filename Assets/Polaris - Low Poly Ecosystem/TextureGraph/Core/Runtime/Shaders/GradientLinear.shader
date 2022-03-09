Shader "Hidden/TextureGraph/GradientLinear"
{
	Properties
	{
		_MidPoint("Mid Point", Float) = 1
	}
		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
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

			float4x4 _UvToGradientMatrix;
			float _MidPoint;

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
				float2 gradientUV = mul(_UvToGradientMatrix, float4(uv.xy, 0, 1));
				float x = frac(gradientUV.x);
				if (x <= _MidPoint)
				{
					x = InverseLerpUnclamped(0, _MidPoint, x);
				}
				else
				{
					x = InverseLerpUnclamped(1, _MidPoint, x);
				}

				float f = x;
				return float4(f,f,f,1);
			}
			ENDCG
		}
	}
}
