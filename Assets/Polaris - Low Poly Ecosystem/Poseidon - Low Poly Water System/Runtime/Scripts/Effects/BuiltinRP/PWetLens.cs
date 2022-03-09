#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

namespace Pinwheel.Poseidon.FX.PostProcessing
{
    [System.Serializable]
    [PostProcess(typeof(PWetLensRenderer), PostProcessEvent.BeforeStack, "Poseidon/Wet Lens", false)]
    public sealed class PWetLens : PostProcessEffectSettings
    {
        [Header("Lens Distort")]
        public TextureParameter normalMap = new TextureParameter();
        [Range(0f, 1f)]
        public FloatParameter strength = new FloatParameter();

        [Header("Internal")]
        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter();

        public override bool IsEnabledAndSupported(PostProcessRenderContext context)
        {
            return enabled.value
                && normalMap.value != null
                && intensity.value > 0;
        }
    }
}
#endif
