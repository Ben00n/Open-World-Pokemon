using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using System;

namespace Pinwheel.TextureGraph
{
    public class TScrollable : MouseManipulator
    {
        public Action<Vector2> OnScroll;

        public TScrollable(Action<Vector2> onScroll)
        {
            OnScroll = onScroll;
        }

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<WheelEvent>(OnMouseWheelEvent, TrickleDown.NoTrickleDown);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<WheelEvent>(OnMouseWheelEvent, TrickleDown.NoTrickleDown);
        }

        private void OnMouseWheelEvent(WheelEvent e)
        {
            if (OnScroll != null)
            {
                OnScroll.Invoke(e.delta);
                e.StopPropagation();
            }
        }
    }
}
