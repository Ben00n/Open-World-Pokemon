using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
#if POSEIDON_URP
using UnityEngine.Rendering.Universal;
#endif
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
#endif
using StopWatch = System.Diagnostics.Stopwatch;

namespace Pinwheel.Poseidon
{
    [ExecuteInEditMode]
    public class PWater : MonoBehaviour
    {
        [SerializeField]
        private PWaterProfile profile;
        public PWaterProfile Profile
        {
            get
            {
                return profile;
            }
            set
            {
                profile = value;
            }
        }

        [SerializeField]
        private Material materialToRender;
        public Material MaterialToRender
        {
            get
            {
                if (Profile == null)
                    return null;
                if (materialToRender == null)
                {
                    Shader shader = PWaterShaderProvider.GetShader(this);
                    materialToRender = new Material(shader);
                    Profile.UpdateMaterialProperties(materialToRender);
                }
                materialToRender.name = materialToRender.shader.name;
                return materialToRender;
            }
        }

        [SerializeField]
        private Material materialBackFace;
        public Material MaterialBackFace
        {
            get
            {
                if (Profile == null)
                    return null;
                if (materialBackFace == null)
                {
                    Shader shader = PWaterShaderProvider.GetBackFaceShader();
                    materialBackFace = new Material(shader);
                    Profile.UpdateMaterialProperties(materialBackFace);
                }
                materialBackFace.name = materialBackFace.shader.name;
                return materialBackFace;
            }
        }

        [SerializeField]
        private PWaterMeshType meshType;
        public PWaterMeshType MeshType
        {
            get
            {
                return meshType;
            }
            set
            {
                PWaterMeshType oldValue = meshType;
                PWaterMeshType newValue = value;
                meshType = newValue;
                if (oldValue != newValue)
                {

                }
            }
        }

