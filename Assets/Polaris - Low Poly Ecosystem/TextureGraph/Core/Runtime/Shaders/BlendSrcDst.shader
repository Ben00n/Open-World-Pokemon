Shader "Hidden/TextureGraph/BlendSrcDst"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_SrcColor("Src Color", Int) = 5
		_DstColor("Dst Color", Int) = 10
		_SrcAlpha("Src Alpha", Int) = 5
		_DstAlpha("Dst Alpha", Int) = 10
		_ColorOps("Color Ops", Int) = 0
		_AlphaOps("Alpha Ops", Int) = 0
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

	float4 frag(v2f i) : SV_Target
	{
		float4 color = tex2D(_MainTex, i.uv);
	#if RRR
		color = float4(color.rrr, 1);
	#endif
		return color;
	}
	ENDCG

	SubShader
	{
		Pass
		{
			Blend Off
			CGPROGRAM
			ENDCG
		}
		Pass
		{
			Blend [_SrcColor] [_DstColor], [_SrcAlpha] [_DstAlpha]
			BlendOp [_ColorOps], [_AlphaOps]
			CGPROGRAM
			ENDCG
		}
	}
}
