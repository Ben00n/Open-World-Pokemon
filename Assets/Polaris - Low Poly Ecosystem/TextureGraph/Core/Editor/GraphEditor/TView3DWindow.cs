using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using UnityEngine.Rendering;

namespace Pinwheel.TextureGraph
{
    public class TView3DWindow : TSubWindow, ITManagedView
    {
        public struct TViewData
        {
            public const string KEY_POSITION_X = "position-x";
            public const string KEY_POSITION_Y = "position-y";
            public const string KEY_SIZE_X = "size-x";
            public const string KEY_SIZE_Y = "size-y";

            public static readonly Vector2 MIN_SIZE = new Vector2(200, 200);
            public static readonly Vector2 MAX_SIZE = new Vector2(1000, 1000);

            public static readonly float CAMERA_MIN_DISTANCE = 0.1f;
            public static readonly float CAMERA_MAX_DISTANCE = 10;

            public Vector2 position;
            public Vector2 size;

            public void Load(string key)
            {
                position = new Vector2(EditorPrefs.GetFloat(key + KEY_POSITION_X, 0), EditorPrefs.GetFloat(key + KEY_POSITION_Y, 0));
                size = new Vector2(EditorPrefs.GetFloat(key + KEY_SIZE_X, 200), EditorPrefs.GetFloat(key + KEY_SIZE_Y, 200));
            }

            public void Save(string key)
            {
                EditorPrefs.SetFloat(key + KEY_POSITION_X, position.x);
                EditorPrefs.SetFloat(key + KEY_POSITION_Y, position.y);
                EditorPrefs.SetFloat(key + KEY_SIZE_X, size.x);
                EditorPrefs.SetFloat(key + KEY_SIZE_Y, size.y);
            }
        }

        private static Material imageMaterial;
        private static Material ImageMaterial
        {
            get
            {
                if (imageMaterial == null)
                {
                    imageMaterial = new Material(Shader.Find("Hidden/TextureGraph/View3DImage"));
                }
                return imageMaterial;
            }
        }

        public TViewData viewData;
        public TPreviewSceneData previewSceneData;

        public IMGUIContainer image;

        public TView3DWindow(string viewKey) : base(viewKey)
        {
            this.viewDataKey = viewKey;
            viewData.Load(viewKey);

            styleSheets.Add(Resources.Load<StyleSheet>("TextureGraph/USS/View3DWindow"));
            image = new IMGUIContainer() { name = "image" };
            image.onGUIHandler = HandleDisplayImage;
            image.AddManipulator(new TScrollable(OnImageScroll));
            image.AddManipulator(new TDraggable(OnImageDrag));
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            bodyContainer.Add(image);

            image.RegisterCallback<DragUpdatedEvent>(OnDragUpdated);
            image.RegisterCallback<DragPerformEvent>(OnDragPerform);

            SetTitle("3D View");
        }

        public override void Show()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            SetPosition(viewData.position);
            SetSize(viewData.size);
            EnableWindowDragging((v) => { viewData.position = v; });
            EnableWindowResizing(TViewData.MIN_SIZE, TViewData.MAX_SIZE, OnResize);
            MarkDirtyRepaint();

            previewSceneData = new TPreviewSceneData();
            RenderPreviewScene();
            MarkDirtyRepaint();
        }

