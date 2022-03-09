Shader "Hidden/TextureGraph/ColorspaceConversion"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
	}
CGINCLUDE
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

	v2f vert(appdata v)
	{
		v2f o;
		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = v.uv;
		return o;
	}


	inline float GammaToLinearSpaceExact(float value)
	{
		if (value <= 0.04045F)
			return value / 12.92F;
		else if (value < 1.0F)
			return pow((value + 0.055F) / 1.055F, 2.4F);
		else
			return pow(value, 2.2F);
	}

	inline float3 GammaToLinearSpace(float3 sRGB)
	{
		// Approximate version from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
		//return sRGB * (sRGB * (sRGB * 0.305306011h + 0.682171111h) + 0.012522878h);

		// Precise version, useful for debugging.
		return float3(GammaToLinearSpaceExact(sRGB.r), GammaToLinearSpaceExact(sRGB.g), GammaToLinearSpaceExact(sRGB.b));
	}

	inline float LinearToGammaSpaceExact(float value)
	{
		if (value <= 0.0F)
			return 0.0F;
		else if (value <= 0.0031308F)
			return 12.92F * value;
		else if (value < 1.0F)
			return 1.055F * pow(value, 0.4166667F) - 0.055F;
		else
			return pow(value, 0.45454545F);
	}

	inline float3 LinearToGammaSpace(float3 linRGB)
	{
		linRGB = max(linRGB, half3(0.h, 0.h, 0.h));
		// An almost-perfect approximation from http://chilliant.blogspot.com.au/2012/08/srgb-approximations-for-hlsl.html?m=1
		//return max(1.055h * pow(linRGB, 0.416666667h) - 0.055h, 0.h);

		// Exact version, useful for debugging.
		return float3(LinearToGammaSpaceExact(linRGB.r), LinearToGammaSpaceExact(linRGB.g), LinearToGammaSpaceExact(linRGB.b));
	}

ENDCG
	SubShader
	{
		Pass
		{
			Name "Gamma To Linear"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif
				color.rgb = GammaToLinearSpace(color.rgb);

				return color;
			}
			ENDCG
		}
		Pass
		{
			Name "Linear To Gamma"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif
				color.rgb = LinearToGammaSpace(color.rgb);

				return color;
			}
			ENDCG
		}
	}
}
