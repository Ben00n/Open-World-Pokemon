Shader "Hidden/TextureGraph/PreviewRedToGray"
{
	Properties
	{
		_MainTex("Main Tex", 2D) = "black"{}
	}
		SubShader
	{
		Tags { "ForceSupported" = "True" }

		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		ZTest Always

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
				float2 texgencoord : TEXCOORD1;
			};

			sampler2D _MainTex;
			sampler2D _GUIClipTexture;
			uniform float4x4 unity_GUIClipTextureMatrix;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float3 texgen = UnityObjectToViewPos(v.vertex);
				o.texgencoord = mul(unity_GUIClipTextureMatrix, float4(texgen.xy, 0, 1.0));
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float r = tex2D(_MainTex, i.uv).r;
				float4 col = float4(r, r, r, 1);
				col.a *= tex2D(_GUIClipTexture, i.texgencoord).a;
				return col;
			}
			ENDCG
		}
	}
}
