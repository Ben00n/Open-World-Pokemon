using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Pinwheel.TextureGraph.UIElements
{
    public class TResizer : VisualElement
    {
        public static readonly string USS_RESIZER = "resizer";
        public static readonly string USS_RESIZER_LEFT = "resizer-left";

        private bool isDragging;

        public VisualElement Target { get; set; }

        public TResizer()
        {
            StyleSheet styles = Resources.Load<StyleSheet>("TextureGraph/USS/ResizerStyles");
            styleSheets.Add(styles);

            AddToClassList(USS_RESIZER);
            AddToClassList(USS_RESIZER_LEFT);
        }

        protected override void ExecuteDefaultAction(EventBase evt)
        {
            base.ExecuteDefaultAction(evt);

            if (Target == null)
                return;

            long eventTypeId = evt.eventTypeId;
            if (eventTypeId == MouseDownEvent.TypeId())
            {
                isDragging = true;
                evt.target.CaptureMouse();
            }
            else if (eventTypeId == MouseMoveEvent.TypeId())
            {
                if (!isDragging)
                    return;
                MouseMoveEvent mouseMoveEvt = evt as MouseMoveEvent;
                float currentWidth = Target.resolvedStyle.width;
                Target.style.width = new StyleLength(currentWidth - mouseMoveEvt.mouseDelta.x);
            }
            else if (eventTypeId == MouseLeaveEvent.TypeId())
            {
                if (!isDragging)
                    return;
                MouseLeaveEvent mouseLeaveEvt = evt as MouseLeaveEvent;
                float currentWidth = Target.resolvedStyle.width;
                Target.style.width = new StyleLength(currentWidth - mouseLeaveEvt.mouseDelta.x);
            }
            else if (eventTypeId == MouseUpEvent.TypeId())
            {
                isDragging = false;
                evt.target.ReleaseMouse();
            }
        }
    }
}
