Shader "Hidden/TextureGraph/Step"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_Step("Step", Float) = 100
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
				float _Step;

				float4 stepValue(float4 v)
				{
					return v - v % (1.0 / _Step);
				}

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
					color = stepValue(color);
					color.a = alpha;

					return color;
				}
				ENDCG
			}
		}
}