        [SerializeField]
        private PPlaneMeshPattern planePattern;
        public PPlaneMeshPattern PlanePattern
        {
            get
            {
                return planePattern;
            }
            set
            {
                planePattern = value;
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
        private Mesh sourceMesh;
        public Mesh SourceMesh
        {
            get
            {
                return sourceMesh;
            }
            set
            {
                Mesh oldValue = sourceMesh;
                Mesh newValue = value;
                sourceMesh = newValue;
                if (oldValue != newValue)
                {
                    BakeCustomMesh();
                }
            }
        }

        [SerializeField]
        private Mesh mesh;
        public Mesh Mesh
        {
            get
            {
                if (mesh == null)
                {
                    mesh = new Mesh();
                    mesh.MarkDynamic();
                    GeneratePlaneMesh();
                }

                return mesh;
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

        [SerializeField]
        private List<PIndex2D> tileIndices;
        public List<PIndex2D> TileIndices
        {
            get
            {
                if (tileIndices == null)
                {
                    tileIndices = new List<PIndex2D>();
                }
                if (tileIndices.Count == 0)
                {
                    tileIndices.Add(new PIndex2D(0, 0));
                }
                return tileIndices;
            }
            set
            {
                tileIndices = value;
            }
        }

        [SerializeField]
        private bool tilesFollowMainCamera;
        public bool TilesFollowMainCamera
        {
            get
            {
                return tilesFollowMainCamera;
            }
            set
            {
                tilesFollowMainCamera = value;
            }
        }

        [SerializeField]
        private List<Vector3> areaMeshAnchors;
        public List<Vector3> AreaMeshAnchors
        {
            get
            {
                if (areaMeshAnchors == null)
                {
                    areaMeshAnchors = new List<Vector3>();
                }
                return areaMeshAnchors;
            }
            set
            {
                areaMeshAnchors = value;
            }
        }

        [SerializeField]
        private PSpline spline;
        public PSpline Spline
        {
            get
            {
                if (spline == null)
                {
                    spline = new PSpline();
                }
                return spline;
            }
            set
            {
                spline = value;
            }
        }

        [SerializeField]
        private int splineResolutionX;
        public int SplineResolutionX
        {
            get
            {
                return splineResolutionX;
            }
            set
            {
                splineResolutionX = Mathf.Clamp(value, 2, 100);
                if (splineResolutionX % 2 == 1)
                {
                    splineResolutionX -= 1;
                }
            }
        }

        [SerializeField]
        private int splineResolutionY;
        public int SplineResolutionY
        {
            get
            {
                return splineResolutionY;
            }
            set
            {
                splineResolutionY = Mathf.Clamp(value, 2, 100);
                if (splineResolutionY % 2 == 1)
                {
                    splineResolutionY -= 1;
                }
            }
        }

        [SerializeField]
        private float splineWidth;
        public float SplineWidth
        {
            get
            {
                return splineWidth;
            }
            set
            {
                splineWidth = Mathf.Max(0, value);
            }
        }

        private Dictionary<Camera, RenderTexture> reflRenderTextures;
        private Dictionary<Camera, RenderTexture> ReflRenderTextures
        {
            get
            {
                if (reflRenderTextures == null)
                {
                    reflRenderTextures = new Dictionary<Camera, RenderTexture>();
                }
                return reflRenderTextures;
            }
        }

        private Dictionary<Camera, Camera> reflCameras;
        private Dictionary<Camera, Camera> ReflCameras
        {
            get
            {
                if (reflCameras == null)
                {
                    reflCameras = new Dictionary<Camera, Camera>();
                }
                return reflCameras;
            }
        }

        private MaterialPropertyBlock materialProperties;
        private MaterialPropertyBlock MaterialProperties
        {
            get
            {
                if (materialProperties == null)
                {
                    materialProperties = new MaterialPropertyBlock();
                }
                return materialProperties;
            }
        }

        //private List<GameObject> obsoletedGameObject;
        //private List<GameObject> ObsoletedGameObject
        //{
        //    get
        //    {
        //        if (obsoletedGameObject == null)
        //        {
        //            obsoletedGameObject = new List<GameObject>();
        //        }
        //        return obsoletedGameObject;
        //    }
        //}

        [SerializeField]
        private MeshFilter meshFilterComponent;
        private MeshFilter MeshFilterComponent
        {
            get
            {
                if (meshFilterComponent == null)
                {
                    meshFilterComponent = PUtilities.GetOrAddComponent<MeshFilter>(gameObject);
                }
                meshFilterComponent.hideFlags = HideFlags.HideInInspector;
                return meshFilterComponent;
            }
        }

        [SerializeField]
        private MeshRenderer meshRendererComponent;
        private MeshRenderer MeshRendererComponent
        {
            get
            {
                if (meshRendererComponent == null)
                {
                    meshRendererComponent = PUtilities.GetOrAddComponent<MeshRenderer>(gameObject);
                }
                meshRendererComponent.hideFlags = HideFlags.HideInInspector;
                return meshRendererComponent;
            }
        }

        private Mesh emptyMesh;
        public Mesh EmptyMesh
        {
            get
            {
                if (emptyMesh == null)
                {
                    emptyMesh = new Mesh();
                }
                return emptyMesh;
            }
        }

        private Material[] emptyMaterials;
        public Material[] EmptyMaterials
        {
            get
            {
                if (emptyMaterials == null)
                {
                    emptyMaterials = new Material[0];
                }
                return emptyMaterials;
            }
        }

        [SerializeField]
        private Bounds bounds;
        public Bounds Bounds
        {
            get
            {
                return bounds;
            }
            set
            {
                bounds = value;
                EmptyMesh.bounds = bounds;
            }
        }

        [SerializeField]
        private bool shouldRenderBackFace;
        public bool ShouldRenderBackface
        {
            get
            {
                return shouldRenderBackFace;
            }
            set
            {
                shouldRenderBackFace = value;
            }
        }

        private bool isMsaaWarningLogged = false;

        [SerializeField]
        private float meshVersion = 0;

        private const float MESH_VERSION_SERIALIZED_IN_WATER_COMPONENT = 130;

        private StopWatch timer;
        private double lastElapsedSeconds;
        private double frameTime;

        [SerializeField]
        private PTimeMode timeMode;
        public PTimeMode TimeMode
        {
            get
            {
                return timeMode;
            }
            set
            {
                timeMode = value;
            }
        }

        [SerializeField]
        private double manualTimeSeconds;
        public double ManualTimeSeconds
        {
            get
            {
                return manualTimeSeconds;
            }
            set
            {
                manualTimeSeconds = value;
            }
        }

        [SerializeField]
        private bool useWaveMask;
        public bool UseWaveMask
        {
            get
            {
                return useWaveMask;
            }
            set
            {
                useWaveMask = value;
            }
        }

        [SerializeField]
        private Texture2D waveMask;
        public Texture2D WaveMask
        {
            get
            {
                return waveMask;
            }
            set
            {
                waveMask = value;
            }
        }

        [SerializeField]
        private Rect waveMaskBounds;
        public Rect WaveMaskBounds
        {
            get
            {
                return waveMaskBounds;
            }
            set
            {
                waveMaskBounds = value;
            }
        }

        private void Reset()
        {
            MeshType = PWaterMeshType.TileablePlane;
            PlanePattern = PPlaneMeshPattern.Hexagon;
            MeshResolution = 100;
            SplineResolutionX = 10;
            SplineResolutionY = 20;
            SplineWidth = 5;
            MeshNoise = 0;
            TileSize = new Vector2(50, 50);
            TilesFollowMainCamera = false;
            TimeMode = PTimeMode.Auto;
            ManualTimeSeconds = 0;
            UseWaveMask = false;
            WaveMask = null;
            WaveMaskBounds = new Rect(0, 0, 100, 100);
            GeneratePlaneMesh();
        }

        private void OnEnable()
        {
            Camera.onPreCull += OnPreCullCamera;

#if POSEIDON_URP
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRenderingSRP;
#endif

            //foreach (Transform child in transform)
            //{
            //    if (child.name.StartsWith("~ReflectionCamera") || child.name.StartsWith("~RefractionCamera"))
            //    {
            //        ObsoletedGameObject.Add(child.gameObject);
            //    }
            //}

#if UNITY_EDITOR
            CheckMeshVersion();
#endif

            ReCalculateBounds();
            UpdateMaterial();

            timer = new StopWatch();
            timer.Start();
        }

        private void OnDisable()
        {
            Camera.onPreCull -= OnPreCullCamera;

#if POSEIDON_URP
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRenderingSRP;
#endif
        }

        private void OnDestroy()
        {
            CleanUp();
        }

        private void CleanUp()
        {
            foreach (RenderTexture rt in ReflRenderTextures.Values)
            {
                if (rt == null)
                    continue;
                rt.Release();
                PUtilities.DestroyObject(rt);
            }

            foreach (Camera cam in ReflCameras.Values)
            {
                if (cam == null)
                    continue;
                PUtilities.DestroyGameobject(cam.gameObject);
            }

            if (mesh != null)
            {
                PUtilities.DestroyObject(mesh);
            }

            if (spline != null)
            {
                spline.Dispose();
            }
        }

        private void Update()
        {
            //for (int i = 0; i < ObsoletedGameObject.Count; ++i)
            //{
            //    GameObject o = ObsoletedGameObject[i];
            //    if (o != null)
            //    {
            //        PUtilities.DestroyGameobject(o);
            //    }
            //}
            //ObsoletedGameObject.Clear();

            SetUpSelfLayer();
            SetUpDummyComponents();

            if (MeshType == PWaterMeshType.TileablePlane && TilesFollowMainCamera && Camera.main != null)
            {
                SnapPosition(Camera.main.transform.position);
            }
        }

        public void SnapPosition(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / TileSize.x);
            int z = Mathf.FloorToInt(worldPos.z / TileSize.y);
            float snapX = x * TileSize.x;
            float snapZ = z * TileSize.y;
            Vector3 newPos = new Vector3(snapX, transform.position.y, snapZ);
            transform.position = newPos;
        }

        private void SetUpSelfLayer()
        {
            int waterLayer = LayerMask.NameToLayer("Water");
            if (gameObject.layer != waterLayer)
            {
                gameObject.layer = LayerMask.NameToLayer("Water");
                //string msg = string.Format("Game object '{0}' must be in 'Water' layer!", gameObject.name);
                //Debug.Log(msg);
            }
        }

        private void SetUpDummyComponents()
        {
            if (Profile == null)
            {
                MeshRendererComponent.enabled = false;
            }
            else
            {
                MeshFilterComponent.sharedMesh = EmptyMesh;
                MeshRendererComponent.sharedMaterials = EmptyMaterials;
                MeshRendererComponent.shadowCastingMode = ShadowCastingMode.Off;
                MeshRendererComponent.receiveShadows = false;
                MeshRendererComponent.enabled = true;
            }
        }

        public void ReCalculateBounds()
        {
            if (MeshType == PWaterMeshType.Area || MeshType == PWaterMeshType.CustomMesh)
            {
                Bounds = Mesh.bounds;
            }
            else if (MeshType == PWaterMeshType.TileablePlane)
            {
                int minX = int.MaxValue;
                int minZ = int.MaxValue;
                int maxX = int.MinValue;
                int maxZ = int.MinValue;

                for (int i = 0; i < TileIndices.Count; ++i)
                {
                    PIndex2D index = TileIndices[i];
                    minX = Mathf.Min(minX, index.X);
                    minZ = Mathf.Min(minZ, index.Z);
                    maxX = Mathf.Max(maxX, index.X);
                    maxZ = Mathf.Max(maxZ, index.Z);
                }

                float width = (maxX - minX + 1) * TileSize.x;
                float length = (maxZ - minZ + 1) * TileSize.y;
                float height = 0;

                float centerX = Mathf.Lerp(minX, maxX + 1, 0.5f) * TileSize.x;
                float centerZ = Mathf.Lerp(minZ, maxZ + 1, 0.5f) * TileSize.y;
                float centerY = 0;

                Bounds = new Bounds(
                    new Vector3(centerX, centerY, centerZ),
                    new Vector3(width, height, length));
            }
            else if (MeshType == PWaterMeshType.Spline)
            {
                List<PSplineSegment> segments = Spline.Segments;
                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float minZ = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                float maxZ = float.MinValue;

                for (int i = 0; i < segments.Count; ++i)
                {
                    Bounds b = segments[i].Mesh.bounds;
                    minX = Mathf.Min(b.min.x, minX);
                    minY = Mathf.Min(b.min.y, minY);
                    minZ = Mathf.Min(b.min.z, minZ);

                    maxX = Mathf.Max(b.max.x, maxX);
                    maxY = Mathf.Max(b.max.y, maxY);
                    maxZ = Mathf.Max(b.max.z, maxZ);
                }

                bounds.SetMinMax(
                    new Vector3(minX, minY, minZ),
                    new Vector3(maxX, maxY, maxZ));
            }
        }

        private void OnPreCullCamera(Camera cam)
        {
            ValidateMaterial();
            SubmitRenderList(cam);
        }

        private void SubmitRenderList(Camera cam)
        {
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView)
                return;
            if (cam.name.StartsWith("~"))
                return;
            if (Profile == null)
                return;
#if UNITY_EDITOR
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                if (prefabStage.scene == cam.scene)
                    return;
            }
#endif

            PerformPreRenderCheck(cam);
            SetShaderParams();

            if (MeshType == PWaterMeshType.TileablePlane)
            {
                RenderTiledMesh(cam);
            }
            else if (MeshType == PWaterMeshType.Area || MeshType == PWaterMeshType.CustomMesh)
            {
                RenderSingleMesh(cam);
            }
            else if (MeshType == PWaterMeshType.Spline)
            {
                RenderSplineMesh(cam);
            }
        }

        private void PerformPreRenderCheck(Camera cam)
        {
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Builtin)
            {
                bool requireDepthTexture = Profile.EnableLightAbsorption || Profile.EnableFoam;
                if (requireDepthTexture)
                {
                    cam.depthTextureMode = DepthTextureMode.Depth | DepthTextureMode.MotionVectors;
                }

                bool msaaEnableInQualitySettings = QualitySettings.antiAliasing >= 2;
                bool msaaEnableOnCamera = cam.allowMSAA;
                if (msaaEnableInQualitySettings && msaaEnableOnCamera)
                {
                    if (Profile.EnableFoam && !isMsaaWarningLogged)
                    {
                        isMsaaWarningLogged = true;
                        Debug.LogWarning("MSAA causes Foam artifact, consider disabling it in the Quality Settings.");
                    }
                }
            }
            else if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
            {
#if POSEIDON_URP
                UniversalRenderPipelineAsset uAsset = UniversalRenderPipeline.asset;
                bool requireDepthTexture = Profile.EnableLightAbsorption || Profile.EnableFoam || Profile.EnableCaustic;
                if (requireDepthTexture)
                {
                    cam.depthTextureMode = DepthTextureMode.Depth;
                    if (uAsset.supportsCameraDepthTexture == false)
                    {
                        uAsset.supportsCameraDepthTexture = true;
                        Debug.Log("Enabling Depth Texture in URP Asset for Light Absorption, Foam and Caustic.");
                    }
                }

                bool requireOpaqueTexture = Profile.EnableRefraction;
                if (requireOpaqueTexture)
                {
                    if (uAsset.supportsCameraOpaqueTexture == false)
                    {
                        uAsset.supportsCameraOpaqueTexture = true;
                        Debug.Log("Enabling Opaque Texture in URP Asset for Refraction.");
                    }
                }

                bool msaaEnabledInUAsset = uAsset.msaaSampleCount >= 2;
                bool msaaEnabledOnCamera = cam.allowMSAA;
                if (msaaEnabledInUAsset && msaaEnabledOnCamera)
                {
                    if (Profile.EnableFoam && !isMsaaWarningLogged)
                    {
                        isMsaaWarningLogged = true;
                        Debug.LogWarning("MSAA causes Foam artifact, consider disabling it in the URP Asset.");
                    }
                }
#endif
            }
        }

