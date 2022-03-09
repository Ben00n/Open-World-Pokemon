Shader "Hidden/TextureGraph/WorleyNoise"
{
	Properties
	{
		_Scale("Scale", Float) = 5
		_Seed("Seed", Float) = 1
		_AngleOffset("Angle Offset", Float) = 0.5
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

			float _Scale;
			float _Seed;
			float _AngleOffset;

			float2 VoronoiRandomVector(float2 UV, float offset)
			{
				float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
				UV = frac(sin(mul(UV, m)) * 46839.32);
				return float2(sin(UV.y * +offset) * 0.5 + 0.5, cos(UV.x * offset) * 0.5 + 0.5);
			}

			float Voronoi(float2 UV, float AngleOffset, float CellDensity)
			{
				float2 g = floor(UV * CellDensity);
				float2 f = frac(UV * CellDensity);
				float t = 8.0;
				float3 res = float3(8.0, 0.0, 0.0);
				float noiseValue = 0;

				for (int y = -1; y <= 1; y++)
				{
					for (int x = -1; x <= 1; x++)
					{
						float2 lattice = float2(x, y);
						float2 offset = VoronoiRandomVector(lattice + g, AngleOffset);
						float d = distance(lattice + offset, f);
						if (d < res.x)
						{
							res = float3(d, offset.x, offset.y);
							noiseValue = res.x;
						}
					}
				}

				return noiseValue;
			}

			float2 RandomPoint01(float2 uv)
			{
				float x = RandomValue(uv.x + 0.0123, uv.y - 0.0456);
				float y = RandomValue(-uv.x - 0.0654, -uv.y + 0.0789);
				return float2(x, y);
			}

			float WorleyNoise(float2 uv)
			{
				float2 cellSize = float2(1.0, 1.0);
				float noiseValue = 0;
				float minD = 100;
				float d = 0;
				float sumD = 0;
				float2 cellOrigin;
				float2 lattice;
				float2 cellRefPoint;

				for (int x = -1; x <= 1; ++x)
				{
					for (int y = -1; y <= 1; ++y)
					{
						lattice = float2(x, y);
						cellOrigin = floor(uv + cellSize * lattice) * cellSize;
						cellRefPoint = RandomPoint01(cellOrigin) * cellSize + cellOrigin;
						d = distance(uv/_Scale, cellRefPoint)/cellSize.x;
						if (d < minD)
						{
							minD = d;
							noiseValue = d;
						}
					}
				}
				return noiseValue;
			}

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
				float noise = Voronoi(uv, _AngleOffset, _Scale);

				float4 color = float4(noise, noise, noise, 1);
				return color;
			}
			ENDCG
		}
	}
}
