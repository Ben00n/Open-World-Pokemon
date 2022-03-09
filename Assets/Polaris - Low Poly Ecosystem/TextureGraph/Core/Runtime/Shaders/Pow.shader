Shader "Hidden/TextureGraph/Pow"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_Power("Power", Float) = 1
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
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
			float _Power;

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
				float alpha = color.a;
				color = pow(color, _Power);
				color.a = alpha;

				return color;
			}
			ENDCG
		}
	}
}