        private void SetShaderParams()
        {
            Shader.SetGlobalFloat(PMat.TIME, (float)GetTimeParam());
            Shader.SetGlobalFloat(PMat.SINE_TIME, Mathf.Sin((float)GetTimeParam()));
        }

        private void RenderTiledMesh(Camera cam)
        {
            Vector3 localPos = this.transform.InverseTransformPoint(cam.transform.position);
            localPos.y = 0;
            localPos = this.GetLocalVertexPosition(localPos, false);
            Vector3 worldPos = this.transform.TransformPoint(localPos);
            float waterHeight = worldPos.y;

            bool isBackface = cam.transform.position.y < waterHeight;
            if (isBackface && !ShouldRenderBackface)
                return;

            MaterialProperties.Clear();
            if (Profile.EnableReflection)
            {
                MaterialProperties.SetTexture(PMat.REFLECTION_TEX, GetReflectionRt(cam));
            }
            MaterialProperties.SetTexture(PMat.NOISE_TEX, PPoseidonSettings.Instance.NoiseTexture);

            Material mat = isBackface ? MaterialBackFace : MaterialToRender;
            PMat.SetActiveMaterial(mat);
            PMat.SetKeywordEnable(PMat.KW_MESH_NOISE, meshNoise != 0);
            PMat.SetFloat(PMat.MESH_NOISE, meshNoise);
            PMat.SetFloat(PMat.TIME, (float)GetTimeParam());
            PMat.SetFloat(PMat.SINE_TIME, Mathf.Sin((float)GetTimeParam()));
            PMat.SetKeywordEnable(PMat.KW_WAVE_MASK, UseWaveMask);
            PMat.SetTexture(PMat.WAVE_MASK, WaveMask);
            PMat.SetVector(PMat.WAVE_MASK_BOUNDS, new Vector4(WaveMaskBounds.min.x, WaveMaskBounds.min.y, WaveMaskBounds.max.x, WaveMaskBounds.max.y));
            PMat.SetActiveMaterial(null);

            for (int i = 0; i < TileIndices.Count; ++i)
            {
                PIndex2D index = TileIndices[i];
                Vector3 pos = transform.TransformPoint(new Vector3(index.X * TileSize.x, 0, index.Z * TileSize.y));
                Quaternion rotation = transform.rotation;
                Vector3 scale = new Vector3(TileSize.x * transform.lossyScale.x, 1 * transform.lossyScale.y, TileSize.y * transform.lossyScale.z);

                Graphics.DrawMesh(
                    Mesh,
                    Matrix4x4.TRS(pos, rotation, scale),
                    mat,
                    gameObject.layer,
                    cam,
                    0,
                    MaterialProperties,
                    ShadowCastingMode.Off,
                    false,
                    null,
                    LightProbeUsage.BlendProbes,
                    null);
            }
        }

