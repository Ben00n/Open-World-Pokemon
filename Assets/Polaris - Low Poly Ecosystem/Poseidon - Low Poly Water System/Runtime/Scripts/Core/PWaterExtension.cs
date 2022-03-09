using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Pinwheel.Poseidon
{
    public static class PWaterExtension
    {
        public static Vector3 GetLocalVertexPosition(this PWater water, Vector3 localPos, bool applyRipple = false)
        {
            if (water.Profile == null)
            {
                throw new NullReferenceException("The water must have a profile to calculate vertex position.");
            }

            if (water.Profile.EnableWave)
            {
                water.ApplyWaveHQ(ref localPos);
            }

            if (applyRipple)
            {
                water.ApplyRipple(ref localPos);
            }

            return localPos;
        }
    }

    internal static class PWaveCalculator
    {
        public static void ApplyWaveHQ(this PWater water, ref Vector3 v0)
        {
            Vector3 offset0 = water.CalculateWaveOffsetHQ(v0);
            v0 += offset0;
        }

        private static Vector3 CalculateWaveOffsetHQ(this PWater water, Vector3 vertex)
        {
            PWaterProfile profile = water.Profile;
            Vector2 waveDirection = new Vector2(Mathf.Cos(profile.WaveDirection * Mathf.Deg2Rad), Mathf.Sin(profile.WaveDirection * Mathf.Deg2Rad));
            float time = (float)water.GetTimeParam();

            //Vector3 worldPos = water.transform.localToWorldMatrix.MultiplyPoint(vertex);
            Vector3 worldPos = water.transform.TransformPoint(vertex);
            Vector2 worldXZ = new Vector2(worldPos.x, worldPos.z);
            Vector2 noisePos = worldXZ - 2 * profile.WaveSpeed * waveDirection * time;
            float noise = PNoiseTextureSampler.SampleVertexNoise(noisePos * 0.001f) * profile.WaveDeform;

            float dotValue = Vector2.Dot(worldXZ, waveDirection);
            float t = Frac((dotValue - profile.WaveSpeed * time) / profile.WaveLength - noise * profile.WaveDeform * 0.25f);

            Vector2 p = SampleWaveCurve(t, profile.WaveSteepness);
            p.y -= Mathf.Lerp(0.5f, 1f, noise * 0.5f + 0.5f);
            Vector3 offset = new Vector3(0, profile.WaveHeight * p.y, 0);
            if (water.UseWaveMask && water.WaveMask != null)
            {
                Rect waveMaskBounds = water.WaveMaskBounds;
                float maskU = InverseLerpUnclamped(waveMaskBounds.min.x, waveMaskBounds.max.x, worldPos.x);
                float maskV = InverseLerpUnclamped(waveMaskBounds.min.y, waveMaskBounds.max.y, worldPos.z);
                Color mask = water.WaveMask.GetPixelBilinear(maskU, maskV);
                float inbounds = (maskU >= 0 && maskU <= 1 && maskV >= 0 && maskV <= 1) ? 1 : 0;
                float heightMask = Mathf.Lerp(1, mask.a, inbounds);
                offset.y *= heightMask;
            }

            return offset;
        }

        private static float Frac(float v)
        {
            return v - Mathf.Floor(v);
        }

        private static Vector2 SampleWaveCurve(float t, float waveSteepness)
        {
            Vector2 left = Vector2.zero;
            Vector2 right = Vector2.right;
            Vector2 middle = new Vector2(Mathf.Lerp(0.5f, 0.95f, waveSteepness), 1);

            Vector2 anchor0 = left * ((t < middle.x) ? 1 : 0) + middle * ((t >= middle.x) ? 1 : 0);
            Vector2 anchor1 = middle * ((t < middle.x) ? 1 : 0) + right * ((t >= middle.x) ? 1 : 0);
            float tangentLength = 0.5f;
            Vector2 tangent0 = new Vector2(tangentLength, 0) * ((t < middle.x) ? 1 : 0) + new Vector2(middle.x + tangentLength, middle.y) * ((t >= middle.x) ? 1 : 0);
            Vector2 tangent1 = new Vector2(middle.x - tangentLength, middle.y) * ((t < middle.x) ? 1 : 0) + new Vector2(1 - tangentLength, 0) * ((t >= middle.x) ? 1 : 0);
            float splineT = InverseLerpUnclamped(anchor0.x, anchor1.x, t);

            Vector2 p = SampleSpline(splineT, anchor0, anchor1, tangent0, tangent1);
            return p;
        }

        private static float InverseLerpUnclamped(float a, float b, float value)
        {
            //adding a==b check if needed
            return (value - a) / (b - a + 0.000001f);
        }

        private static Vector2 SampleSpline(float t, Vector2 anchor0, Vector2 anchor1, Vector2 tangent0, Vector2 tangent1)
        {
            float oneMinusT = 1 - t;
            Vector2 p =
                oneMinusT * oneMinusT * oneMinusT * anchor0 +
                3 * oneMinusT * oneMinusT * t * tangent0 +
                3 * oneMinusT * t * t * tangent1 +
                t * t * t * anchor1;
            return p;
        }
    }

    internal static class PRippleCalculator
    {
        private static Vector3 CalculateRippleVertexOffset(this PWater water, Vector3 localPos, float rippleHeight, float rippleSpeed, float rippleScale, Vector3 flowDir)
        {
            Vector3 worldPos = water.transform.localToWorldMatrix.MultiplyPoint(localPos);
            Vector2 noisePos = new Vector2(worldPos.x, worldPos.z);

            rippleScale *= 0.01f;
            rippleSpeed *= 0.1f;

            Vector2 flowDirXZ = new Vector2(flowDir.x, flowDir.z);

            float time = (float)water.GetTimeParam();
            float noiseBase = PNoiseTextureSampler.SampleVertexNoise((noisePos - flowDirXZ * time * rippleSpeed) * rippleScale) * 0.5f + 0.5f;
            float noiseFade = Mathf.Lerp(0.5f, 1f, PNoiseTextureSampler.SampleVertexNoise((noisePos + flowDirXZ * time * rippleSpeed * 3) * rippleScale) * 0.5f + 0.5f);

            float noise = Mathf.Abs(noiseBase - noiseFade);

            Vector3 offset = new Vector3(0, noise * rippleHeight, 0);
            return offset;
        }

        public static void ApplyRipple(this PWater water, ref Vector3 v0)
        {
            Vector3 flowDir = Vector3.one;
            v0 += water.CalculateRippleVertexOffset(v0, water.Profile.RippleHeight, water.Profile.RippleSpeed, water.Profile.RippleNoiseScale, flowDir);
        }
    }

    internal static class PNoiseTextureSampler
    {
        public static float SampleVertexNoise(Vector2 uv)
        {
            Texture2D tex = PPoseidonSettings.Instance.NoiseTexture;
            return tex.GetPixelBilinear(uv.x, uv.y, 0).r * 2 - 1;
        }
    }
}
