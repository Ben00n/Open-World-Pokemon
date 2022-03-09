using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
#if UNITY_POST_PROCESSING_STACK_V2
using Pinwheel.Poseidon.FX.PostProcessing;
using UnityEngine.Rendering.PostProcessing;
#endif
#if UNITY_EDITOR
using UnityEditor;
#endif
#if POSEIDON_URP
using Pinwheel.Poseidon.FX.Universal;
#endif

namespace Pinwheel.Poseidon.FX
{
    [CreateAssetMenu(menuName = "Poseidon/Water FX Profile")]
    public class PWaterFXProfile : ScriptableObject
    {
        [SerializeField]
        private bool enableUnderwater;
        public bool EnableUnderwater
        {
            get
            {
                return enableUnderwater;
            }
            set
            {
                enableUnderwater = value;
            }
        }

        [SerializeField]
        private float underwaterMaxDepth;
        public float UnderwaterMaxDepth
        {
            get
            {
                return underwaterMaxDepth;
            }
            set
            {
                underwaterMaxDepth = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float underwaterSurfaceColorBoost;
        public float UnderwaterSurfaceColorBoost
        {
            get
            {
                return underwaterSurfaceColorBoost;
            }
            set
            {
                underwaterSurfaceColorBoost = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private Color underwaterShallowFogColor;
        public Color UnderwaterShallowFogColor
        {
            get
            {
                return underwaterShallowFogColor;
            }
            set
            {
                underwaterShallowFogColor = value;
            }
        }

        [SerializeField]
        private Color underwaterDeepFogColor;
        public Color UnderwaterDeepFogColor
        {
            get
            {
                return underwaterDeepFogColor;
            }
            set
            {
                underwaterDeepFogColor = value;
            }
        }

        [SerializeField]
        private float underwaterViewDistance;
        public float UnderwaterViewDistance
        {
            get
            {
                return underwaterViewDistance;
            }
            set
            {
                underwaterViewDistance = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private bool underwaterEnableCaustic;
        public bool UnderwaterEnableCaustic
        {
            get
            {
                return underwaterEnableCaustic;
            }
            set
            {
                underwaterEnableCaustic = value;
            }
        }

        [SerializeField]
        private Texture underwaterCausticTexture;
        public Texture UnderwaterCausticTexture
        {
            get
            {
                return underwaterCausticTexture;
            }
            set
            {
                underwaterCausticTexture = value;
            }
        }

        [SerializeField]
        private float underwaterCausticSize;
        public float UnderwaterCausticSize
        {
            get
            {
                return underwaterCausticSize;
            }
            set
            {
                underwaterCausticSize = value;
            }
        }

        [SerializeField]
        private float underwaterCausticStrength;
        public float UnderwaterCausticStrength
        {
            get
            {
                return underwaterCausticStrength;
            }
            set
            {
                underwaterCausticStrength = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private bool underwaterEnableDistortion;
        public bool UnderwaterEnableDistortion
        {
            get
            {
                return underwaterEnableDistortion;
            }
            set
            {
                underwaterEnableDistortion = value;
            }
        }

        [SerializeField]
        private Texture underwaterDistortionTexture;
        public Texture UnderwaterDistortionTexture
        {
            get
            {
                return underwaterDistortionTexture;
            }
            set
            {
                underwaterDistortionTexture = value;
            }
        }

        [SerializeField]
        private float underwaterDistortionStrength;
        public float UnderwaterDistortionStrength
        {
            get
            {
                return underwaterDistortionStrength;
            }
            set
            {
                underwaterDistortionStrength = value;
            }
        }

        [SerializeField]
        private float underwaterWaterFlowSpeed;
        public float UnderwaterWaterFlowSpeed
        {
            get
            {
                return underwaterWaterFlowSpeed;
            }
            set
            {
                underwaterWaterFlowSpeed = value;
            }
        }

        private float underwaterIntensity;
        public float UnderwaterIntensity
        {
            get
            {
                return underwaterIntensity;
            }
            set
            {
                underwaterIntensity = value;
            }
        }

        [SerializeField]
        private bool enableWetLens;
        public bool EnableWetLens
        {
            get
            {
                return enableWetLens;
            }
            set
            {
                enableWetLens = value;
            }
        }

        [SerializeField]
        private Texture wetLensNormalMap;
        public Texture WetLensNormalMap
        {
            get
            {
                return wetLensNormalMap;
            }
            set
            {
                wetLensNormalMap = value;
            }
        }

        [SerializeField]
        private float wetLensStrength;
        public float WetLensStrength
        {
            get
            {
                return wetLensStrength;
            }
            set
            {
                wetLensStrength = Mathf.Clamp01(value);
            }
        }

        private float wetLensIntensity;
        public float WetLensIntensity
        {
            get
            {
                return wetLensIntensity;
            }
            set
            {
                wetLensIntensity = Mathf.Clamp01(value);
            }
        }

        [SerializeField]
        private float wetLensDuration;
        public float WetLensDuration
        {
            get
            {
                return wetLensDuration;
            }
            set
            {
                wetLensDuration = Mathf.Max(0.01f, value);
            }
        }

        [SerializeField]
        private AnimationCurve wetLensFadeCurve;
        public AnimationCurve WetLensFadeCurve
        {
            get
            {
                if (wetLensFadeCurve == null)
                {
                    wetLensFadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
                }
                return wetLensFadeCurve;
            }
            set
            {
                wetLensFadeCurve = value;
            }
        }

        public void Reset()
        {
            EnableUnderwater = true;
            UnderwaterMaxDepth = 10;
            UnderwaterShallowFogColor = new Color(0, 0, 0, 0.5f);
            UnderwaterDeepFogColor = new Color(0, 0, 0, 0.95f);
            UnderwaterViewDistance = 50;
            UnderwaterEnableCaustic = false;
            UnderwaterCausticTexture = null;
            UnderwaterCausticSize = 10;
            UnderwaterCausticStrength = 1;
            UnderwaterEnableDistortion = true;
            UnderwaterDistortionTexture = PPoseidonSettings.Instance.DefaultUnderwaterDistortionMap;
            UnderwaterDistortionStrength = 0.5f;
            UnderwaterWaterFlowSpeed = 1;

            EnableWetLens = true;
            WetLensNormalMap = PPoseidonSettings.Instance.DefaultWetLensDistortionMap;
            WetLensStrength = 1;
            WetLensDuration = 3;
            WetLensFadeCurve = AnimationCurve.Linear(0, 1, 1, 0);
        }

#if UNITY_POST_PROCESSING_STACK_V2 
        public void UpdatePostProcessingProfile(PostProcessProfile p)
        {
            if (!p.HasSettings<PUnderwater>())
            {
                p.AddSettings<PUnderwater>();
            }

            PUnderwater underwaterSettings = p.GetSetting<PUnderwater>();
            underwaterSettings.active = EnableUnderwater;
            underwaterSettings.enabled.Override(EnableUnderwater);
            if (EnableUnderwater)
            {
                underwaterSettings.maxDepth.Override(UnderwaterMaxDepth);
                underwaterSettings.surfaceColorBoost.Override(UnderwaterSurfaceColorBoost);

                underwaterSettings.shallowFogColor.Override(UnderwaterShallowFogColor);
                underwaterSettings.deepFogColor.Override(UnderwaterDeepFogColor);
                underwaterSettings.viewDistance.Override(UnderwaterViewDistance);

                underwaterSettings.enableCaustic.Override(UnderwaterEnableCaustic);
                underwaterSettings.causticTexture.Override(UnderwaterCausticTexture);
                underwaterSettings.causticSize.Override(UnderwaterCausticSize);
                underwaterSettings.causticStrength.Override(UnderwaterCausticStrength);

                underwaterSettings.enableDistortion.Override(UnderwaterEnableDistortion);
                underwaterSettings.distortionNormalMap.Override(UnderwaterDistortionTexture);
                underwaterSettings.distortionStrength.Override(UnderwaterDistortionStrength);
                underwaterSettings.waterFlowSpeed.Override(UnderwaterWaterFlowSpeed);
            }

            if (!p.HasSettings<PWetLens>())
            {
                p.AddSettings<PWetLens>();
            }

            PWetLens wetLensSettings = p.GetSetting<PWetLens>();
            wetLensSettings.active = EnableWetLens;
            wetLensSettings.enabled.Override(EnableWetLens);
            if (EnableWetLens)
            {
                wetLensSettings.normalMap.Override(WetLensNormalMap);
                wetLensSettings.strength.Override(WetLensStrength);
            }
        }
#endif

#if POSEIDON_URP
        public void UpdateVolumeProfile(VolumeProfile p)
        {
            if (!p.Has<PUnderwaterOverride>())
            {
                p.Add<PUnderwaterOverride>();
            }

            PUnderwaterOverride underwaterSettings;
            if (p.TryGet<PUnderwaterOverride>(out underwaterSettings))
            {
                underwaterSettings.active = EnableUnderwater;
                if (EnableUnderwater)
                {
                    underwaterSettings.maxDepth.Override(UnderwaterMaxDepth);
                    underwaterSettings.surfaceColorBoost.Override(UnderwaterSurfaceColorBoost);

                    underwaterSettings.shallowFogColor.Override(UnderwaterShallowFogColor);
                    underwaterSettings.deepFogColor.Override(UnderwaterDeepFogColor);
                    underwaterSettings.viewDistance.Override(UnderwaterViewDistance);

                    underwaterSettings.enableCaustic.Override(UnderwaterEnableCaustic);
                    underwaterSettings.causticTexture.Override(UnderwaterCausticTexture);
                    underwaterSettings.causticSize.Override(UnderwaterCausticSize);
                    underwaterSettings.causticStrength.Override(UnderwaterCausticStrength);

                    underwaterSettings.enableDistortion.Override(UnderwaterEnableDistortion);
                    underwaterSettings.distortionNormalMap.Override(UnderwaterDistortionTexture);
                    underwaterSettings.distortionStrength.Override(UnderwaterDistortionStrength);
                    underwaterSettings.waterFlowSpeed.Override(UnderwaterWaterFlowSpeed);
                }
            }

            if (!p.Has<PWetLensOverride>())
            {
                p.Add<PWetLensOverride>();
            }

            PWetLensOverride wetLensSettings;
            if (p.TryGet<PWetLensOverride>(out wetLensSettings))
            {
                wetLensSettings.active = EnableWetLens;
                if (EnableWetLens)
                {
                    wetLensSettings.normalMap.Override(WetLensNormalMap);
                    wetLensSettings.strength.Override(WetLensStrength);
                }
            }
        }
#endif
    }
}
