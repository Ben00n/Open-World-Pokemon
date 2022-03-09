#ifndef PCOMMON_INCLUDED
#define PCOMMON_INCLUDED

sampler2D _NoiseTex;

float NoiseTexFrag(float2 uv)
{
	return tex2D(_NoiseTex, uv).r*2 - 1;
}

float NoiseTexVert(float2 uv)
{
	return tex2Dlod(_NoiseTex, float4(uv.xy, 0, 0)).r*2 - 1;
}

float2 GradientNoise_dir(float2 p)
{
	p = p % 289;
	float x = (34 * p.x + 1) * p.x % 289 + p.y;
	x = (34 * x + 1) * x % 289;
	x = frac(x / 41) * 2 - 1;
	return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
}

float GradientNoise(float2 p)
{
	float2 ip = floor(p);
	float2 fp = frac(p);
	float d00 = dot(GradientNoise_dir(ip), fp);
	float d01 = dot(GradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
	float d10 = dot(GradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
	float d11 = dot(GradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
	fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
	return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
}

float InverseLerpUnclamped(float a, float b, float value)
{
	//adding a==b check if needed
	return (value - a) / (b - a + 0.00000001);
}

float RandomValue(float seed)
{
	return frac(sin(dot(float2(seed, seed+1), float2(12.9898, 78.233)))*43758.5453);
}

float RandomValue(float x, float y)
{
	return frac(sin(dot(float2(x, y), float2(12.9898, 78.233)))*43758.5453);
}

float2 VoronoiRandomVector (float2 UV, float offset)
{
    float2x2 m = float2x2(15.27, 47.63, 99.41, 89.98);
    UV = frac(sin(mul(UV, m)) * 46839.32);
    return float2(sin(UV.y*+offset)*0.5+0.5, cos(UV.x*offset)*0.5+0.5);
}

float Voronoi(float2 UV, float AngleOffset, float CellDensity)
{
    float2 g = floor(UV * CellDensity);
    float2 f = frac(UV * CellDensity);
    float t = 8.0;
    float3 res = float3(8.0, 0.0, 0.0);
	float noiseValue = 0;

    for(int y=-1; y<=1; y++)
    {
        for(int x=-1; x<=1; x++)
        {
            float2 lattice = float2(x,y);
            float2 offset = VoronoiRandomVector(lattice + g, AngleOffset);
            float d = distance(lattice + offset, f);
            if(d < res.x)
            {
                res = float3(d, offset.x, offset.y);
                noiseValue = res.x;
            }
        }
    }

	return noiseValue;
}

float2 PanUV(float2 uv, float2 speed)
{
	return uv + _Time.y*speed;
}

half IsOrtho()
{
	return unity_OrthoParams.w;
}

half GetNearPlane()
{
	return _ProjectionParams.y;
}

half GetFarPlane()
{
	return _ProjectionParams.z;
}

float SqrDistance(float3 pt1, float3 pt2)
{
  float3 v = pt2 - pt1;
  return dot(v,v);
}

half TriangleWave(half In)
{
    return 2.0 * abs(2 * (In - floor(0.5 + In))) - 1.0;
}

half RandomValueHalf(half x, half y)
{
    return frac(sin(dot(half2(x, y), half2(12.9898, 78.233))) * 43758.5453);
}

half ValueNoiseInterpolate(half a, half b, half t)
{
    return (1.0 - t) * a + (t * b);
}

half ValueNoise(half2 uv)
{
    half2 i = floor(uv);
    half2 f = frac(uv);
    f = f * f * (3.0 - 2.0 * f);

    uv = abs(frac(uv) - 0.5);
    half2 c0 = i + half2(0.0, 0.0);
    half2 c1 = i + half2(1.0, 0.0);
    half2 c2 = i + half2(0.0, 1.0);
    half2 c3 = i + half2(1.0, 1.0);
    half r0 = RandomValueHalf(c0.x, c0.y);
    half r1 = RandomValueHalf(c1.x, c1.y);
    half r2 = RandomValueHalf(c2.x, c2.y);
    half r3 = RandomValueHalf(c3.x, c3.y);

    half bottomOfGrid = ValueNoiseInterpolate(r0, r1, f.x);
    half topOfGrid = ValueNoiseInterpolate(r2, r3, f.x);
    half t = ValueNoiseInterpolate(bottomOfGrid, topOfGrid, f.y);
    return t;
}

float SampleVertexNoise(float2 uv)
{
    return NoiseTexVert(uv);
}

float SampleFragmentNoise(float2 uv)
{
    return NoiseTexFrag(uv);
}

void CalculateNormal(float4 v0, float4 v1, float4 v2, inout float3 normal)
{
    float4 dir0 = v1 - v0;
    float4 dir1 = v2 - v0;

    normal = normalize(cross(dir0.xyz, dir1.xyz));
}
#endif