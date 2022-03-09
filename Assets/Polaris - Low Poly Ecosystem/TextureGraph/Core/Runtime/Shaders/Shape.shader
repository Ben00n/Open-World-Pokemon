Shader "Hidden/TextureGraph/Shape"
{
CGINCLUDE
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

float4x4 _UvToShapeMatrix;
float _InnerSize;

v2f vert(appdata v)
{
	v2f o;
	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = mul(_UvToShapeMatrix, float4(v.uv.xy, 0, 1));
	return o;
}

ENDCG
	SubShader
	{
		Pass
		{
			Name "Square"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float x = i.uv.x;
				float y = i.uv.y;
				float f = (x >= -0.5) * (x <= 0.5) * (y >= -0.5) * (y <= 0.5);
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Disc"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv);
				float f = d <= 0.5;
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Hemisphere"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv) * 2;
				float f = saturate(sqrt(1 - d * d));
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Cone"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv) * 2;
				float f = 1 - d;
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Paraboloid"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv)*2;
				float f = saturate(1 - d * d);
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Bell"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv) * 2;
				float f = saturate(1 - tanh(2*d*2*d));
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Thorn"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv) * 2;
				float f = saturate(-pow(d-1, 5));
				
				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Pyramid"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv + float2(0.5,0.5); //Remap to [0,1]
				float minX = min(uv.x, 1 - uv.x);
				float minY = min(uv.y, 1 - uv.y);
				float f = saturate(min(minX, minY)*2);

				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Brick"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv + float2(0.5,0.5); //Remap to [0,1]
				float minX = min(uv.x, 1 - uv.x);
				float minY = min(uv.y, 1 - uv.y);
				float f = saturate(min(minX, minY) * 2);
				f = saturate(InverseLerpUnclamped(0, 1 - _InnerSize, f));

				return float4(f, f, f, 1);
			}
			ENDCG
		}
		Pass
		{
			Name "Torus"
			CGPROGRAM
			float4 frag(v2f i) : SV_Target
			{
				float2 uv = i.uv;
				float d = length(uv) * 2;
				d = InverseLerpUnclamped(_InnerSize, 1, d);
				d = lerp(-1, 1, d);
				float inRange = (d >= -1) * (d <= 1);
				d = 1 * (1 - inRange) + d * inRange;

				float f = saturate(sqrt(1 - d * d));
				return float4(f, f, f, 1);
			}
			ENDCG
		}
	}
}
