Shader "Hidden/TextureGraph/Crop"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
		_Rect("Rect", Vector) = (0,0,1,1)
		_BackgroundColor("Background Color", Color) = (0,0,0,1)
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
			float4 _Rect;
			float4 _BackgroundColor;

			float4x4 _UvToRectMatrix;

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

				float2 uvRectSpace = mul(_UvToRectMatrix, float4(i.uv.xy, 0, 1));
				float isInsideRect = (uvRectSpace.x >= -0.5) * (uvRectSpace.x <= 0.5) * (uvRectSpace.y >= -0.5) * (uvRectSpace.y <= 0.5);
				color = color * isInsideRect + _BackgroundColor * (1 - isInsideRect);

				return color;
			}
			ENDCG
		}
	}
}
