Shader "Hidden/TextureGraph/Warp"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_VectorMap("Vector Map", 2D) = "bump"{}
		_Intensity("Intensity", Float) = 0
		_RRR("RRR", Float) = 0
	}

CGINCLUDE
#define FRAG(iteration)		float4 frag##iteration(v2f v) : SV_Target\
							{\
								float2 uv = v.uv;\
								for (int i = 0; i < iteration; ++i)\
								{\
									uv = DistortUV(uv);\
								}\
								float4 color = tex2D(_MainTex, uv);\
								color = float4(color.rrr, 1)*_RRR + color*(1 - _RRR);\
								return color;\
							}\

#pragma vertex vert

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
sampler2D _VectorMap;
float _Intensity;
float _RRR;

v2f vert(appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	return o;
}

float2 DistortUV(float2 uv)
{
	float2 v = tex2D(_VectorMap, uv).xy;
	v = v * 2 - float2(1, 1); //Remap to [-1,1]
	v *= _Intensity;
	uv = saturate(uv - v);

	return uv;
}

float4 frag1(v2f v) : SV_Target
{
	float2 uv = v.uv;
	uv = DistortUV(uv);
	float4 color = tex2D(_MainTex, uv);
	color = float4(color.rrr, 1) * _RRR + color * (1 - _RRR);
	return color;
}

ENDCG

	SubShader
	{
		Pass{ CGPROGRAM #pragma fragment frag1 ENDCG }
	}
}
