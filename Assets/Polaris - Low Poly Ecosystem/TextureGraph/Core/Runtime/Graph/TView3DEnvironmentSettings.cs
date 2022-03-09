using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    [System.Serializable]
    public class TView3DEnvironmentSettings
    {
        public class TConst
        {
            public static readonly int LIGHT_COLOR_0 = Shader.PropertyToID("_Light0_Color");
            public static readonly int LIGHT_INTENSITY_0 = Shader.PropertyToID("_Light0_Intensity");
            public static readonly int LIGHT_DIRECTION_0 = Shader.PropertyToID("_Light0_Direction");

            public static readonly int LIGHT_COLOR_1 = Shader.PropertyToID("_Light1_Color");
            public static readonly int LIGHT_INTENSITY_1 = Shader.PropertyToID("_Light1_Intensity");
            public static readonly int LIGHT_DIRECTION_1 = Shader.PropertyToID("_Light1_Direction");

            public static readonly int TESSELLATION_LEVEL = Shader.PropertyToID("_TessellationLevel");
            public static readonly int DISPLACEMENT_STRENGTH = Shader.PropertyToID("_DisplacementStrength");
            public static readonly int ALBEDO_MAP = Shader.PropertyToID("_AlbedoMap");
            public static readonly int HEIGHT_MAP = Shader.PropertyToID("_HeightMap");
            public static readonly int NORMAL_MAP = Shader.PropertyToID("_NormalMap");
        }

        [System.Serializable]
        public class TLightData
        {
            [SerializeField]
            private Color lightColor;
            public Color LightColor
            {
                get
                {
                    return lightColor;
                }
                set
                {
                    lightColor = value;
                }
            }

            [SerializeField]
            private float intensity;
            public float Intensity
            {
                get
                {
                    return intensity;
                }
                set
                {
                    intensity = Mathf.Max(0, value);
                }
            }

            [SerializeField]
            private Vector3 direction;
            public Vector3 Direction
            {
                get
                {
                    return direction;
                }
                set
                {
                    direction = value.normalized;
                }
            }

            public TLightData()
            {
                LightColor = Color.white;
                Intensity = 1;
                Direction = new Vector3(1, 1, 0);
            }
        }

        [SerializeField]
        private TLightData light0;
        public TLightData Light0
        {
            get
            {
                return light0;
            }
            set
            {
                light0 = value;
            }
        }

        [SerializeField]
        private TLightData light1;
        public TLightData Light1
        {
            get
            {
                return light1;
            }
            set
            {
                light1 = value;
            }
        }

        [SerializeField]
        private float lightAngle;
        public float LightAngle
        {
            get
            {
                return lightAngle;
            }
            set
            {
                lightAngle = value;
            }
        }

        [SerializeField]
        private float cameraDistance;
        public float CameraDistance
        {
            get
            {
                return cameraDistance;
            }
            set
            {
                cameraDistance = value;
            }
        }

        [SerializeField]
        private Vector3 cameraAngle;
        public Vector3 CameraAngle
        {
            get
            {
                return cameraAngle;
            }
            set
            {
                cameraAngle = value;
            }
        }

        [SerializeField]
        private TPremitives.TMeshType meshType;
        public TPremitives.TMeshType MeshType
        {
            get
            {
                return meshType;
            }
            set
            {
                meshType = value;
            }
        }

        [SerializeField]
        private Mesh customMesh;
        public Mesh CustomMesh
        {
            get
            {
                return customMesh;
            }
            set
            {
                customMesh = value;
            }
        }

        [SerializeField]
        private int tessellationLevel;
        public int TessellationLevel
        {
            get
            {
                return tessellationLevel;
            }
            set
            {
                tessellationLevel = Mathf.Clamp(value, 1, 64);
            }
        }

        [SerializeField]
        private float displacementStrength;
        public float DisplacementStrength
        {
            get
            {
                return displacementStrength;
            }
            set
            {
                displacementStrength = Mathf.Clamp(value, 0, 1);
            }
        }

        public TView3DEnvironmentSettings()
        {
            light0 = new TLightData()
            {
                LightColor = new Color32(255, 255, 255, 255),
                Intensity = 0.85f,
                Direction = new Vector3(-1, -1, 0)
            };
            light1 = new TLightData()
            {
                LightColor = new Color32(120, 90, 70, 255),
                Intensity = 0.15f,
                Direction = new Vector3(1, 1, 0)
            };
            MeshType = TPremitives.TMeshType.Plane;
            TessellationLevel = 16;
            DisplacementStrength = 1;

            CameraDistance = 1;
            CameraAngle = new Vector3(30, 0, 0);
        }

        public void Setup(Material mat, TGraph graph)
        {
            Matrix4x4 lightRotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, lightAngle, 0));
            mat.SetColor(TConst.LIGHT_COLOR_0, Light0.LightColor);
            mat.SetFloat(TConst.LIGHT_INTENSITY_0, Light0.Intensity);
            mat.SetVector(TConst.LIGHT_DIRECTION_0, lightRotationMatrix.MultiplyVector(Light0.Direction));

            mat.SetColor(TConst.LIGHT_COLOR_1, Light1.LightColor);
            mat.SetFloat(TConst.LIGHT_INTENSITY_1, Light1.Intensity);
            mat.SetVector(TConst.LIGHT_DIRECTION_1, lightRotationMatrix.MultiplyVector(Light1.Direction));

            if (graph != null)
            {
                mat.SetInt(TConst.TESSELLATION_LEVEL, TessellationLevel);
                mat.SetFloat(TConst.DISPLACEMENT_STRENGTH, DisplacementStrength);

                mat.SetTexture(TConst.ALBEDO_MAP, null);
                mat.SetTexture(TConst.HEIGHT_MAP, null);
                mat.SetTexture(TConst.NORMAL_MAP, null);

                List<TOutputNode> outputNodes = graph.GraphData.GetNodeOfType<TOutputNode>();
                foreach (TOutputNode o in outputNodes)
                {
                    RenderTexture rt = graph.GetMainRT(o.GUID);
                    if (rt == null)
                        continue;
                    TTextureUsage usage = o.Usage.value;
                    if (usage == TTextureUsage.Albedo)
                    {
                        mat.SetTexture(TConst.ALBEDO_MAP, rt);
                    }
                    else if (usage == TTextureUsage.Height)
                    {
                        mat.SetTexture(TConst.HEIGHT_MAP, rt);
                    }
                    else if (usage == TTextureUsage.Normal)
                    {
                        mat.SetTexture(TConst.NORMAL_MAP, rt);
                    }
                }
            }
        }

        public float GetCameraDistance()
        {
            return cameraDistance;
        }

        public void SetCameraDistance(float d)
        {
            cameraDistance = d;
        }

        public void RotateCamera(Vector2 dragOffset)
        {
            cameraAngle.x += dragOffset.y;
            cameraAngle.x = Mathf.Clamp(cameraAngle.x, -90, 90);

            cameraAngle.y -= dragOffset.x;
            cameraAngle.z = 0;
        }

        public Mesh GetMesh()
        {
            if (MeshType == TPremitives.TMeshType.Custom)
            {
                return CustomMesh;
            }
            else
            {
                return TPremitives.Get(MeshType);
            }
        }

        public Vector3 GetCameraPosition()
        {
            Matrix4x4 rotateMatrix = Matrix4x4.Rotate(Quaternion.Euler(cameraAngle));
            Vector3 dir = rotateMatrix.MultiplyVector(-Vector3.forward);
            Vector3 pos = dir * CameraDistance;
            return pos;
        }

        public Quaternion GetCameraRotation()
        {
            return Quaternion.Euler(cameraAngle);
        }

        public void RotateLight(Vector2 dragOffset)
        {
            lightAngle -= dragOffset.x;
        }
    }
}
