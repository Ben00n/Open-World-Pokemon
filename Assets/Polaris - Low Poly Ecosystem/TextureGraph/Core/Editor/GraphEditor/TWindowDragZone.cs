using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

namespace Pinwheel.TextureGraph
{
    public class TWindowDragZone : MouseManipulator
    {
        public VisualElement Window { get; set; }
        public VisualElement WindowParent { get; set; }
        private bool IsDragging { get; set; }

        public Action<Vector2> OnPositionChanged;

        public TWindowDragZone(VisualElement window, VisualElement windowParent)
        {
            Window = window;
            WindowParent = windowParent;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.NoTrickleDown);
            target.RegisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.NoTrickleDown);
            target.RegisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.NoTrickleDown);
            WindowParent.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged, TrickleDown.NoTrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseDownEvent>(OnMouseDown, TrickleDown.NoTrickleDown);
            target.UnregisterCallback<MouseMoveEvent>(OnMouseMove, TrickleDown.NoTrickleDown);
            target.UnregisterCallback<MouseUpEvent>(OnMouseUp, TrickleDown.NoTrickleDown);
            WindowParent.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged, TrickleDown.NoTrickleDown);
        }

        private void OnMouseDown(MouseDownEvent e)
        {
            target.CaptureMouse();
            if (e.button == 0)
            {
                IsDragging = true;
                Window.BringToFront();
            }
            e.StopPropagation();
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!IsDragging)
                return;
            Vector2 delta = e.mouseDelta;
            MoveAndClampPosition(delta);
            Window.BringToFront();
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

        private void MoveAndClampPosition(Vector2 delta)
        {
            float left = Window.style.left.value.value + delta.x;
            float top = Window.style.top.value.value + delta.y;
            float right = float.NaN;
            float bottom = float.NaN;

            float minLeft = 0;
            float maxLeft = Window.parent.layout.width - Window.layout.width;
            float minTop = 0;
            float maxTop = Window.parent.layout.height - Window.layout.height;

            left = Mathf.Clamp(left, minLeft, maxLeft);
            top = Mathf.Clamp(top, minTop, maxTop);

            Window.style.left = new StyleLength(left);
            Window.style.top = new StyleLength(top);
            Window.style.right = new StyleLength(right);
            Window.style.bottom = new StyleLength(bottom);

            if (OnPositionChanged != null)
            {
                OnPositionChanged.Invoke(new Vector2(left, top));
            }
        }

        private void OnGeometryChanged(GeometryChangedEvent e)
        {
            MoveAndClampPosition(Vector2.zero);
        }
    }
}
