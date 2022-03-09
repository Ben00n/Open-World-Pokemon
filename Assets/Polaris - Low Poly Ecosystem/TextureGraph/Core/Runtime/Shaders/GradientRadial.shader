Shader "Hidden/TextureGraph/GradientRadial"
{
	Properties
	{
		_Center("Center", Vector) = (0.5, 0.5, 0, 0)
		_EndPoint("End Point", Vector) = (1, 0.5, 0, 0)
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

			float4 _Center;
			float4 _EndPoint;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float radius = distance(_Center, _EndPoint);
				float d = distance(i.uv, _Center);
				float f = saturate(1 - d / radius);

				return float4(f, f, f, 1);
			}
			ENDCG
		}
	}
}
