using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.Poseidon
{
    //[CreateAssetMenu(menuName = "Poseidon/Settings")]
    public class PPoseidonSettings : ScriptableObject
    {
        private static PPoseidonSettings instance;
        public static PPoseidonSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PPoseidonSettings>("PoseidonSettings");
                    if (instance == null)
                    {
                        instance = ScriptableObject.CreateInstance<PPoseidonSettings>();
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        private PWaterProfile calmWaterProfile;
        public PWaterProfile CalmWaterProfile
        {
            get
            {
                return calmWaterProfile;
            }
            set
            {
                calmWaterProfile = value;
            }
        }

        [SerializeField]
        private PWaterProfile calmWaterHQProfile;
        public PWaterProfile CalmWaterHQProfile
        {
            get
            {
                return calmWaterHQProfile;
            }
            set
            {
                calmWaterHQProfile = value;
            }
        }

        [SerializeField]
        private Texture2D noiseTexture;
        public Texture2D NoiseTexture
        {
            get
            {
                return noiseTexture;
            }
            set
            {
                noiseTexture = value;
            }
        }

        [SerializeField]
        private Texture2D defaultNormalMap;
        public Texture2D DefaultNormalMap
        {
            get
            {
                return defaultNormalMap;
            }
            set
            {
                defaultNormalMap = value;
            }
        }

        [SerializeField]
        private Texture2D defaultUnderwaterDistortionMap;
        public Texture2D DefaultUnderwaterDistortionMap
        {
            get
            {
                return defaultUnderwaterDistortionMap;
            }
            set
            {
                defaultUnderwaterDistortionMap = value;
            }
        }

        [SerializeField]
        private Texture2D defaultWetLensDistortionMap;
        public Texture2D DefaultWetLensDistortionMap
        {
            get
            {
                return defaultWetLensDistortionMap;
            }
            set
            {
                defaultWetLensDistortionMap = value;
            }
        }

#if UNITY_POST_PROCESSING_STACK_V2
        [SerializeField]
        private Shader underwaterShader;
        public Shader UnderwaterShader
        {
            get
            {
                if (underwaterShader == null)
                {
                    underwaterShader = Shader.Find("Hidden/Poseidon/Underwater");
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
                return underwaterShader;
            }
            set
            {
                underwaterShader = value;
            }
        }

        [SerializeField]
        private Shader wetLensShader;
        public Shader WetLensShader
        {
            get
            {
                if (wetLensShader == null)
                {
                    wetLensShader = Shader.Find("Hidden/Poseidon/WetLens");
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
                return wetLensShader;
            }
            set
            {
                wetLensShader = value;
            }
        }
#endif
#if POSEIDON_URP
        [SerializeField]
        private Shader underwaterShaderURP;
        public Shader UnderwaterShaderURP
        {
            get
            {
                if (underwaterShaderURP == null)
                {
                    underwaterShaderURP = Shader.Find("Hidden/Poseidon/UnderwaterURP");
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
                return underwaterShaderURP;
            }
            set
            {
                underwaterShaderURP = value;
            }
        }

        [SerializeField]
        private Shader wetLensShaderURP;
        public Shader WetLensShaderURP
        {
            get
            {
                if (wetLensShaderURP == null)
                {
                    wetLensShaderURP = Shader.Find("Hidden/Poseidon/WetLensURP");
#if UNITY_EDITOR
                    EditorUtility.SetDirty(this);
#endif
                }
                return wetLensShaderURP;
            }
            set
            {
                wetLensShaderURP = value;
            }
        }
#endif

        [SerializeField]
        private PInternalShaderSettings internalShaders;
        public PInternalShaderSettings InternalShaders
        {
            get
            {
                return internalShaders;
            }
            set
            {
                internalShaders = value;
            }
        }

    }
}