        private void RenderSingleMesh(Camera cam)
        {
            MaterialProperties.Clear();
            if (Profile.EnableReflection)
            {
                MaterialProperties.SetTexture(PMat.REFLECTION_TEX, GetReflectionRt(cam));
            }
            MaterialProperties.SetTexture(PMat.NOISE_TEX, PPoseidonSettings.Instance.NoiseTexture);

            PMat.SetActiveMaterial(MaterialToRender);
            PMat.SetKeywordEnable(PMat.KW_BACK_FACE, false);
            PMat.SetKeywordEnable(PMat.KW_MESH_NOISE, meshNoise != 0);
            PMat.SetFloat(PMat.MESH_NOISE, meshNoise);
            PMat.SetActiveMaterial(null);

            Graphics.DrawMesh(
                Mesh,
                transform.localToWorldMatrix,
                MaterialToRender,
                gameObject.layer,
                cam,
                0,
                MaterialProperties,
                ShadowCastingMode.Off,
                false,
                null,
                LightProbeUsage.BlendProbes,
                null);
        }

        private void RenderSplineMesh(Camera cam)
        {
            MaterialProperties.Clear();
            if (Profile.EnableReflection)
            {
                MaterialProperties.SetTexture(PMat.REFLECTION_TEX, GetReflectionRt(cam));
            }
            MaterialProperties.SetTexture(PMat.NOISE_TEX, PPoseidonSettings.Instance.NoiseTexture);

            PMat.SetActiveMaterial(MaterialToRender);
            PMat.SetKeywordEnable(PMat.KW_BACK_FACE, false);
            PMat.SetKeywordEnable(PMat.KW_MESH_NOISE, meshNoise != 0);
            PMat.SetFloat(PMat.MESH_NOISE, meshNoise);
            PMat.SetActiveMaterial(null);

            List<PSplineSegment> segments = Spline.Segments;
            for (int i = 0; i < segments.Count; ++i)
            {
                Graphics.DrawMesh(
                    segments[i].Mesh,
                    transform.localToWorldMatrix,
                    MaterialToRender,
                    gameObject.layer,
                    cam,
                    0,
                    MaterialProperties,
                    ShadowCastingMode.Off,
                    false,
                    null,
                    LightProbeUsage.BlendProbes,
                    null);
            }
        }

