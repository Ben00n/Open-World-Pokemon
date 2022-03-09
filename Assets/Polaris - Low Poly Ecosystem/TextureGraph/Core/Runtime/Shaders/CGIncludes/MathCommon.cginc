#ifndef MATH_COMMON_INCLUDED
#define MATH_COMMON_INCLUDED

#define PI 3.14159265359

float RandomValue(float seed)
{
	return frac(sin(dot(float2(seed, seed + 1), float2(12.98, 78.23))) * 43.54);
}

float RandomValue(float u, float v)
{
	return frac(sin(dot(float2(u,v), float2(12.9898, 78.233))) * 43758.5453);
}

float InverseLerpUnclamped(float a, float b, float value)
{
	//adding a==b check if needed
	return (value - a) / (b - a + 0.0000001);
}

#endif