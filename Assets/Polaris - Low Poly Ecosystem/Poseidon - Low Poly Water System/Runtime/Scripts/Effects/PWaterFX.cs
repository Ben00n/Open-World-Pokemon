using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Events;
#if UNITY_POST_PROCESSING_STACK_V2
using Pinwheel.Poseidon.FX.PostProcessing;
using UnityEngine.Rendering.PostProcessing;
#endif
#if POSEIDON_URP
using Pinwheel.Poseidon.FX.Universal;
#endif

namespace Pinwheel.Poseidon.FX
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(PWater))]
    public class PWaterFX : MonoBehaviour
    {
        [SerializeField]
        private PWater water;
        public PWater Water
        {
            get
            {
                return water;
            }
            set
            {
                water = value;
            }
        }

        [SerializeField]
        private PWaterFXProfile profile;
        public PWaterFXProfile Profile
        {
            get
            {
                return profile;
            }
            set
            {
                PWaterFXProfile oldProfile = profile;
                PWaterFXProfile newProfile = value;
                profile = newProfile;

                if (oldProfile != newProfile && newProfile != null)
                {
                    UpdatePostProcessOrVolumeProfile();
                }
            }
        }

#if UNITY_POST_PROCESSING_STACK_V2
        public PostProcessProfile PostProcessProfile { get; private set; }
        public PostProcessVolume PostProcessVolume { get; private set; }
#endif

#if POSEIDON_URP
        public VolumeProfile VolumeProfile { get; private set; }
        public Volume Volume { get; private set; }
#endif

        [SerializeField]
        private Vector3 volumeExtent;
        public Vector3 VolumeExtent
        {
            get
            {
                return volumeExtent;
            }
            set
            {
                Vector3 v = value;
                //v.x = Mathf.Max(0, v.x);
                //v.y = Mathf.Max(0, v.y);
                //v.z = Mathf.Max(0, v.z);
                volumeExtent = v;
            }
        }

        [SerializeField]
        private LayerMask volumeLayer;
        public LayerMask VolumeLayer
        {
            get
            {
                return volumeLayer;
            }
            set
            {
                volumeLayer = value;
            }
        }

        private float lastDistanceToSurface;
        private float wetLensTime;

        [SerializeField]
        private UnityEvent onEnterWater;
        public UnityEvent OnEnterWater
        {
            get
            {
                return onEnterWater;
            }
            set
            {
                onEnterWater = value;
            }
        }

        [SerializeField]
        private UnityEvent onExitWater;
        public UnityEvent OnExitWater
        {
            get
            {
                return onExitWater;
            }
            set
            {
                onExitWater = value;
            }
        }

        private void Reset()
        {
            Water = GetComponent<PWater>();
#if UNITY_POST_PROCESSING_STACK_V2
            VolumeExtent = new Vector3(0, 100, 0);
#endif
        }

        private void OnEnable()
        {
            Camera.onPreCull += OnCameraPreCull;
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

            if (Camera.main != null)
            {
                float waterHeight = GetWaterHeight(Camera.main.transform.position);
                lastDistanceToSurface = Camera.main.transform.position.y - waterHeight;
            }

            wetLensTime = Mathf.Infinity;

            if (PUtilities.IsPlaying)
            {
                SetupQuickVolume();
            }
        }

        private void OnDisable()
        {
            Camera.onPreCull -= OnCameraPreCull;
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
            CleanupQuickVolume();
        }

        private void OnBeginCameraRendering(ScriptableRenderContext context, Camera cam)
        {
            OnCameraPreCull(cam);
        }

        private void OnCameraPreCull(Camera cam)
        {
            UpdateEffects(cam);
            if (cam == Camera.main && Water != null)
            {
                Vector3 cameraPos = cam.transform.position;
                float waterHeight = GetWaterHeight(cam.transform.position);
                float distanceToSuface = cam.transform.position.y - waterHeight;
                if (distanceToSuface <= 0 && lastDistanceToSurface > 0)
                {
                    if (Water.CheckTilesContainPoint(cameraPos))
                    {
                        OnEnterWater.Invoke();
                    }
                }
                if (distanceToSuface > 0 && lastDistanceToSurface <= 0)
                {
                    if (Water.CheckTilesContainPoint(cameraPos))
                    {
                        OnExitWater.Invoke();
                    }
                }
                lastDistanceToSurface = cameraPos.y - waterHeight;
            }
        }

        private void SetupQuickVolume()
        {
            if (Water == null || Profile == null)
                return;

            Water.ReCalculateBounds();
            Bounds bounds = Water.Bounds;
            GameObject volumeGO = null;

#if UNITY_POST_PROCESSING_STACK_V2
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                PostProcessEffectSettings[] settings = new PostProcessEffectSettings[0];
                PostProcessVolume volume = PostProcessManager.instance.QuickVolume(VolumeLayer, 0, settings);
                volume.isGlobal = false;
                volumeGO = volume.gameObject;
                PostProcessVolume = volume;
                PostProcessProfile = volume.profile;
                PostProcessProfile.name = "~TemporaryEffectProfile";
                Profile.UpdatePostProcessingProfile(PostProcessProfile);
            }
#endif
#if POSEIDON_URP
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                volumeGO = new GameObject();
                volumeGO.layer = VolumeLayer;
                Volume volume = volumeGO.AddComponent<Volume>();
                volume.isGlobal = false;

                VolumeProfile profile = ScriptableObject.CreateInstance<VolumeProfile>();
                volume.profile = profile;

                this.Volume = volume;
                this.VolumeProfile = profile;
                this.VolumeProfile.name = "~TemporaryEffectProfile";
                Profile.UpdateVolumeProfile(VolumeProfile);
            }