        private RenderTexture GetReflectionRt(Camera cam)
        {
            if (!ReflRenderTextures.ContainsKey(cam))
            {
                ReflRenderTextures.Add(cam, null);
            }

            int resolution = 128;
            if (Profile != null)
            {
                resolution = Profile.ReflectionTextureResolution;
            }
            RenderTexture rt = ReflRenderTextures[cam];
            if (rt == null)
            {
                rt = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32);
            }
            if (rt.width != resolution || rt.height != resolution)
            {
                Camera reflCam;
                if (ReflCameras.TryGetValue(cam, out reflCam))
                {
                    reflCam.targetTexture = null;
                }

                rt.Release();
                PUtilities.DestroyObject(rt);
                rt = new RenderTexture(resolution, resolution, 16, RenderTextureFormat.ARGB32);
            }
            rt.name = string.Format("~ReflectionRt_{0}_{1}", cam.name, resolution);
            ReflRenderTextures[cam] = rt;

            if (cam.stereoEnabled)
                rt.Release();

            return rt;
        }

        private void OnWillRenderObject()
        {
            if (PCommon.CurrentRenderPipeline == PRenderPipelineType.Universal)
                return;

            if (Profile == null)
                return;

            Camera currentCam = Camera.current;
            if (currentCam == null)
                return;
            if (currentCam.cameraType != CameraType.Game && currentCam.cameraType != CameraType.SceneView)
                return;
            if (currentCam.name.EndsWith("Preview Camera"))
                return;
            if (ReflCameras.ContainsValue(currentCam))
                return;

            if (Profile.EnableReflection)
            {
                RenderReflectionTexture(currentCam);
            }
        }

        private void RenderReflectionTexture(Camera cam)
        {
            if (MeshType == PWaterMeshType.Spline)
                return;

            bool isBackface = cam.transform.position.y < transform.position.y;
            if (isBackface)
                return;

            if (cam.stereoEnabled)
                return;

            //prepare reflection camera
            if (!ReflCameras.ContainsKey(cam))
            {
                ReflCameras.Add(cam, null);
            }
            if (ReflCameras[cam] == null)
            {
                GameObject g = new GameObject();
                g.name = "~ReflectionCamera_" + cam.name;
                g.hideFlags = HideFlags.HideAndDontSave;

                Camera rCam = g.AddComponent<Camera>();
                rCam.enabled = false;

                g.AddComponent<Skybox>();
                g.AddComponent<FlareLayer>();

                PUtilities.ResetTransform(g.transform, transform);
                ReflCameras[cam] = rCam;
            }

            //define reflection plane by position & normal in world space
            Vector3 planePos = transform.position;
            Vector3 planeNormal = Vector3.up;

            //disable pixel light if needed
            int oldPixelLightCount = QualitySettings.pixelLightCount;
            if (!Profile.EnableReflectionPixelLight)
            {
                QualitySettings.pixelLightCount = 0;
            }

            //set up camera and render
            Camera reflectionCamera = ReflCameras[cam];
            reflectionCamera.enabled = false;
            reflectionCamera.targetTexture = GetReflectionRt(cam);
            MatchCameraSettings(cam, reflectionCamera);

            // Reflect camera around reflection plane
            float d = -Vector3.Dot(planeNormal, planePos) - Profile.ReflectionClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, d);

