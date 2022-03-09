Shader "Hidden/TextureGraph/Normal"
{
	Properties
	{
		_HeightMap("Height Map", 2D) = "black"{}
		_Strength("Strength", Float) = 1
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

			sampler2D _HeightMap;
			float4 _HeightMap_TexelSize;
			float _Strength;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag(v2f i) : SV_Target
			{
				float inverseSqrt2 = 1.0 / sqrt(2.0);
				float2 texel = _HeightMap_TexelSize.xy;
				float2 uvLeft = i.uv - float2(texel.x, 0);
				float2 uvUp = i.uv + float2(0, texel.y);
				float2 uvRight = i.uv + float2(texel.x, 0);
				float2 uvDown = i.uv - float2(0, texel.y);
				float2 uvCenter = i.uv;
				float2 uvLeftUp = i.uv + float2(-texel.x, texel.y) * inverseSqrt2;
				float2 uvUpRight = i.uv + float2(texel.x, texel.y) * inverseSqrt2;
				float2 uvRightDown = i.uv + float2(texel.x, -texel.y) * inverseSqrt2;
				float2 uvDownLeft = i.uv + float2(-texel.x, -texel.y) * inverseSqrt2;

				float leftHeight = tex2D(_HeightMap, uvLeft).r * _Strength;
				float upHeight = tex2D(_HeightMap, uvUp).r * _Strength;
				float rightHeight = tex2D(_HeightMap, uvRight).r * _Strength;
				float downHeight = tex2D(_HeightMap, uvDown).r * _Strength;
				float centerHeight = tex2D(_HeightMap, uvCenter).r * _Strength;
				float leftUpHeight = tex2D(_HeightMap, uvLeftUp).r * _Strength;
				float upRightHeight = tex2D(_HeightMap, uvUpRight).r * _Strength;
				float rightDownHeight = tex2D(_HeightMap, uvRightDown).r * _Strength;
				float downLeftHeight = tex2D(_HeightMap, uvDownLeft).r * _Strength;

				float3 left = float3(uvLeft.x, leftHeight, uvLeft.y);
				float3 up = float3(uvUp.x, leftHeight, uvUp.y);
				float3 right = float3(uvRight.x, leftHeight, uvRight.y);
				float3 down = float3(uvDown.x, leftHeight,  uvDown.y);
				float3 center = float3(uvCenter.x, centerHeight,  uvCenter.y);
				float3 leftUp = float3(uvLeftUp.x, leftUpHeight,  uvLeftUp.y);
				float3 upRight = float3(uvUpRight.x, upRightHeight,  uvUpRight.y);
				float3 rightDown = float3(uvRightDown.x, rightDownHeight,  uvRightDown.y);
				float3 downLeft = float3(uvDownLeft.x, downLeftHeight, uvDownLeft.y);

				float3 n0 = cross(left - center, leftUp - center);
				float3 n1 = cross(up - center, upRight - center);
				float3 n2 = cross(right - center, rightDown - center);
				float3 n3 = cross(down - center, downLeft - center);

				float3 n4 = cross(leftUp - center, up - center);
				float3 n5 = cross(upRight - center, right - center);
				float3 n6 = cross(rightDown - center, down - center);
				float3 n7 = cross(downLeft - center, left - center);

				float3 nc = (n0 + n1 + n2 + n3 + n4 + n5 + n6 + n7) / 8;

				float3 n = float3(nc.x, nc.z, nc.y);
				float3 normal = normalize(n);

				float3 col = float3(
					(normal.x + 1) / 2,
					(normal.y + 1) / 2,
					(normal.z + 1) / 2);
				return float4(col,1);
			}
			ENDCG
		}
	}
}
