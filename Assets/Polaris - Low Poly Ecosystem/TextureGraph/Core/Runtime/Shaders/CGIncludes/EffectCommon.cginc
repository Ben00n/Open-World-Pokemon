#ifndef EFFECT_COMMON_INCLUDED
#define EFFECT_COMMON_INCLUDED

float3 ExtractNormal(sampler2D _HeightMap, float4 _HeightMap_TexelSize, float2 uv)
{
	float inverseSqrt2 = 1.0 / sqrt(2.0);
	float2 texel = _HeightMap_TexelSize.xy;
	float2 uvLeft = uv - float2(texel.x, 0);
	float2 uvUp = uv + float2(0, texel.y);
	float2 uvRight = uv + float2(texel.x, 0);
	float2 uvDown = uv - float2(0, texel.y);
	float2 uvCenter = uv;
	float2 uvLeftUp = uv + float2(-texel.x, texel.y) * inverseSqrt2;
	float2 uvUpRight = uv + float2(texel.x, texel.y) * inverseSqrt2;
	float2 uvRightDown = uv + float2(texel.x, -texel.y) * inverseSqrt2;
	float2 uvDownLeft = uv + float2(-texel.x, -texel.y) * inverseSqrt2;

	float leftHeight = tex2D(_HeightMap, uvLeft).r;
	float upHeight = tex2D(_HeightMap, uvUp).r;
	float rightHeight = tex2D(_HeightMap, uvRight).r;
	float downHeight = tex2D(_HeightMap, uvDown).r;
	float centerHeight = tex2D(_HeightMap, uvCenter).r;
	float leftUpHeight = tex2D(_HeightMap, uvLeftUp).r;
	float upRightHeight = tex2D(_HeightMap, uvUpRight).r;
	float rightDownHeight = tex2D(_HeightMap, uvRightDown).r;
	float downLeftHeight = tex2D(_HeightMap, uvDownLeft).r;

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
	return normal;
}

#endif