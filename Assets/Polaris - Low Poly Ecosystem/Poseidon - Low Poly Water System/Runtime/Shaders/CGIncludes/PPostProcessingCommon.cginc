#ifndef PPOSTPROCESSING_COMMON_INCLUDED
#define PPOSTPROCESSING_COMMON_INCLUDED

#if defined (POSEIDON_SRP)

#define PDECLARE_TEXTURE2D(textureName) TEXTURE2D_X(textureName)
#define PSAMPLE_TEXTURE2D(textureName, uv) SAMPLE_TEXTURE2D_X(textureName, sampler_LinearRepeat, uv)

#define PDECLARE_DEPTH_TEXTURE PDECLARE_TEXTURE2D(_CameraDepthTexture)
#define PLINEAR_EYE_DEPTH(uv) LinearEyeDepth(PSAMPLE_TEXTURE2D(_CameraDepthTexture, uv).r, _ZBufferParams)

#else //Builtin RP

#define PDECLARE_TEXTURE2D(textureName) TEXTURE2D_SAMPLER2D(textureName, sampler##textureName)
#define PSAMPLE_TEXTURE2D(textureName, uv) SAMPLE_TEXTURE2D(textureName, sampler##textureName, uv)

#define PDECLARE_DEPTH_TEXTURE PDECLARE_TEXTURE2D(_CameraDepthTexture)
#define PLINEAR_EYE_DEPTH(uv) LinearEyeDepth(PSAMPLE_TEXTURE2D(_CameraDepthTexture, uv).r)

#endif

#endif
