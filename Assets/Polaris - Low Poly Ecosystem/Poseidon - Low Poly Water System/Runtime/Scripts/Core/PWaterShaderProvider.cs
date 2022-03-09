using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon
{
    public static class PWaterShaderProvider
    {
        private static Shader waterBasicShader;
        public static Shader WaterBasicShader
        {
            get
            {
                if (waterBasicShader == null)
                {
                    waterBasicShader = Shader.Find("Poseidon/Default/WaterBasic");
                }
                return waterBasicShader;
            }
        }

        private static Shader waterAdvancedShader;
        public static Shader WaterAdvancedShader
        {
            get
            {
                if (waterAdvancedShader == null)
                {
                    waterAdvancedShader = Shader.Find("Poseidon/Default/WaterAdvanced");
                }
                return waterAdvancedShader;
            }
        }

        private static Shader waterBasicShaderURP;
        public static Shader WaterBasicShaderURP
        {
            get
            {
                if (waterBasicShaderURP == null)
                {
                    waterBasicShaderURP = Shader.Find("Poseidon/URP/WaterBasicURP");
                }
                return waterBasicShaderURP;
            }
        }

        private static Shader waterAdvancedShaderURP;
        public static Shader WaterAdvancedShaderURP
        {
            get
            {
                if (waterAdvancedShaderURP == null)
                {
                    waterAdvancedShaderURP = Shader.Find("Poseidon/URP/WaterAdvancedURP");
                }
                return waterAdvancedShaderURP;
            }
        }

        private static Shader waterBackFaceShader;
        public static Shader WaterBackFaceShader
        {
            get
            {
                if (waterBackFaceShader == null)
                {
                    waterBackFaceShader = Shader.Find("Poseidon/Default/WaterBackFace");
                }
                return waterBackFaceShader;
            }
        }

        private static Shader waterBackFaceShaderURP;
        public static Shader WaterBackFaceShaderURP
        {
            get
            {
                if (waterBackFaceShaderURP == null)
                {
                    waterBackFaceShaderURP = Shader.Find("Poseidon/URP/WaterBackFaceURP");
                }
                return waterBackFaceShaderURP;
            }
        }

        private static Shader riverShader;
        public static Shader RiverShader
        {
            get
            {
                if (riverShader == null)
                {
                    riverShader = Shader.Find("Poseidon/Default/River");
                }
                return riverShader;
            }
        }

        private static Shader riverShaderURP;
        public static Shader RiverShaderURP
        {
            get
            {
                if (riverShaderURP == null)
                {
                    riverShaderURP = Shader.Find("Poseidon/URP/RiverURP");
                }
                return riverShaderURP;
            }
        }

        public static Shader GetBackFaceShader()
        {
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                return WaterBackFaceShader;
            }
            else if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                return WaterBackFaceShaderURP;
            }
            else
            {
                return null;
            }
        }

        public static Shader GetShader(PWater water)
        {
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                return GetBuiltinRPShader(water);
            }
            else if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                return GetUniversalRPShader(water);
            }
            else
            {
                return null;
            }
        }

        public static Shader GetBuiltinRPShader(PWater water)
        {
            if (water.Profile == null)
                return null;
            if (water.MeshType == PWaterMeshType.Spline)
            {
                return RiverShader;
            }
            else
            {
                if (water.Profile.EnableReflection || water.Profile.EnableRefraction)
                {
                    return WaterAdvancedShader;
                }
                else
                {
                    return WaterBasicShader;
                }
            }
        }

        public static Shader GetUniversalRPShader(PWater water)
        {
            if (water.Profile == null)
                return null;
            if (water.MeshType == PWaterMeshType.Spline)
            {
                return RiverShaderURP;
            }
            else
            {
                if (water.Profile.EnableReflection || water.Profile.EnableRefraction)
                {
                    return WaterAdvancedShaderURP;
                }
                else
                {
                    return WaterBasicShaderURP;
                }
            }
        }

        public static bool Validate(PWater water)
        {
            bool validate = true;
            bool validateBackFace = true;
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                validate =
                    water.MaterialToRender.shader == WaterBasicShader ||
                    water.MaterialToRender.shader == WaterAdvancedShader ||
                    water.MaterialToRender.shader == RiverShader;
                validateBackFace =
                    water.ShouldRenderBackface && water.MaterialBackFace.shader == WaterBackFaceShader;
            }
            else if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
                validate =
                    water.MaterialToRender.shader == WaterBasicShaderURP ||
                    water.MaterialToRender.shader == WaterAdvancedShaderURP ||
                    water.MaterialToRender.shader == RiverShaderURP;
                validateBackFace =
                     water.ShouldRenderBackface && water.MaterialBackFace.shader == WaterBackFaceShaderURP;
            }

            return validate && validateBackFace;
        }

        public static void ResetShaderReferences()
        {
            waterBasicShader = null;
            waterAdvancedShader = null;
            waterBackFaceShader = null;
            riverShader = null;

            waterBasicShaderURP = null;
            waterAdvancedShaderURP = null;
            waterBackFaceShaderURP = null;
            riverShaderURP = null;
        }
    }
}