#endif

            volumeGO.hideFlags = HideFlags.DontSave;
            volumeGO.transform.parent = transform;
            volumeGO.transform.localPosition = bounds.center;
            volumeGO.transform.localRotation = Quaternion.identity;
            volumeGO.transform.localScale = Vector3.one;
            volumeGO.name = "~WaterPostFXVolume";

            BoxCollider b = volumeGO.AddComponent<BoxCollider>();
            b.center = new Vector3(0, 0, 0);
            b.size = bounds.size + VolumeExtent;
            b.isTrigger = true;
        }

        private void CleanupQuickVolume()
        {
#if UNITY_POST_PROCESSING_STACK_V2
            if (PostProcessProfile != null)
            {
                PUtilities.DestroyObject(PostProcessProfile);
            }
            if (PostProcessVolume != null)
            {
                PUtilities.DestroyGameobject(PostProcessVolume.gameObject);
            }
#endif
#if POSEIDON_URP
            if (VolumeProfile != null)
            {
                PUtilities.DestroyObject(VolumeProfile);
            }
            if (Volume != null)
            {
                PUtilities.DestroyGameobject(Volume.gameObject);
            }
#endif
        }

        private void UpdateEffects(Camera cam)
        {
            if (cam == null)
                return;
            if (cam != Camera.main)
                return;
            if (Profile == null)
                return;

            float waterHeight = GetWaterHeight(cam.transform.position);

            if (Profile.EnableUnderwater)
            {
                float intensity = cam.transform.position.y < waterHeight ? 1 : 0;
                UpdateUnderwaterIntensityAndWaterLevel(intensity, waterHeight);
            }

            float distanceToSurface = Camera.main.transform.position.y - waterHeight;
            if (Profile.EnableWetLens)
            {
                if (distanceToSurface > 0 && lastDistanceToSurface <= 0)
                {
                    wetLensTime = 0;
                }
                float intensity;
                if (distanceToSurface <= 0)
                {
                    intensity = 0;
                }
                else if (wetLensTime > Profile.WetLensDuration)
                {
                    intensity = 0;
                }
                else
                {
                    float f = Mathf.InverseLerp(0, Profile.WetLensDuration, wetLensTime);
                    intensity = Profile.WetLensFadeCurve.Evaluate(f);
                }
                UpdateWetLensIntensity(intensity);
            }
            wetLensTime += PUtilities.DeltaTime;
        }

        private void UpdateUnderwaterIntensityAndWaterLevel(float intensity, float waterLevel)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                if (PostProcessProfile == null)
                    return;
                PUnderwater underwaterSettings;
                if (PostProcessProfile.TryGetSettings<PUnderwater>(out underwaterSettings))
                {
                    underwaterSettings.intensity.Override(intensity);
                    underwaterSettings.waterLevel.Override(waterLevel);
                }
            }
#endif

#if POSEIDON_URP
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                if (VolumeProfile == null)
                    return;
                PUnderwaterOverride underwaterSettigns;
                if (VolumeProfile.TryGet<PUnderwaterOverride>(out underwaterSettigns))
                {
                    underwaterSettigns.intensity.Override(intensity);
                    underwaterSettigns.waterLevel.Override(waterLevel);
                }
            }
#endif
        }

        private void UpdateWetLensIntensity(float intensity)
        {
#if UNITY_POST_PROCESSING_STACK_V2
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                if (PostProcessProfile == null)
                    return;
                PWetLens wetlensSettings;
                if (PostProcessProfile.TryGetSettings<PWetLens>(out wetlensSettings))
                {
                    wetlensSettings.intensity.Override(intensity);
                }
            }
#endif

#if POSEIDON_URP
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                if (VolumeProfile == null)
                    return;
                PWetLensOverride wetlensSettings;
                if (VolumeProfile.TryGet<PWetLensOverride>(out wetlensSettings))
                {
                    wetlensSettings.intensity.Override(intensity);
                }
            }
#endif
        }

        public void UpdatePostProcessOrVolumeProfile()
        {
            if (profile == null)
                return;

#if UNITY_POST_PROCESSING_STACK_V2
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                if (PostProcessProfile != null)
                {
                    profile.UpdatePostProcessingProfile(PostProcessProfile);
                }
            }
#endif
#if POSEIDON_URP
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                if (VolumeProfile != null)
                {
                    profile.UpdateVolumeProfile(VolumeProfile);
                }
            }
#endif
        }

        private float GetWaterHeight(Vector3 worldPos)
        {
            if (water == null)
                return 0;
            if (water.Profile.EnableWave)
            {
                Vector3 localPos = water.transform.InverseTransformPoint(worldPos);
                localPos.y = 0;
                localPos = water.GetLocalVertexPosition(localPos, false);

                worldPos = water.transform.TransformPoint(localPos);
                return worldPos.y;
            }
            else
            {
                return water.transform.position.y;
            }
        }
    }
}
