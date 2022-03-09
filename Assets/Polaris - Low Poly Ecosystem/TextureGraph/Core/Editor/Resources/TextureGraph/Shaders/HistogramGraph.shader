Shader "Hidden/TextureGraph/HistogramGraph"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Max("Max", Float) = 1
	}
		SubShader
	{
		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			StructuredBuffer<uint> _Histogram;
			float4 _Color;
			float _Max;

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

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float index = uint(255.0 * uv.x);
				float histogramValue = _Histogram[index];
				float h = histogramValue / _Max;

				float4 color = _Color * (uv.y <= h) + float4(0, 0, 0, 0) * (uv.y > h);

				return color;
			}
			ENDCG
		}
	}
}
