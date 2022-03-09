using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;


namespace Pinwheel.TextureGraph
{
    public class TView2DWindow : TSubWindow
    {
        public struct TViewData
        {
            public const string KEY_POSITION_X = "position-x";
            public const string KEY_POSITION_Y = "position-y";
            public const string KEY_SIZE_X = "size-x";
            public const string KEY_SIZE_Y = "size-y";
            public const string KEY_TILING_X = "tiling-x";
            public const string KEY_TILING_Y = "tiling-y";
            public const string KEY_OFFSET_X = "offset-x";
            public const string KEY_OFFSET_Y = "offset-y";
            public const string KEY_GUID = "guid";
            public const string KEY_CHANNEL = "channel";

            public Vector2 position;
            public Vector2 size;
            public Vector2 imageTiling;
            public Vector2 imageOffset;
            public Guid nodeGuid;
            public int channel;

            public void Load(string key)
            {
                position = new Vector2(EditorPrefs.GetFloat(key + KEY_POSITION_X, 0), EditorPrefs.GetFloat(key + KEY_POSITION_Y, 0));
                size = new Vector2(EditorPrefs.GetFloat(key + KEY_SIZE_X, 200), EditorPrefs.GetFloat(key + KEY_SIZE_Y, 200));
                imageTiling = new Vector2(EditorPrefs.GetFloat(key + KEY_TILING_X, 1), EditorPrefs.GetFloat(key + KEY_TILING_Y, 1));
                imageOffset = new Vector2(EditorPrefs.GetFloat(key + KEY_OFFSET_X, 0), EditorPrefs.GetFloat(key + KEY_OFFSET_Y, 0));
                string guidString = EditorPrefs.GetString(key + KEY_GUID, "");
                if (!string.IsNullOrEmpty(guidString))
                {
                    nodeGuid = new Guid(guidString);
                }
                else
                {
                    nodeGuid = new Guid();
                }
                channel = EditorPrefs.GetInt(key + KEY_CHANNEL, 0);
            }

            public void Save(string key)
            {
                EditorPrefs.SetFloat(key + KEY_POSITION_X, position.x);
                EditorPrefs.SetFloat(key + KEY_POSITION_Y, position.y);
                EditorPrefs.SetFloat(key + KEY_SIZE_X, size.x);
                EditorPrefs.SetFloat(key + KEY_SIZE_Y, size.y);
                EditorPrefs.SetFloat(key + KEY_TILING_X, imageTiling.x);
                EditorPrefs.SetFloat(key + KEY_TILING_Y, imageTiling.y);
                EditorPrefs.SetFloat(key + KEY_OFFSET_X, imageOffset.x);
                EditorPrefs.SetFloat(key + KEY_OFFSET_Y, imageOffset.y);
                EditorPrefs.SetString(key + KEY_GUID, nodeGuid.ToString());
                EditorPrefs.SetInt(key + KEY_CHANNEL, channel);
            }
        }

        private static Material imageMaterial;
        private static Material ImageMaterial
        {
            get
            {
                if (imageMaterial == null)
                {
                    imageMaterial = new Material(Shader.Find("Hidden/TextureGraph/View2DImage"));
                }
                return imageMaterial;
            }
        }

        private static readonly int TILING = Shader.PropertyToID("_Tiling");
        private static readonly int OFFSET = Shader.PropertyToID("_Offset");
        private static readonly int RRR = Shader.PropertyToID("_RRR");
        private static readonly int CHANNEL = Shader.PropertyToID("_Channel");

        public TViewData viewData;

        private VisualElement headerToolbar;
        private Button rgbButton;
        private Button rButton;
        private Button gButton;
        private Button bButton;
        private Button aButton;

        public TView2DWindow(string viewKey) : base(viewKey)
        {
            this.viewDataKey = viewKey;
            viewData.Load(viewKey);

            styleSheets.Add(Resources.Load<StyleSheet>("TextureGraph/USS/View2DWindow"));

            headerToolbar = new VisualElement() { name = "headerToolbar" };
            topContainer.Add(headerToolbar);
            rgbButton = new Button() { name = "rgbButton", text = "RGB" };
            rgbButton.clicked += () => { SetActiveChannel(0); };

            rButton = new Button() { name = "rButton", text = "R" };
            rButton.clicked += () => { SetActiveChannel(1); };

            gButton = new Button() { name = "gButton", text = "G" };
            gButton.clicked += () => { SetActiveChannel(2); };

            bButton = new Button() { name = "bButton", text = "B" };
            bButton.clicked += () => { SetActiveChannel(3); };

            aButton = new Button() { name = "aButton", text = "A" };
            aButton.clicked += () => { SetActiveChannel(4); };

            headerToolbar.Add(rgbButton);
            headerToolbar.Add(rButton);
            headerToolbar.Add(gButton);
            headerToolbar.Add(bButton);
            headerToolbar.Add(aButton);

            IMGUIContainer img = new IMGUIContainer() { name = "image" };
            img.AddManipulator(new TScrollable(OnImageScroll));
            img.AddManipulator(new TDraggable(OnImageDrag));
            this.AddManipulator(new ContextualMenuManipulator(BuildContextualMenu));
            bodyContainer.Add(img);

            SetTitle("2D View");
        }

