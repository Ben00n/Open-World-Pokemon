#if UNITY_EDITOR && GRIFFIN
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.Poseidon.GriffinExtension
{
    //[CreateAssetMenu(menuName = "Poseidon/Water Quick Setup Config")]
    public class PWaterQuickSetupConfig : ScriptableObject
    {
        private static PWaterQuickSetupConfig instance;
        public static PWaterQuickSetupConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<PWaterQuickSetupConfig>("WaterQuickSetupConfig");
                    if (instance == null)
                    {
                        instance = ScriptableObject.CreateInstance<PWaterQuickSetupConfig>();
                    }
                }
                return instance;
            }
        }

        [SerializeField]
        private float waterLevel;
        public float WaterLevel
        {
            get
            {
                return waterLevel;
            }
            set
            {
                waterLevel = value;
            }
        }

        [SerializeField]
        private int meshResolution;
        public int MeshResolution
        {
            get
            {
                return meshResolution;
            }
            set
            {
                meshResolution = Mathf.Clamp(value, 2, 100);
                if (meshResolution % 2 == 1)
                {
                    meshResolution -= 1;
                }
            }
        }

        [SerializeField]
        private float meshNoise;
        public float MeshNoise
        {
            get
            {
                return meshNoise;
            }
            set
            {
                meshNoise = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float approximateSizeX;
        public float ApproximateSizeX
        {
            get
            {
                return approximateSizeX;
            }
            set
            {
                approximateSizeX = Mathf.Max(0, value);
            }
        }

        [SerializeField]
        private float approximateSizeZ;
        public float ApproximateSizeZ
        {
            get
            {
                return approximateSizeZ;
            }
            set
            {
                approximateSizeZ = Mathf.Max(0, value);
            }
        }
        
        [SerializeField]
        private Vector2 tileSize;
        public Vector2 TileSize
        {
            get
            {
                return tileSize;
            }
            set
            {
                tileSize = new Vector2(Mathf.Max(1, value.x), Mathf.Max(1, value.y));
            }
        }
    }
}
#endif
