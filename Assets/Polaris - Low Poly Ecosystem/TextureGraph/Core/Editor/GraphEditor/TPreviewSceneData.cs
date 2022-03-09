using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public class TPreviewSceneData : IDisposable
    {
        public Scene PreviewScene { get; set; }
        public Camera Camera { get; set; }
        public Material Material { get; set; }
        public RenderTexture RenderTarget { get; set; }

        public TPreviewSceneData()
        {
            Material = new Material(Shader.Find("Hidden/TextureGraph/Preview3D"));
        }

        public void Dispose()
        {
            if (RenderTarget != null)
            {
                RenderTarget.Release();
                TUtilities.DestroyObject(RenderTarget);
            }
            if (Material != null)
            {
                TUtilities.DestroyObject(Material);
            }
        }

        public void PrepareScene()
        {
            PreviewScene = EditorSceneManager.NewPreviewScene();
            GameObject cameraObject = new GameObject("Camera") { hideFlags = HideFlags.HideAndDontSave };
            SceneManager.MoveGameObjectToScene(cameraObject, PreviewScene);
            Camera = cameraObject.AddComponent<Camera>();
            Camera.transform.position = new Vector3(0, 0, -1);
            Camera.transform.rotation = Quaternion.identity;
            Camera.transform.localScale = Vector3.one;
            Camera.nearClipPlane = 0.01f;
            Camera.farClipPlane = 1000;
            Camera.clearFlags = CameraClearFlags.SolidColor;
            Camera.backgroundColor = Color.clear;
            Camera.cameraType = CameraType.Preview;
            Camera.useOcclusionCulling = false;
            Camera.scene = PreviewScene;
            Camera.enabled = false;
        }

        public void CloseScene()
        {
            if (Camera != null)
            {
                Camera.targetTexture = null;
                TUtilities.DestroyGameobject(Camera.gameObject);
            }
            EditorSceneManager.ClosePreviewScene(PreviewScene);
        }

        public void PrepareRenderTarget(int width, int height)
        {
            if (RenderTarget != null)
            {
                if (RenderTarget.width != width ||
                    RenderTarget.height != height)
                {
                    RenderTarget.Release();
                    TUtilities.DestroyObject(RenderTarget);
                    RenderTarget = null;
                }
            }

            if (RenderTarget == null)
            {
                RenderTarget = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32, 0);
            }
        }

    }
}
