Shader "Hidden/TextureGraph/GradientMap"
{
	Properties
	{
		_MainTex("Texture", 2D) = "black" {}
		_GradientTex("Gradient", 2D) = "black"{}
		_Scale ("Scale", Float) = 1
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		LOD 100

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
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			sampler2D _GradientTex;
			float _Scale;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float r = tex2D(_MainTex, i.uv).r;
				r = r * _Scale;
				float4 color = tex2D(_GradientTex, float2(r, r));

				return color;
			}

			ENDCG
		}
	}
}
