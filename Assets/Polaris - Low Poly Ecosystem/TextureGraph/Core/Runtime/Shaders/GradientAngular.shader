Shader "Hidden/TextureGraph/GradientAngular"
{
	Properties
	{
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

			float4x4 _TransformMatrix;

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
				float2 gradientPoint = mul(_TransformMatrix, float4(uv.xy,0,1));
				float2 normalizedPoint = normalize(gradientPoint);
				float rad = atan2(normalizedPoint.y, normalizedPoint.x);
				float deg = (rad >= 0) * (rad * 57.2958) + (rad < 0) * (359 + rad * 57.2958);
				float f = 1- deg / 359;

				return float4(f, f, f, 1);
			}
			ENDCG
		}
	}
}
