#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

namespace Pinwheel.Poseidon.FX.PostProcessing
{
    [System.Serializable]
    [PostProcess(typeof(PUnderwaterRenderer), PostProcessEvent.BeforeStack, "Poseidon/Underwater", false)]
    public sealed class PUnderwater : PostProcessEffectSettings
    {
        [Header("Water Body")]
        public FloatParameter waterLevel = new FloatParameter();
        public FloatParameter maxDepth = new FloatParameter() { value = 10 };
        [Range(0f, 3f)]
        public FloatParameter surfaceColorBoost = new FloatParameter() { value = 1 };

        [Header("Fog")]
        public ColorParameter shallowFogColor = new ColorParameter();
        public ColorParameter deepFogColor = new ColorParameter();
        public FloatParameter viewDistance = new FloatParameter() { value = 40 };

        [Header("Caustic")]
        public BoolParameter enableCaustic = new BoolParameter();
        public TextureParameter causticTexture = new TextureParameter();
        public FloatParameter causticSize = new FloatParameter();
        [Range(0f, 1f)]
        public FloatParameter causticStrength = new FloatParameter();

        [Header("Distortion")]
        public BoolParameter enableDistortion = new BoolParameter();
        public TextureParameter distortionNormalMap = new TextureParameter();
        public FloatParameter distortionStrength = new FloatParameter();
        public FloatParameter waterFlowSpeed = new FloatParameter();

        [Header("Internal")]
        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter();

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value
                && intensity.value > 0
                && context.camera.transform.position.y <= waterLevel.value;
        }
    }
}
#endif
