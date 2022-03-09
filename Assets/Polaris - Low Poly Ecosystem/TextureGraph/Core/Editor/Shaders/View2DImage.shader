Shader "Hidden/TextureGraph/View2DImage"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_RRR("RRR", Int) = 0
		_Channel("Channel", Float) = 0
	}
		SubShader
		{ 
			Tags { "ForceSupported" = "True" }

			Lighting Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Off
			ZWrite Off
			ZTest Always
			LOD 100

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
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
					float2 texgencoord : TEXCOORD1;
				};

				sampler2D _MainTex;
				float2 _Tiling;
				float2 _Offset;
				float _RRR;
				sampler2D _GUIClipTexture;
				uniform float4x4 unity_GUIClipTextureMatrix;
				float _Channel;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					float3 texgen = UnityObjectToViewPos(v.vertex);
					o.texgencoord = mul(unity_GUIClipTextureMatrix, float4(texgen.xy, 0, 1.0));
					o.uv = v.uv;
					return o;
				}

				float4 ApplyChannelMask(float4 color)
				{
					return
						float4(color.r, color.g, color.b, 1) * (_Channel == 0) +
						float4(color.r, 0, 0, 1) * (_Channel == 1) +
						float4(0, color.g, 0, 1) * (_Channel == 2) +
						float4(0, 0, color.b, 1) * (_Channel == 3) +
						float4(color.a, color.a, color.a, 1) * (_Channel == 4);
				}

				float4 frag(v2f i) : SV_Target
				{
					float2 uv = i.uv;
					uv.x = uv.x / _Tiling.x + _Offset.x;
					uv.y = uv.y / _Tiling.y + _Offset.y;
					uv = frac(uv);

					float4 col = tex2D(_MainTex, uv);
					col = float4(col.rrr, 1) * _RRR + col * (1 - _RRR);
					col = ApplyChannelMask(col);
					col.a *= tex2D(_GUIClipTexture, i.texgencoord).a;
					return col;
				}
				ENDCG
			}
		}
}
