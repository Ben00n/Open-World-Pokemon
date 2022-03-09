#ifndef PUNDERWATER_COMMON_INCLUDED
#define PUNDERWATER_COMMON_INCLUDED

PDECLARE_TEXTURE2D(_MainTex);
PDECLARE_DEPTH_TEXTURE;

float _WaterLevel;
float _MaxDepth;
float _SurfaceColorBoost;

float4 _ShallowFogColor;
float4 _DeepFogColor;
float _ViewDistance;

#if CAUSTIC
PDECLARE_TEXTURE2D(_CausticTex);
float _CausticSize;
float _CausticStrength;
#endif

#if DISTORTION
PDECLARE_TEXTURE2D(_DistortionTex);
float _DistortionStrength;
float _WaterFlowSpeed;
#endif

float3 _CameraViewDir;
float _CameraFov;
float4x4 _CameraToWorldMatrix;
float _Intensity;

PDECLARE_TEXTURE2D(_NoiseTex);

float NoiseTexFrag(float2 uv)
{
	return PSAMPLE_TEXTURE2D(_NoiseTex, uv).r * 2 - 1;
}

float InverseLerpUnclamped(float a, float b, float value)
{
	return (value - a) / (b - a + 0.00000001);
}

float3 LinearDepthToWorldPosition(float depth, float2 uv)
{
	float viewPlaneHeight = 2 * depth * tan(radians(_CameraFov * 0.5));
	float viewPlaneWidth = viewPlaneHeight * (_ScreenParams.x / _ScreenParams.y);
	float x = viewPlaneWidth * (uv.x - 0.5);
	float y = viewPlaneHeight * (uv.y - 0.5);
	float3 pos = float3(x, y, -depth);
	float3 worldPos = mul(_CameraToWorldMatrix, float4(pos.xyz, 1)).xyz;
	return worldPos;
}

float3 GetWaterPlaneIntersection(float3 camPos, float3 dir)
{
	float3 planeOrigin = float3(0, _WaterLevel - camPos.y, 0);
	float3 planeNormal = float3(0, 1, 0);
	float rayLength = dot(planeOrigin, planeNormal) / (dot(dir, planeNormal) + 0.000001);
	float3 localIntersect = dir * rayLength;
	float3 worldIntersect = localIntersect + camPos;
	return worldIntersect;
}

#if CAUSTIC
float4 SampleCaustic(float3 worldPos)
{
	float2 uv = worldPos.xz / (_CausticSize + 0.0000001);
	float4 causticColor = PSAMPLE_TEXTURE2D(_CausticTex, uv + _Time.y * 0.0125).rrrr * _CausticStrength;
	float fade = lerp(0.25, 1, NoiseTexFrag(uv * 0.05 - _Time.y * 0.0125));
	causticColor *= fade;
	causticColor = saturate(causticColor);

	return causticColor;
}
#endif

#if DISTORTION
float2 DistorUV(float2 uv)
{
	float depth = _WaterLevel - _WorldSpaceCameraPos.y;
	float fDepth = saturate(depth / _MaxDepth);
	float flow = _WaterFlowSpeed * lerp(1, 0.25, fDepth);
	flow = _WaterFlowSpeed;

	float3 n0 = PSAMPLE_TEXTURE2D(_DistortionTex, uv + flow * _Time.y * 0.025).xyz;
	float3 n1 = PSAMPLE_TEXTURE2D(_DistortionTex, uv * 2 - flow * _Time.y * 2 * 0.025).xyz;
	float3 n = (n0 + n1) * 0.5;
	n = n * 2 - 1;
	n = normalize(n) * _DistortionStrength * 0.025;

	//fade function y=1-x^8, x[-1,1]
	float2 uvRemap = uv * 2 - 1; //remap to [-1, 1]
	float x = uvRemap.x;
	float fadeX = 1 - x * x * x * x * x * x * x * x;
	float y = uvRemap.y;
	float fadeY = 1 - y * y * y * y * y * y * y * y;
	float fade = fadeX * fadeY;
	n *= fade;

	uv += float2(n.x, n.y);
	return uv;
}
#endif

float4 ApplyUnderwater(float2 uv)
{
#if DISTORTION
	uv = DistorUV(uv);
#endif
	float4 color = PSAMPLE_TEXTURE2D(_MainTex, uv);

	float sceneDepth = PLINEAR_EYE_DEPTH(uv);
	float3 worldPos = LinearDepthToWorldPosition(sceneDepth, uv);
	float aboveWaterSurface = worldPos.y >= _WaterLevel;

	float3 direction = normalize(worldPos - _WorldSpaceCameraPos);
	float3 waterIntersection = GetWaterPlaneIntersection(_WorldSpaceCameraPos, direction);
	worldPos = lerp(worldPos, waterIntersection, aboveWaterSurface);

	float depth = _WaterLevel - min(_WorldSpaceCameraPos.y, worldPos.y);
	float fDepth = saturate(depth / _MaxDepth);

	float isCameraBelowWater = _WorldSpaceCameraPos.y <= _WaterLevel;

#if CAUSTIC
	float4 causticColor = SampleCaustic(worldPos);
	color += causticColor * (1 - fDepth) * (1 - aboveWaterSurface) * isCameraBelowWater;
#endif

	float4 fogColor = lerp(_ShallowFogColor, _DeepFogColor, fDepth);

	float d = distance(worldPos, _WorldSpaceCameraPos);
	float fDistance = saturate(InverseLerpUnclamped(-_ViewDistance, _ViewDistance, d));

	float fColor = fDistance * fogColor.a;
	float f = fColor * _Intensity * isCameraBelowWater;

	color *= lerp(1, _SurfaceColorBoost, aboveWaterSurface);
	float4 result = lerp(color, fogColor, f);
	
	return result;
}

#endif
