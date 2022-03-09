Shader "Hidden/TextureGraph/PerlinNoise"
{
	Properties
	{
		_Scale("Scale", Float) = 1
		_Seed("Seed", Float) = 0
	}
	CGINCLUDE
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
	float4x4 _VariantMatrix;

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}

	float2 GradientNoise_dir(float2 p)
	{
		float2 offset = floor(RandomValue(_Seed) * 123);
		p = p % _Scale + offset;
		float x = (34 * p.x + 1) * p.x % 289 + p.y;
		x = (34 * x + 1) * x % 289;
		x = frac(x / 41) * 2 - 1;
		float2 dir = float2(x - floor(x + 0.5), abs(x) - 0.5);
		dir = mul(_VariantMatrix, dir);

		return normalize(dir);
	}

	float GradientNoise(float2 p)
	{
		float2 ip = floor(p);
		float2 fp = frac(p);
		float d00 = dot(GradientNoise_dir(ip), fp);
		float d01 = dot(GradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
		float d10 = dot(GradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
		float d11 = dot(GradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
		fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
		return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
	}
	ENDCG

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Name "Perlin"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv * _Scale;
				float noise = GradientNoise(uv);
				noise = (noise + 1) * 0.5;

				float value = noise;
				return float4(value, value, value, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Billow"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv * _Scale;
				float noise = GradientNoise(uv);
				noise = abs(noise);

				float value = noise;
				return float4(value, value, value, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Ridged"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv * _Scale;
				float noise = GradientNoise(uv);
				noise = 1 - abs(noise);

				float value = noise;
				return float4(value, value, value, 1);
			}
			ENDCG
		}
	}
}
