#if POSEIDON_URP
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Pinwheel.Poseidon.FX.Universal
{
    [System.Serializable]
    [VolumeComponentMenu("Poseidon/Wet Lens")]
    public class PWetLensOverride : VolumeComponent
    {
        [Header("Lens Distort")]
        public TextureParameter normalMap = new TextureParameter(null);
        [Range(0f, 1f)]
        public FloatParameter strength = new FloatParameter(1);

        [Header("Internal")]
        [Range(0f, 1f)]
        public FloatParameter intensity = new FloatParameter(0);
    }
}
#endif
