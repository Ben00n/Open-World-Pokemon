Shader "Hidden/TextureGraph/Clamp"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black" {}
		_Min("Min", Float) = 0
		_Max("Max", Float) = 1
		_ApplyToAlpha("Apply To Alpha", Float) = 0
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
			float _Min;
			float _Max;
			float _ApplyToAlpha;

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
				color = max(color, _Min.rrrr);
				color = min(color, _Max.rrrr);
				color.a = color.a * _ApplyToAlpha + alpha * (1 - _ApplyToAlpha);

				return color;
			}
			ENDCG
		}
	}
}
