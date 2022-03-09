Shader "Hidden/TextureGraph/ContrastBrightness"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_Contrast("Contrast", Float) = 0
		_Brightness("Brightness", Float) = 0
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
			float _Contrast;
			float _Brightness;

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

				float rangeWidth = _Contrast + 1;
				float rangeOffset = 0.5 + _Brightness * 0.5;
				float rangeMin = rangeOffset - rangeWidth * 0.5;
				float rangeMax = rangeOffset + rangeWidth * 0.5;
				
				color.rgb = lerp(rangeMin.xxx, rangeMax.xxx, color.rgb);
				return color;
			}
			ENDCG
		}
	}
}