            Matrix4x4 reflection = CalculateReflectionMatrix(reflectionPlane);
            Vector3 oldpos = cam.transform.position;
            Vector3 newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            bool isBackFace = cam.transform.position.y < transform.position.y;
            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, planePos, planeNormal, Profile.ReflectionClipPlaneOffset, isBackFace ? -1.0f : 1.0f);
            reflectionCamera.projectionMatrix = cam.CalculateObliqueMatrix(clipPlane);

            // Set custom culling matrix from the current camera
            reflectionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;

            reflectionCamera.cullingMask = ~(1 << 4) & Profile.ReflectionLayers.value; // never render water layer
            //reflectionCamera.targetTexture = m_ReflectionTexture;
            bool oldCulling = GL.invertCulling;
            GL.invertCulling = !oldCulling;
            reflectionCamera.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

            if (reflectionCamera.transform.rotation != Quaternion.identity)
            {
                reflectionCamera.Render();
            }

            reflectionCamera.transform.position = oldpos;
            GL.invertCulling = oldCulling;

            //restore pixel light
            if (!Profile.EnableReflectionPixelLight)
            {
                QualitySettings.pixelLightCount = oldPixelLightCount;
            }
        }

        private void MatchCameraSettings(Camera src, Camera dest)
        {
            if (dest == null)
            {
                return;
            }
            // set water camera to clear the same way as current camera
            dest.clearFlags = src.clearFlags;
            dest.backgroundColor = src.backgroundColor;
            if (src.clearFlags == CameraClearFlags.Skybox && Profile.ReflectCustomSkybox)
            {
                Skybox sky = src.GetComponent<Skybox>();
                Skybox mysky = dest.GetComponent<Skybox>();
                if (!sky || !sky.material)
                {
                    mysky.enabled = false;
                }
                else
                {
                    mysky.enabled = true;
                    mysky.material = sky.material;
                }
            }
            // update other values to match current camera.
            // even if we are supplying custom camera&projection matrices,
            // some of values are used elsewhere (e.g. skybox uses far plane)
            //dest.ResetWorldToCameraMatrix();
            dest.farClipPlane = src.farClipPlane;
            dest.nearClipPlane = src.nearClipPlane;
            dest.orthographic = src.orthographic;
            dest.fieldOfView = src.fieldOfView;
            dest.aspect = src.aspect;
            dest.orthographicSize = src.orthographicSize;
            dest.depthTextureMode = DepthTextureMode.None;
            dest.depth = float.MinValue;
            dest.stereoTargetEye = StereoTargetEyeMask.None;
            //dest.stereoSeparation = src.stereoSeparation;
            //dest.stereoConvergence = src.stereoConvergence;
            //dest.stereoTargetEye = src.stereoTargetEye;

            //if (src.stereoEnabled)
            //{
            //    if (src.stereoTargetEye == StereoTargetEyeMask.Left || src.stereoTargetEye == StereoTargetEyeMask.Both)
            //    {
            //        Vector3 eyePos = src.transform.TransformPoint(new Vector3(-0.5f * src.stereoSeparation, 0, 0));
            //        dest.transform.position = eyePos;

            //        Matrix4x4 projectionMatrix = src.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
            //        dest.projectionMatrix = projectionMatrix;
            //    }
            //    else if (src.stereoTargetEye == StereoTargetEyeMask.Right)
            //    {
            //        Vector3 eyePos = src.transform.TransformPoint(new Vector3(0.5f * src.stereoSeparation, 0, 0));
            //        dest.transform.position = eyePos;

            //        Matrix4x4 projectionMatrix = src.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
            //        dest.projectionMatrix = projectionMatrix;
            //    }
            //}
        }

        private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane, Camera.MonoOrStereoscopicEye eye = Camera.MonoOrStereoscopicEye.Mono)
        {
            Matrix4x4 reflectionMat = new Matrix4x4();

            reflectionMat.m00 = (1F - 2F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2F * plane[1] * plane[0]);
            reflectionMat.m11 = (1F - 2F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2F * plane[2] * plane[1]);
            reflectionMat.m22 = (1F - 2F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2F * plane[3] * plane[2]);

            reflectionMat.m30 = 0F;
            reflectionMat.m31 = 0F;
            reflectionMat.m32 = 0F;
            reflectionMat.m33 = 1F;

            //reflectionMat = Matrix4x4.Translate(new Vector3(float0, 0, 0))*reflectionMat ;

            return reflectionMat;
        }

        private static Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float clipPlaneOffset, float sideSign)
        {
            Vector3 offsetPos = pos + normal * clipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;
            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        private void ValidateMaterial()
        {
            bool validate = PWaterShaderProvider.Validate(this);
            if (!validate)
            {
                UpdateMaterial();
            }
        }

        public void UpdateMaterial()
        {
            if (Profile != null)
            {
                Shader shader = PWaterShaderProvider.GetShader(this);
                PMat.SetActiveMaterial(MaterialToRender);
                PMat.SetShader(shader);
                PMat.SetActiveMaterial(null);
                Profile.UpdateMaterialProperties(MaterialToRender);

                if (ShouldRenderBackface)
                {
                    Shader backFaceShader = PWaterShaderProvider.GetBackFaceShader();
                    PMat.SetActiveMaterial(MaterialBackFace);
                    PMat.SetShader(backFaceShader);
                    PMat.SetActiveMaterial(null);
                    Profile.UpdateMaterialProperties(MaterialBackFace);
                }
            }
        }

#if POSEIDON_URP
        private void OnBeginCameraRenderingSRP(ScriptableRenderContext context, Camera cam)
        {
            ValidateMaterial();
            SubmitRenderList(cam);

            if (Profile == null)
                return;
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView)
                return;
            if (cam.cameraType == CameraType.Game && cam != Camera.main)
                return;
            if (cam.name.EndsWith("Preview Camera"))
                return;
            if (ReflCameras.ContainsValue(cam))
            {
                //cam is a reflection camera
                return;
            }
            else
            {
                if (Profile.EnableReflection)
                {
                    RenderReflectionTextureSRP(context, cam);
                }
            }
        }

        private void RenderReflectionTextureSRP(ScriptableRenderContext context, Camera cam)
        {
            if (MeshType == PWaterMeshType.Spline)
                return;

            bool isBackface = cam.transform.position.y < transform.position.y;
            if (isBackface)
                return;

            if (cam.stereoEnabled)
                return;

            //prepare reflection camera
            if (!ReflCameras.ContainsKey(cam))
            {
                ReflCameras.Add(cam, null);
            }
            if (ReflCameras[cam] == null)
            {
                GameObject g = new GameObject();
                g.name = "~ReflectionCamera_" + cam.name;
                g.hideFlags = HideFlags.HideAndDontSave;

                Camera rCam = g.AddComponent<Camera>();
                rCam.enabled = false;

                g.AddComponent<Skybox>();
                g.AddComponent<FlareLayer>();

                PUtilities.ResetTransform(g.transform, transform);
                ReflCameras[cam] = rCam;
            }

            //define reflection plane by position & normal in world space
            Vector3 planePos = transform.position;
            Vector3 planeNormal = transform.up;

            int lastPixelLightCount = QualitySettings.pixelLightCount;
            if (!Profile.EnableReflectionPixelLight)
            {
                QualitySettings.pixelLightCount = 0;
            }

            bool lastInvertCulling = GL.invertCulling;
            GL.invertCulling = !lastInvertCulling;

            //set up camera and render
            Camera reflectionCamera = ReflCameras[cam];
            reflectionCamera.enabled = false;
            reflectionCamera.targetTexture = GetReflectionRt(cam);
            MatchCameraSettings(cam, reflectionCamera);

            // Reflect camera around reflection plane
            float d = -Vector3.Dot(planeNormal, planePos) - Profile.ReflectionClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(planeNormal.x, planeNormal.y, planeNormal.z, d);

            Matrix4x4 reflection = CalculateReflectionMatrix(reflectionPlane, Camera.MonoOrStereoscopicEye.Mono);
            Vector3 oldpos = cam.transform.position;
            Vector3 newpos = reflection.MultiplyPoint(oldpos);
            reflectionCamera.worldToCameraMatrix = cam.worldToCameraMatrix * reflection;

            // Setup oblique projection matrix so that near plane is our reflection
            // plane. This way we clip everything below/above it for free.
            bool isBackFace = cam.transform.position.y < transform.position.y;
            Vector4 clipPlane = CameraSpacePlane(reflectionCamera, planePos, planeNormal, Profile.ReflectionClipPlaneOffset, isBackFace && ShouldRenderBackface ? -1.0f : 1.0f);
            Matrix4x4 obliqueMatrix = cam.CalculateObliqueMatrix(clipPlane);
            reflectionCamera.projectionMatrix = obliqueMatrix;

            // Set custom culling matrix from the current camera
            reflectionCamera.cullingMatrix = cam.projectionMatrix * cam.worldToCameraMatrix;
            reflectionCamera.cullingMask = ~(1 << 4) & Profile.ReflectionLayers.value; // never render water layer
            reflectionCamera.transform.position = newpos;
            Vector3 euler = cam.transform.eulerAngles;
            reflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);

            if (reflectionCamera.transform.rotation != Quaternion.identity)
            {
                UniversalRenderPipeline.RenderSingleCamera(context, reflectionCamera);
            }

            QualitySettings.pixelLightCount = lastPixelLightCount;
            GL.invertCulling = lastInvertCulling;
        }
