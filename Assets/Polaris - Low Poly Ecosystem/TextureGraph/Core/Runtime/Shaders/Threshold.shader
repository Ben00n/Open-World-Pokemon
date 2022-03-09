Shader "Hidden/TextureGraph/Threshold"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_ThresholdLow("Threshold Low", Float) = 0
		_ThresholdHigh("Threshold High", Float) = 1
		_Mode("Mode", Float) = 0
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

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
			float _ThresholdLow;
			float _ThresholdHigh;
			float _Mode;

			float ApplyThresholdLow(float v)
			{
				float v0 = v * (v >= _ThresholdLow) + 0 * (v < _ThresholdLow);
				float v1 = v * (v > _ThresholdLow) + 0 * (v <= _ThresholdLow);
				v = v0 * (_Mode == 0) + v1 * (_Mode == 1);
				return v;
			}

			float ApplyThresholdHigh(float v)
			{
				float v0 = v * (v <= _ThresholdHigh) + 1 * (v > _ThresholdHigh);
				float v1 = v * (v < _ThresholdHigh) + 1 * (v >= _ThresholdHigh);
				v = v0 * (_Mode == 0) + v1 * (_Mode == 1);
				return v;
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
				float r = color.r;
				r = ApplyThresholdLow(r);
				r = ApplyThresholdHigh(r);			

				return float4(r, r, r, 1);
			}
			ENDCG
		}
	}
}
