#ifndef PLIGHTABSORPTION_INCLUDED
#define PLIGHTABSORPTION_INCLUDED

#include "PUniforms.cginc"
#include "PCommon.cginc"
#include "PDepth.cginc"

void CalculateDeepWaterColor(float sceneDepth, float surfaceDepth, out half4 waterColor)
{
	float waterDepth = sceneDepth - surfaceDepth;
	float depthFade = saturate(InverseLerpUnclamped(0, _MaxDepth, waterDepth));

	waterColor = lerp(_Color, _DepthColor, depthFade);
}
#endif