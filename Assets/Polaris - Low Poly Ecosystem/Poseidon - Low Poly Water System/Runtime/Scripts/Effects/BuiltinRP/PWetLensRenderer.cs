#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

namespace Pinwheel.Poseidon.FX.PostProcessing
{
    public class PWetLensRenderer : PostProcessEffectRenderer<PWetLens>
    {
        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(PPoseidonSettings.Instance.WetLensShader);
            Texture normalMap = settings.normalMap.value ?? PPoseidonSettings.Instance.DefaultNormalMap;
            sheet.properties.SetTexture(PMat.PP_WET_LENS_TEX, normalMap);
            sheet.properties.SetFloat(PMat.PP_WET_LENS_STRENGTH, settings.strength * settings.intensity);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }
    }
}
#endif