        public override void Show()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            SetPosition(viewData.position);
            SetSize(viewData.size);
            EnableWindowDragging((v) => { viewData.position = v; });
            EnableWindowResizing(Vector2.one * 100, Vector2.one * 1000, OnResize);
            SetActiveChannel(viewData.channel);
            MarkDirtyRepaint();
        }

        public override void Hide()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            DisableWindowDragging();
            DisableWindowResizing();
            viewData.Save(viewDataKey);
        }

        public override void OnEnable()
        {
            viewData.Load(viewDataKey);
        }

        public override void OnDisable()
        {
            viewData.Save(viewDataKey);
            Dispose();
        }

        public override void OnDestroy()
        {
            viewData.Save(viewDataKey);
            Dispose();
        }

        public void SetImage(Texture tex)
        {
            IMGUIContainer img = bodyContainer.Q<IMGUIContainer>("image");
            img.onGUIHandler = () =>
            {
                Rect r = new Rect(0, 0, img.layout.width, img.layout.height);
                if (tex == null)
                {
                    EditorGUI.DrawRect(r, Color.black);
                    EditorGUI.LabelField(r, "No preview available.", TEditorCommon.CenteredWhiteLabel);
                }
                else
                {
                    ImageMaterial.SetVector(TILING, viewData.imageTiling);
                    ImageMaterial.SetVector(OFFSET, viewData.imageOffset);
                    if (TUtilities.IsGrayscaleFormat(tex.graphicsFormat))
                    {
                        ImageMaterial.SetFloat(RRR, 1);
                    }
                    else
                    {
                        ImageMaterial.SetFloat(RRR, 0);
                    }
                    ImageMaterial.SetFloat(CHANNEL, viewData.channel);

                    EditorGUI.DrawPreviewTexture(r, tex, ImageMaterial, ScaleMode.ScaleAndCrop);
                }
            };
        }

        private void OnImageScroll(Vector2 delta)
        {
            viewData.imageTiling -= Vector2.one * delta.y * 0.005f;
            viewData.imageTiling.x = Mathf.Max(0.01f, viewData.imageTiling.x);
            viewData.imageTiling.y = Mathf.Max(0.01f, viewData.imageTiling.y);
        }

        private void OnImageDrag(TDraggable.TDragInfo drag)
        {
            float fx = drag.delta.x / layout.width;
            float fy = drag.delta.y / layout.height;
            float ox = -fx / viewData.imageTiling.x;
            float oy = fy / viewData.imageTiling.y;

            viewData.imageOffset += new Vector2(ox, oy);
            this.MarkDirtyRepaint();
        }

        private void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction(
                "Reset View",
                (a) =>
                {
                    ResetView();
                });
        }

        public void ResetView()
        {
            viewData.imageTiling = Vector2.one;
            viewData.imageOffset = Vector2.zero;
            SetActiveChannel(0);
            MarkDirtyRepaint();
        }

        private void OnResize(Vector2 s)
        {
            viewData.size = s;

            if (s.x < 210)
            {
                headerToolbar.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            }
            else
            {
                headerToolbar.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
            }
        }

        private void SetActiveChannel(int channel)
        {
            viewData.channel = channel;
            const string CLASS_ACTIVE = "active-button";
            if (channel == 0)
            {
                rgbButton.AddToClassList(CLASS_ACTIVE);
                rButton.RemoveFromClassList(CLASS_ACTIVE);
                gButton.RemoveFromClassList(CLASS_ACTIVE);
                bButton.RemoveFromClassList(CLASS_ACTIVE);
                aButton.RemoveFromClassList(CLASS_ACTIVE);
            }
            else if (channel == 1)
            {
                rgbButton.RemoveFromClassList(CLASS_ACTIVE);
                rButton.AddToClassList(CLASS_ACTIVE);
                gButton.RemoveFromClassList(CLASS_ACTIVE);
                bButton.RemoveFromClassList(CLASS_ACTIVE);
                aButton.RemoveFromClassList(CLASS_ACTIVE);
            }
            else if (channel == 2)
            {
                rgbButton.RemoveFromClassList(CLASS_ACTIVE);
                rButton.RemoveFromClassList(CLASS_ACTIVE);
                gButton.AddToClassList(CLASS_ACTIVE);
                bButton.RemoveFromClassList(CLASS_ACTIVE);
                aButton.RemoveFromClassList(CLASS_ACTIVE);
            }
            else if (channel == 3)
            {
                rgbButton.RemoveFromClassList(CLASS_ACTIVE);
                rButton.RemoveFromClassList(CLASS_ACTIVE);
                gButton.RemoveFromClassList(CLASS_ACTIVE);
                bButton.AddToClassList(CLASS_ACTIVE);
                aButton.RemoveFromClassList(CLASS_ACTIVE);
            }
            else if (channel == 4)
            {
                rgbButton.RemoveFromClassList(CLASS_ACTIVE);
                rButton.RemoveFromClassList(CLASS_ACTIVE);
                gButton.RemoveFromClassList(CLASS_ACTIVE);
                bButton.RemoveFromClassList(CLASS_ACTIVE);
                aButton.AddToClassList(CLASS_ACTIVE);
            }
        }
    }
}