#endif

        public PIndex2D WorldPointToTileIndex(Vector3 p)
        {
            Vector3 localPoint = transform.InverseTransformPoint(p);
            int x = Mathf.FloorToInt(localPoint.x / TileSize.x);
            int z = Mathf.FloorToInt(localPoint.z / TileSize.y);
            return new PIndex2D(x, z);
        }

        public bool CheckTilesContainPoint(Vector3 worldPoint)
        {
            PIndex2D index = WorldPointToTileIndex(worldPoint);
            return TileIndices.Contains(index);
        }

        public void GeneratePlaneMesh()
        {
            meshVersion = PVersionInfo.Number;
            IPMeshCreator meshCreator = null;
            if (PlanePattern == PPlaneMeshPattern.Hexagon)
            {
                meshCreator = new PHexMeshCreator();
            }
            else if (PlanePattern == PPlaneMeshPattern.Diamond)
            {
                meshCreator = new PDiamondMeshCreator();
            }
            else if (PlanePattern == PPlaneMeshPattern.Quad)
            {
                meshCreator = new PQuadMeshCreator();
            }
            else
            {
                meshCreator = new PHexMeshCreator();
            }
            meshCreator.Create(this);
        }

        public void BakeCustomMesh()
        {
            meshVersion = PVersionInfo.Number;
            PCustomMeshBaker baker = new PCustomMeshBaker();
            baker.Create(this);
        }

        public void GenerateAreaMesh()
        {
            meshVersion = PVersionInfo.Number;
            PAreaMeshCreator creator = new PAreaMeshCreator();
            creator.Create(this);
        }

        public void GenerateSplineMesh()
        {
            meshVersion = PVersionInfo.Number;
            PSplineMeshCreator creator = new PSplineMeshCreator();
            creator.Create(this);
        }

        public void GenerateSplineMeshAtSegments(IEnumerable<int> indices)
        {
            meshVersion = PVersionInfo.Number;
            PSplineMeshCreator creator = new PSplineMeshCreator();
            IEnumerator<int> i = indices.GetEnumerator();
            while (i.MoveNext())
            {
                int index = i.Current;
                creator.Create(this, index);
            }
        }

        public void GenerateSplineMeshAtSegment(int index)
        {
            meshVersion = PVersionInfo.Number;
            PSplineMeshCreator creator = new PSplineMeshCreator();
            creator.Create(this, index);
        }

        public void GenerateMesh()
        {
            if (MeshType == PWaterMeshType.TileablePlane)
            {
                GeneratePlaneMesh();
            }
            else if (MeshType == PWaterMeshType.Area)
            {
                GenerateAreaMesh();
            }
            else if (MeshType == PWaterMeshType.Spline)
            {
                GenerateSplineMesh();
            }
            else if (MeshType == PWaterMeshType.CustomMesh && SourceMesh != null)
            {
                BakeCustomMesh();
            }
        }