        public override void Hide()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            DisableWindowDragging();
            DisableWindowResizing();
            Dispose();
            viewData.Save(viewDataKey);
        }

        public override void OnEnable()
        {
            viewData.Load(viewDataKey);
        }

        public override void OnDisable()
        {
            viewData.Save(viewDataKey);
            OnDisable();
        }

        public override void OnDestroy()
        {
            viewData.Save(viewDataKey);
            Dispose();
        }

        private void OnResize(Vector2 s)
        {
            viewData.size = s;
            RenderPreviewScene();
        }

        public void RenderPreviewScene()
        {
            if (previewSceneData == null)
                return;

            int width = (int)Mathf.Max(TViewData.MIN_SIZE.x, image.layout.width) * 2;
            int height = (int)Mathf.Max(TViewData.MIN_SIZE.y, image.layout.height) * 2;
            if (width <= 0 || height <= 0)
                return;

            previewSceneData.PrepareScene();
            try
            {
                previewSceneData.PrepareRenderTarget(width, height);

                Camera cam = previewSceneData.Camera;
                cam.renderingPath = RenderingPath.Forward;
                cam.targetTexture = previewSceneData.RenderTarget;
                TView3DEnvironmentSettings envSettings = GraphEditor.ClonedGraph.View3DEnvironmentSettings;
                cam.transform.position = envSettings.GetCameraPosition();
                cam.transform.rotation = envSettings.GetCameraRotation();
                cam.transform.localScale = Vector3.one;

                Mesh mesh = envSettings.GetMesh();
                Material mat = previewSceneData.Material;
                envSettings.Setup(mat, GraphEditor.ClonedGraph);

                Matrix4x4 trs = Matrix4x4.identity;
                Graphics.DrawMesh(mesh, trs, mat, 1, cam, 0, null, ShadowCastingMode.Off, false, null, false);
                cam.Render();
                MarkDirtyRepaint();
            }
            catch { }
            previewSceneData.CloseScene();
        }

        public override void Dispose()
        {
            if (previewSceneData != null)
            {
                previewSceneData.Dispose();
            }
        }

        private void OnImageScroll(Vector2 delta)
        {
            if (previewSceneData == null)
                return;
            TView3DEnvironmentSettings envSettings = GraphEditor.ClonedGraph.View3DEnvironmentSettings;
            float d = envSettings.GetCameraDistance();
            d += delta.y * 0.05f;
            d = Mathf.Clamp(d, TViewData.CAMERA_MIN_DISTANCE, TViewData.CAMERA_MAX_DISTANCE);
            envSettings.SetCameraDistance(d);
            RenderPreviewScene();
        }

        private void HandleDisplayImage()
        {
            if (previewSceneData == null || previewSceneData.RenderTarget == null || !previewSceneData.RenderTarget.IsCreated())
            {
                RenderPreviewScene();
            }

            Rect r = new Rect(0, 0, image.layout.width, image.layout.height);
            if (previewSceneData == null || previewSceneData.RenderTarget == null || !previewSceneData.RenderTarget.IsCreated())
            {
                EditorGUI.DrawRect(r, Color.black);
                EditorGUI.LabelField(r, "No preview available.", TEditorCommon.CenteredWhiteLabel);
            }
            else
            {
                Texture tex = previewSceneData.RenderTarget;
                EditorGUI.DrawPreviewTexture(r, tex, ImageMaterial, ScaleMode.ScaleAndCrop);
            }
        }

        private void OnImageDrag(TDraggable.TDragInfo drag)
        {
            if (previewSceneData == null)
                return;
            TView3DEnvironmentSettings envSettings = GraphEditor.ClonedGraph.View3DEnvironmentSettings;
            float d = envSettings.GetCameraDistance();
            float fd = Mathf.InverseLerp(TViewData.CAMERA_MIN_DISTANCE, TViewData.CAMERA_MAX_DISTANCE, d);
            float speed = 0.15f;
            Vector2 offset = new Vector2(-drag.delta.x, drag.delta.y) * speed;

            if (drag.isShift)
            {
                envSettings.RotateLight(offset);
            }
            else
            {
                envSettings.RotateCamera(offset);
            }
            RenderPreviewScene();
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent e)
        {
            e.menu.AppendAction(
                "Reset View",
                (a) =>
                {
                    ResetEnvironmentView();
                });

            foreach (string n in Enum.GetNames(typeof(TPremitives.TMeshType)))
            {
                TPremitives.TMeshType meshType;
                if (Enum.TryParse<TPremitives.TMeshType>(n, out meshType))
                {
                    e.menu.AppendAction(
                        "Mesh/" + n,
                        (a) =>
                        {
                            GraphEditor.ClonedGraph.View3DEnvironmentSettings.MeshType = meshType;
                            RenderPreviewScene();
                        });
                }
            }
        }

        private void ResetEnvironmentView()
        {
            GraphEditor.ClonedGraph.View3DEnvironmentSettings = new TView3DEnvironmentSettings();
            RenderPreviewScene();
        }

        private void OnDragUpdated(DragUpdatedEvent e)
        {
            UnityEngine.Object[] dragged = DragAndDrop.objectReferences;
            if (dragged.Length > 1)
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
            else
            {
                if (dragged[0] is Mesh m)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                }
            }
        }

        private void OnDragPerform(DragPerformEvent e)
        {
            UnityEngine.Object[] dragged = DragAndDrop.objectReferences;
            if (dragged.Length == 1 && dragged[0] is Mesh m)
            {
                GraphEditor.ClonedGraph.View3DEnvironmentSettings.MeshType = TPremitives.TMeshType.Custom;
                GraphEditor.ClonedGraph.View3DEnvironmentSettings.CustomMesh = m;
                RenderPreviewScene();
            }
        }
    }
}
