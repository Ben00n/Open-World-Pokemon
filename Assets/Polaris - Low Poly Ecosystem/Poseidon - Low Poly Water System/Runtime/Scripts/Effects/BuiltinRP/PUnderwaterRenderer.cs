#if UNITY_POST_PROCESSING_STACK_V2
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;

namespace Pinwheel.Poseidon.FX.PostProcessing
{

    public class PUnderwaterRenderer : PostProcessEffectRenderer<PUnderwater>
    {
        public override void Render(PostProcessRenderContext context)
        {
            PropertySheet sheet = context.propertySheets.Get(PPoseidonSettings.Instance.UnderwaterShader);
            sheet.properties.SetFloat(PMat.PP_WATER_LEVEL, settings.waterLevel);
            sheet.properties.SetFloat(PMat.PP_MAX_DEPTH, settings.maxDepth);
            sheet.properties.SetFloat(PMat.PP_SURFACE_COLOR_BOOST, settings.surfaceColorBoost);

            sheet.properties.SetColor(PMat.PP_SHALLOW_FOG_COLOR, settings.shallowFogColor);
            sheet.properties.SetColor(PMat.PP_DEEP_FOG_COLOR, settings.deepFogColor);
            sheet.properties.SetFloat(PMat.PP_VIEW_DISTANCE, settings.viewDistance);

            if (settings.enableCaustic && settings.causticTexture.value != null)
            {
                sheet.EnableKeyword(PMat.KW_PP_CAUSTIC);
                sheet.properties.SetTexture(PMat.PP_NOISE_TEX, PPoseidonSettings.Instance.NoiseTexture);
                sheet.properties.SetTexture(PMat.PP_CAUSTIC_TEX, settings.causticTexture.value);
                sheet.properties.SetFloat(PMat.PP_CAUSTIC_SIZE, settings.causticSize);
                sheet.properties.SetFloat(PMat.PP_CAUSTIC_STRENGTH, settings.causticStrength);
            }
            else
            {
                sheet.DisableKeyword(PMat.KW_PP_CAUSTIC);
            }

            if (settings.enableDistortion && settings.distortionNormalMap.value != null)
            {
                sheet.EnableKeyword(PMat.KW_PP_DISTORTION);
                sheet.properties.SetTexture(PMat.PP_DISTORTION_TEX, settings.distortionNormalMap.value);
                sheet.properties.SetFloat(PMat.PP_DISTORTION_STRENGTH, settings.distortionStrength);
                sheet.properties.SetFloat(PMat.PP_WATER_FLOW_SPEED, settings.waterFlowSpeed);
            }
            else
            {
                sheet.DisableKeyword(PMat.KW_PP_DISTORTION);
            }

            sheet.properties.SetVector(PMat.PP_CAMERA_VIEW_DIR, context.camera.transform.forward);
            sheet.properties.SetFloat(PMat.PP_CAMERA_FOV, context.camera.fieldOfView);
            sheet.properties.SetMatrix(PMat.PP_CAMERA_TO_WORLD_MATRIX, context.camera.cameraToWorldMatrix);
            sheet.properties.SetFloat(PMat.PP_INTENSITY, settings.intensity);

            context.command.BlitFullscreenTriangle(context.source, context.destination, sheet, 0);
        }

        public override DepthTextureMode GetCameraFlags()
        {
            return DepthTextureMode.Depth;
        }
    }
}
#endif