#if UNITY_EDITOR
        private void CheckMeshVersion()
        {
            if (meshVersion < MESH_VERSION_SERIALIZED_IN_WATER_COMPONENT)
            {
                MeshType = PWaterMeshType.TileablePlane;
                MeshResolution = 100;
                SplineResolutionX = 10;
                SplineResolutionY = 20;
                SplineWidth = 5;
                GenerateMesh();
                meshVersion = PVersionInfo.Number;
            }
        }
#endif

        public double GetTimeParam()
        {
            if (TimeMode == PTimeMode.Auto)
            {
                if (timer == null)
                    return 0; 
                double elapsedSeconds = timer.ElapsedMilliseconds * 0.001f;
                double delta = elapsedSeconds - lastElapsedSeconds;
                frameTime += delta * Time.timeScale;
                lastElapsedSeconds = elapsedSeconds;
                return frameTime;
            }
            else
            {
                return ManualTimeSeconds;
            }
        }

        //#if TEXTURE_GRAPH
        //        public void EmbedWaveMask()
        //        {
        //            TGraph graph = Instantiate<TGraph>(TextureGraphAsset);
        //            List<PPoseidonWaveMaskNode> nodes = graph.GraphData.GetNodeOfType<PPoseidonWaveMaskNode>();
        //            if (nodes.Count == 0)
        //            {
        //                Debug.Log("There are no Poseidon Wave Mask Node in the graph. Make sure you have one.");
        //                return;
        //            }

        //            PPoseidonWaveMaskNode n = nodes[0];
        //            graph.Execute();
        //            RenderTexture rt = graph.GetMainRT(n.GUID);

        //            if (embeddedWaveMask != null)
        //            {
        //                PUtilities.DestroyObject(embeddedWaveMask);
        //            }
        //            embeddedWaveMask = new Texture2D(rt.width, rt.height, TextureFormat.RGBAFloat, false, true);
        //            embeddedWaveMask.wrapMode = TextureWrapMode.Clamp;
        //            embeddedWaveMask.name = "Mask";
        //            PCommon.CopyFromRT(embeddedWaveMask, rt);

        //            graph.CleanUp();
        //            PUtilities.DestroyObject(graph);
        //        }
        //#endif
    }
}
