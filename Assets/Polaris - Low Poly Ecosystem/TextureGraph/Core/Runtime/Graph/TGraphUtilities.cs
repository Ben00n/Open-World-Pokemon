using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;

namespace Pinwheel.TextureGraph
{
    public static class TGraphUtilities
    {
        public static void ValidateOutputTexture(TGraphContext context, TSlot slot, ref RenderTexture targetRT)
        {            
            RenderTextureFormat format = RenderTextureFormat.ARGB32;
            if (slot.DataType == TSlotDataType.RGBA)
            {
                format = context.useHighPrecision ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGB32;
            }
            else if (slot.DataType == TSlotDataType.Gray)
            {
                format = context.useHighPrecision ? RenderTextureFormat.R16 : RenderTextureFormat.R8;
            }

            if (targetRT != null)
            {
                if (targetRT.width != context.baseResolution.x ||
                    targetRT.height != context.baseResolution.y|| 
                    targetRT.format != format)
                {
                    targetRT.Release();
                    TUtilities.DestroyObject(targetRT);
                    targetRT = null;
                }
            }

            if (targetRT == null)
            {
                targetRT = new RenderTexture(context.baseResolution.x, context.baseResolution.y, 0, format, RenderTextureReadWrite.Linear);
                targetRT.wrapMode = TextureWrapMode.Clamp;
            }
        }

        public static void PrepareShaderAndMaterial(string shaderName, ref Shader shader, ref Material material)
        {
            if (shader == null)
            {
                shader = Shader.Find(shaderName);
            }
            if (material == null || material.shader != shader)
            {
                material = new Material(shader);
            }
        }
    }
}
