Shader "Hidden/TextureGraph/Brick"
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
			#include "./CGIncludes/MathCommon.cginc"

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

			float4x4 _UvToCellMatrix;
			float4x4 _CellToBrickMatrix;
			float _GapSize;
			float _InnerSize;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float CalculateBrickShape(float2 uv)
			{
				uv = uv*0.5 + float2(0.5, 0.5); //Remap to [0,1]
				float minX = min(uv.x, 1 - uv.x);
				float minY = min(uv.y, 1 - uv.y);
				float f = saturate(min(minX, minY) * 2);
				f = saturate(InverseLerpUnclamped(0, 1 - _InnerSize, f));

				return float4(f, f, f, 1);
			}

			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float2 cellUV = mul(_UvToCellMatrix, float4(uv.xy, 0, 1));
				float isOddLine = (floor(cellUV.y) % 2) != 0;
				float xOffset = isOddLine * 0.5;
				cellUV.x += xOffset;
				cellUV = frac(cellUV) * 2 - 1;

				float2 s = sign(cellUV);
				float2 brickUV = float2(
					s.x*InverseLerpUnclamped(0, 1 - _GapSize, abs(cellUV.x)),
					s.y*InverseLerpUnclamped(0, 1 - _GapSize, abs(cellUV.y)));
				//From now brickUV is in [-1,1]

				float brickShape = CalculateBrickShape(brickUV);
				float isInRange = (abs(brickUV.x) <= 1) * (abs(brickUV.y) <= 1);
				float4 color = isInRange * brickShape + (1 - isInRange) * 0;

				return color;
			}
			ENDCG
		}
	}
}
