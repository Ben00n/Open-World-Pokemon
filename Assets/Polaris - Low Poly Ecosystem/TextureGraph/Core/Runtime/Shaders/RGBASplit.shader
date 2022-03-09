Shader "Hidden/TextureGraph/RGBASplit"
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
ENDCG

	SubShader
	{
		Pass
		{
			Name "Red"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif
				return float4(color.rrr,1);
			}
			ENDCG
		}
		Pass
		{
			Name "Green"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv); 
#if RRR
				color = float4(color.rrr, 1);
#endif
				return float4(color.ggg,1);
			}
			ENDCG
		}
		Pass
		{
			Name "Blue"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif
				return float4(color.bbb,1);
			}
			ENDCG
		}
		Pass
		{
			Name "Alpha"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float4 color = tex2D(_MainTex, i.uv);
#if RRR
				color = float4(color.rrr, 1);
#endif
				return float4(color.aaa,1);
			}
			ENDCG
		}
	}
}
