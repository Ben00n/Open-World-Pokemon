Shader "Hidden/TextureGraph/RedToGray"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

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
				float4 col = float4(r, r, r, 1);
				return col;
			}
			ENDCG
		}
	}
}
