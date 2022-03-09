using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor;
using System;

namespace Pinwheel.TextureGraph
{
    public class TSubWindow : VisualElement, ITManagedView
    {
        public TGraphEditor GraphEditor { get; set; }

        public VisualElement mainContainer;
        public VisualElement topContainer;
        public VisualElement bodyContainer;
        public VisualElement resizeHandleContainer;
        public Button closeButton;

        protected TWindowDragZone dragZone;
        protected TSubWindowResizer resizer;

        protected TextElement titleLabel;

        public TSubWindow(string viewKey)
        {
            viewDataKey = viewKey;
            styleSheets.Add(Resources.Load<StyleSheet>("TextureGraph/USS/SubWindow"));

            mainContainer = new VisualElement() { name = "mainContainer" };
            this.Add(mainContainer);

            topContainer = new VisualElement() { name = "topContainer" };
            titleLabel = new TextElement() { name = "titleLabel" };
            topContainer.Add(titleLabel);
            mainContainer.Add(topContainer);

            closeButton = new Button() { name = "closeButton", text = "X" };
            closeButton.clicked += () => { TViewManager.HideView(this, viewDataKey); };
            topContainer.Add(closeButton);

            bodyContainer = new VisualElement() { name = "bodyContainer" };
            mainContainer.Add(bodyContainer);

            resizeHandleContainer = new VisualElement() { name = "resizeHandleContainer" };
            mainContainer.Add(resizeHandleContainer);
        }

        public void SetTitle(string t)
        {
            TextElement titleLabel = topContainer.Q<TextElement>("titleLabel");
            titleLabel.text = t;
        }

        public void SetPosition(Vector2 pos)
        {
            style.left = new StyleLength(pos.x);
            style.top = new StyleLength(pos.y);
            style.right = new StyleLength(float.NaN);
            style.bottom = new StyleLength(float.NaN);
        }

        public void SetSize(Vector2 size)
        {
            style.width = new StyleLength(size.x);
            style.height = new StyleLength(size.y);
        }

        public void EnableWindowDragging(Action<Vector2> onPositionChanged = null)
        {
            dragZone = new TWindowDragZone(this, this.parent);
            dragZone.OnPositionChanged = onPositionChanged;
            topContainer.AddManipulator(dragZone);
        }

        public void DisableWindowDragging()
        {
            topContainer.RemoveManipulator(dragZone);
        }

        public void EnableWindowResizing(Vector2 minSize, Vector2 maxSize, Action<Vector2> onSizeChanged = null)
        {
            resizer = new TSubWindowResizer(this)
            {
                MinSize = minSize,
                MaxSize = maxSize,
                OnSizeChanged = onSizeChanged
            };
            resizeHandleContainer.AddManipulator(resizer);
        }

        public void DisableWindowResizing()
        {
            resizeHandleContainer.RemoveManipulator(resizer);
        }

        public virtual void Show()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
        }

        public virtual void Hide()
        {
            style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
        }

        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }

        public virtual void OnDestroy()
        {

        }

        public virtual void Dispose()
        {
        }
    }
}
