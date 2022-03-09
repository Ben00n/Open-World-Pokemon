Shader "Hidden/TextureGraph/Checkerboard"
{
	Properties
	{
		_Scale("Scale", Float) = 2
		_Color0("Color 0", Color) = (1,1,1,1)
		_Color1("Color 1", Color) = (0,0,0,1)
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

			float _Scale;
			float4 _Color0;
			float4 _Color1;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float cellSize = 1.0 / (2 * _Scale);
				float2 f = floor(i.uv / cellSize);
				float4 color;
				if ((f.x + f.y) % 2 != 0)
				{
					color = _Color1;
				}
				else
				{
					color = _Color0;
				}

				return color;
			}
			ENDCG
		}
	}
}
