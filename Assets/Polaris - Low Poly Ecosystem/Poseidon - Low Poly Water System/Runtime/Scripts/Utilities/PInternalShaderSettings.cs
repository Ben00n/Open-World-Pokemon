using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon
{
    [System.Serializable]
    public struct PInternalShaderSettings
    {
        [SerializeField]
        private Shader copyTextureShader;
        public Shader CopyTextureShader
        {
            get
            {
                return copyTextureShader;
            }
            set
            {
                copyTextureShader = value;
            }
        }

        [SerializeField]
        private Shader solidColorShader;
        public Shader SolidColorShader
        {
            get
            {
                return solidColorShader;
            }
            set
            {
                solidColorShader = value;
            }
        }


    }
}
