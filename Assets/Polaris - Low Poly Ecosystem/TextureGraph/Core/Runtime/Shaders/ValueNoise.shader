Shader "Hidden/TextureGraph/ValueNoise"
{
	Properties
	{
		_Scale("Scale", Float) = 1
		_Seed("Seed", Float) = 0
		_Variant("Variant", Float) = 0
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

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

			float _Scale;
			float _Seed;
			float _Variant;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float ValueNoiseInterpolate(float a, float b, float t)
			{
				return (1.0 - t) * a + (t * b);
			}

			float ValueNoise(float2 uv)
			{
				float2 i = floor(uv);
				float2 f = frac(uv);
				f = f * f * (3.0 - 2.0 * f);

				uv = abs(frac(uv) - 0.5);
				float2 c0 = i + float2(0.0, 0.0);
				float2 c1 = i + float2(1.0, 0.0);
				float2 c2 = i + float2(0.0, 1.0);
				float2 c3 = i + float2(1.0, 1.0);


				float2 offset = floor(RandomValue(_Seed) * 123);
				c0 = c0 % _Scale + offset;
				c1 = c1 % _Scale + offset;
				c2 = c2 % _Scale + offset;
				c3 = c3 % _Scale + offset;

				float r0 = RandomValue(c0.x, c0.y);
				float r1 = RandomValue(c1.x, c1.y);
				float r2 = RandomValue(c2.x, c2.y);
				float r3 = RandomValue(c3.x, c3.y);

				float bottomOfGrid = ValueNoiseInterpolate(r0, r1, f.x);
				float topOfGrid = ValueNoiseInterpolate(r2, r3, f.x);
				float t = ValueNoiseInterpolate(bottomOfGrid, topOfGrid, f.y);
				return t;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv * _Scale;
				float noise = ValueNoise(uv);
				noise = (noise + 1) * 0.5;

				float value = noise;
				return float4(value, value, value, 1);
			}
			ENDCG
		}
	}
}
