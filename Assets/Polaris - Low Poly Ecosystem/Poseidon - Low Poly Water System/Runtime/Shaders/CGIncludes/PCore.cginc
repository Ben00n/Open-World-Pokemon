#ifndef PCORE_INCLUDED
#define PCORE_INCLUDED

#include "PUniforms.cginc"
#include "PWave.cginc"

struct Input  
{
	float4 vertexPos;
	float3 worldPos;
	float4 screenPos;
	float3 normal;
	float fogCoord;
	float crestMask;
};
 
void vertexFunction(inout appdata_full v, out Input o)
{
	UNITY_INITIALIZE_OUTPUT(Input, o);
	o.vertexPos = v.vertex;
	
	#if MESH_NOISE
		ApplyMeshNoise(v.vertex, v.texcoord, v.color);
	#endif
	#if WAVE
		ApplyWaveHQ(v.vertex, v.texcoord, v.color, o.crestMask);
	#endif
	ApplyRipple(v.vertex, v.texcoord, v.color);
	CalculateNormal(v.vertex, v.texcoord, v.color, v.normal);
	o.normal = v.normal;

	UNITY_TRANSFER_FOG(o, UnityObjectToClipPos(v.vertex));
}

void finalColorFunction(Input i, SurfaceOutputStandardSpecular o, inout fixed4 color)
{
	UNITY_APPLY_FOG(i.fogCoord, color);
}
#endif
