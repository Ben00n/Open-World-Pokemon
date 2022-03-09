Shader "Hidden/TextureGraph/GradientMapDynamic"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" {}
		_GradientTex("Gradient", 2D) = "black"{}
		_Slice ("Slice", Float) = 0
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
	float2 uv : TEXCOORD0;
	float4 vertex : SV_POSITION;
};

sampler2D _MainTex;
sampler2D _GradientTex;
float _Slice;

v2f vert(appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	return o;
}
ENDCG

	SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

		Pass
		{
			Name "Horizontal"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float r = tex2D(_MainTex, i.uv).r;
				float4 color = tex2D(_GradientTex, float2(r, _Slice));
#if RRR
				color = float4(color.rrr, 1);
#endif
				return color;
			}

			ENDCG
		}
		Pass
		{
			Name "Vertical"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float r = tex2D(_MainTex, i.uv).r;
				float4 color = tex2D(_GradientTex, float2(_Slice, r));
#if RRR
				color = float4(color.rrr, 1);
#endif
				return color;
			}

			ENDCG
		}
	}
}
