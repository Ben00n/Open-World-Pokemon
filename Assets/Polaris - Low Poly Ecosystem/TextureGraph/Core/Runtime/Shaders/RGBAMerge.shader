Shader "Hidden/TextureGraph/RGBAMerge"
{
	Properties
	{
		_RedTex ("Red", 2D) = "black"{}
		_GreenTex("Green", 2D) = "black"{}
		_BlueTex("Blue", 2D) = "black"{}
		_AlphaTex("Alpha", 2D) = "white"{}
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

			sampler2D _RedTex;
			sampler2D _GreenTex;
			sampler2D _BlueTex;
			sampler2D _AlphaTex;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float r = tex2D(_RedTex, i.uv).r;
				float g = tex2D(_GreenTex, i.uv).r;
				float b = tex2D(_BlueTex, i.uv).r;
				float a = tex2D(_AlphaTex, i.uv).r;
				float4 color = float4(r, g, b, a);
				return color;
			}
			ENDCG
		}
	}
}
