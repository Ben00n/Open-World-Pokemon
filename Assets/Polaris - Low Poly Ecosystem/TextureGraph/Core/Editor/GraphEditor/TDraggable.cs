using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

namespace Pinwheel.TextureGraph
{
    public class TDraggable : MouseManipulator
    {
        public struct TDragInfo
        {
            public Vector2 delta;
            public bool isShift;
            public bool isCtrl;
            public bool isAlt;
        }

        public Action<TDragInfo> OnDrag;
        public bool IsDragging;

        public TDraggable(Action<TDragInfo> onDrag)
        {
            OnDrag = onDrag;
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
            if (e.button == 0)
            {
                target.CaptureMouse();
                IsDragging = true;
                e.StopPropagation();
            }
        }

        private void OnMouseMove(MouseMoveEvent e)
        {
            if (!IsDragging)
                return;
            if (OnDrag != null)
            {
                TDragInfo d = new TDragInfo();
                d.delta = e.mouseDelta;
                d.isShift = e.shiftKey;
                d.isCtrl = e.ctrlKey;
                d.isAlt = e.altKey;
                OnDrag.Invoke(d);
            }
        }

        private void OnMouseUp(MouseUpEvent e)
        {
            if (target.HasMouseCapture())
            {
                target.ReleaseMouse();
                e.StopPropagation();
            }
            IsDragging = false;
        }
    }
}
