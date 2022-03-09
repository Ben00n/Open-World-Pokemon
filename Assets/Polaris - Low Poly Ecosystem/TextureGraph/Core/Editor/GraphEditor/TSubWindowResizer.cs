using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

namespace Pinwheel.TextureGraph
{
    public class TSubWindowResizer : MouseManipulator
    {
        public VisualElement Window { get; set; }
        public Vector2 MinSize { get; set; }
        public Vector2 MaxSize { get; set; }
        private bool IsDragging { get; set; }

        public Action<Vector2> OnSizeChanged;

        public TSubWindowResizer(VisualElement window)
        {
            Window = window;
            MinSize = Vector2.zero;
            MaxSize = Vector2.one * 10000;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.NoTrickleDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.NoTrickleDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.NoTrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.NoTrickleDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.NoTrickleDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.NoTrickleDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            target.CaptureMouse();
            if (e.button == 0)
            {
                IsDragging = true;
            }
            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!IsDragging)
                return;
            Vector2 delta = e.mouseDelta;
            ResizeAndClamp(delta, e.shiftKey);
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (target.HasMouseCapture())
            {
                target.ReleaseMouse();
            }
            IsDragging = false;
            e.StopPropagation();
        }

        private void ResizeAndClamp(Vector2 delta, bool maintainAspect = false)
        {
            float width = Window.style.width.value.value;
            float height = Window.style.height.value.value;

            width += delta.x;
            height += delta.y;

            width = Mathf.Clamp(width, MinSize.x, MaxSize.x);
            height = Mathf.Clamp(height, MinSize.y, MaxSize.y);

            if (maintainAspect)
            {
                float max = Mathf.Max(width, height);
                width = max;
                height = max;
            }

            Window.style.width = new StyleLength(width);
            Window.style.height = new StyleLength(height);

            if (OnSizeChanged != null)
            {
                OnSizeChanged.Invoke(new Vector2(width, height));
            }
        }
    }
}
