using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [TCustomParametersDrawer(typeof(TCropNode))]
    public class TCropNodeParamsDrawer : TParametersDrawer
    {
        private static readonly GUIContent rectCenterGUI = new GUIContent("Rect Center", "Center position of the cropping rectangle (percentage)");
        private static readonly GUIContent rectRotationGUI = new GUIContent("Rect Rotation", "Rotation of the cropping rectangle (degree)");
        private static readonly GUIContent rectSizeGUI = new GUIContent("Rect Size", "Size of the cropping rectangle (percentage)");
        private static readonly GUIContent backgroundColorGUI = new GUIContent("Background Color", "Fill color for pixels that being cropped");
        private static readonly float HANDLE_SIZE = 10;

        private static bool isDraggingCenterPoint = false;
        private static int draggedCornerIndex = -1;
        private static bool isDraggingRotationPoint = false;
        private static Vector2 rotationStartPoint = Vector2.zero;

        public override void DrawGUI(TAbstractTextureNode target)
        {
            TCropNode n = target as TCropNode;
            n.RectCenter = TParamGUI.Vector2Field(rectCenterGUI, n.RectCenter);
            n.RectRotation = TParamGUI.FloatField(rectRotationGUI, n.RectRotation);
            n.RectSize = TParamGUI.Vector2Field(rectSizeGUI, n.RectSize);
            n.BackgroundColor = TParamGUI.ColorField(backgroundColorGUI, n.BackgroundColor);

            EditorGUILayout.Space();
            DrawRectEditor(n);
        }

        private void DrawRectEditor(TCropNode n)
        {
            Rect r = GUILayoutUtility.GetAspectRect(1);
            Texture preview = null;
            TSlot previewSlot = n.GetMainOutputSlot();
            if (previewSlot!=null)
            {
                preview = Graph.GetRT(TSlotReference.Create(n.GUID, previewSlot.Id));
            }

            if (preview != null)
            {
                EditorGUI.DrawTextureTransparent(r, preview);
            }
            else
            {
                EditorGUI.DrawRect(r, Color.black);
            }
            GUI.BeginClip(r);
            Rect canvas = new Rect(0, 0, r.width, r.height);
            Handles.BeginGUI();
            Color color = Handles.color;
            Handles.color = new Color(0f, 0.9f, 0f, 1f);
            HandleCenterPointDragging(n, canvas);
            HandleCornerPointsDragging(n, canvas);
            HandleRotationPointsDragging(n, canvas);
            Handles.color = color;
            Handles.EndGUI();
            GUI.EndClip();
            TEditorCommon.DrawOutlineBox(r, TEditorCommon.boxBorderColor);
        }

        private Vector2 NodeToCanvasCenterPos(TCropNode n, Rect canvas)
        {
            Vector2 pos = n.RectCenter.value / 100f;
            pos.y = 1 - pos.y;
            pos = Rect.NormalizedToPoint(canvas, pos);
            return pos;
        }

        private Vector2 CanvasToNodeCenterPos(TCropNode n, Rect canvas, Vector2 pos)
        {
            pos = Rect.PointToNormalized(canvas, pos);
            pos.y = 1 - pos.y;
            return pos * 100f;
        }

        private void HandleCenterPointDragging(TCropNode n, Rect canvas)
        {
            Vector2 centerPos = NodeToCanvasCenterPos(n, canvas);
            Rect centerHandleRect = new Rect() { size = Vector2.one * HANDLE_SIZE, center = centerPos };
            Handles.DrawSolidDisc(centerPos, Vector3.forward, HANDLE_SIZE * 0.5f);
            EditorGUIUtility.AddCursorRect(centerHandleRect, MouseCursor.MoveArrow);

            if (Event.current.type == EventType.MouseDown)
            {
                if (centerHandleRect.Contains(Event.current.mousePosition))
                {
                    isDraggingCenterPoint = true;
                }
                else
                {
                    isDraggingCenterPoint = false;
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (isDraggingCenterPoint)
                {
                    Vector2 mousePos = Event.current.mousePosition;
                    n.RectCenter.value = CanvasToNodeCenterPos(n, canvas, mousePos);
                    GUI.changed = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDraggingCenterPoint = false;
            }
        }

        private Vector2 FlipY(Vector2 v)
        {
            return new Vector2(v.x, 1 - v.y);
        }

        private Vector2 NormalizedToPointUnclamped(Rect r, Vector2 p)
        {
            float x = Mathf.LerpUnclamped(r.min.x, r.max.x, p.x);
            float y = Mathf.LerpUnclamped(r.min.y, r.max.y, p.y);
            return new Vector2(x, y);
        }

        private void HandleCornerPointsDragging(TCropNode n, Rect canvas)
        {
            Vector2[] rectSpaceCorner = new Vector2[4]
            {
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, -0.5f)
            };

            Vector2[] uvSpaceCorners = new Vector2[4];
            Matrix4x4 rectToUV = n.GetRectToUvMatrix();
            for (int i = 0; i < uvSpaceCorners.Length; ++i)
            {
                uvSpaceCorners[i] = rectToUV.MultiplyPoint(rectSpaceCorner[i]);
            }

            Matrix4x4 uvToCanvas = Matrix4x4.TRS(new Vector3(canvas.position.x, canvas.position.y, 0), Quaternion.identity, new Vector3(canvas.width, canvas.height, 1));
            Vector2[] canvasSpaceCorners = new Vector2[4];
            Rect[] cornersHandleRect = new Rect[4];
            for (int i = 0; i < uvSpaceCorners.Length; ++i)
            {
                canvasSpaceCorners[i] = uvToCanvas.MultiplyPoint(FlipY(uvSpaceCorners[i]));
                cornersHandleRect[i] = new Rect() { size = Vector2.one * HANDLE_SIZE, center = canvasSpaceCorners[i] };
                EditorGUIUtility.AddCursorRect(cornersHandleRect[i], MouseCursor.MoveArrow);
            }

            for (int i = 0; i < canvasSpaceCorners.Length; ++i)
            {
                Handles.DrawSolidDisc(canvasSpaceCorners[i], Vector3.forward, HANDLE_SIZE * 0.25f);
            }
            Handles.DrawPolyLine(canvasSpaceCorners[0], canvasSpaceCorners[1], canvasSpaceCorners[2], canvasSpaceCorners[3], canvasSpaceCorners[0]);

            if (isDraggingCenterPoint)
                return;

            if (Event.current.type == EventType.MouseDown)
            {
                for (int i = 0; i < cornersHandleRect.Length; ++i)
                {
                    if (cornersHandleRect[i].Contains(Event.current.mousePosition))
                    {
                        draggedCornerIndex = i;
                        break;
                    }
                    draggedCornerIndex = -1;
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (draggedCornerIndex == -1)
                    return;
                int index = draggedCornerIndex;
                canvasSpaceCorners[index] = Event.current.mousePosition;
                uvSpaceCorners[index] = uvToCanvas.inverse.MultiplyPoint(canvasSpaceCorners[index]);
                rectSpaceCorner[index] = rectToUV.inverse.MultiplyPoint(FlipY(uvSpaceCorners[index]));

                float minX = float.MaxValue;
                float minY = float.MaxValue;
                float maxX = float.MinValue;
                float maxY = float.MinValue;
                for (int i = 0; i < rectSpaceCorner.Length; ++i)
                {
                    minX = Mathf.Min(minX, rectSpaceCorner[i].x);
                    minY = Mathf.Min(minY, rectSpaceCorner[i].y);
                    maxX = Mathf.Max(maxX, rectSpaceCorner[i].x);
                    maxY = Mathf.Max(maxY, rectSpaceCorner[i].y);
                }

                Vector2 p = rectSpaceCorner[index];
                if (p.x < 0)
                {
                    minX = p.x;
                }
                else
                {
                    maxX = p.x;
                }
                if (p.y < 0)
                {
                    minY = p.y;
                }
                else
                {
                    maxY = p.y;
                }

                Rect newRect = Rect.MinMaxRect(minX, minY, maxX, maxY);
                n.RectSize.value = new Vector2(n.RectSize.value.x * newRect.width, n.RectSize.value.y * newRect.height);
                if (Event.current.shift)
                {
                    float minSize = Mathf.Min(n.RectSize.value.x, n.RectSize.value.y);
                    n.RectSize.value = new Vector2(minSize, minSize);
                }
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                draggedCornerIndex = -1;
            }
        }

        private void HandleRotationPointsDragging(TCropNode n, Rect canvas)
        {
            if (isDraggingCenterPoint || draggedCornerIndex != -1)
                return;
            Vector2[] rectSpaceCorner = new Vector2[4]
            {
                new Vector2(-0.5f, -0.5f),
                new Vector2(-0.5f, 0.5f),
                new Vector2(0.5f, 0.5f),
                new Vector2(0.5f, -0.5f)
            };

            Vector2[] uvSpaceCorners = new Vector2[4];
            Matrix4x4 rectToUV = n.GetRectToUvMatrix();
            for (int i = 0; i < uvSpaceCorners.Length; ++i)
            {
                uvSpaceCorners[i] = rectToUV.MultiplyPoint(rectSpaceCorner[i]);
            }

            Matrix4x4 uvToCanvas = Matrix4x4.TRS(new Vector3(canvas.position.x, canvas.position.y, 0), Quaternion.identity, new Vector3(canvas.width, canvas.height, 1));
            Vector2[] canvasSpaceCorners = new Vector2[4];
            for (int i = 0; i < uvSpaceCorners.Length; ++i)
            {
                canvasSpaceCorners[i] = uvToCanvas.MultiplyPoint(FlipY(uvSpaceCorners[i]));
                //cornersHandleRect[i] = new Rect() { size = Vector2.one * HANDLE_SIZE, center = canvasSpaceCorners[i] };
                //EditorGUIUtility.AddCursorRect(cornersHandleRect[i], MouseCursor.MoveArrow);
            }

            Rect[] cornersHandleRect = new Rect[4];
            for (int i = 0; i < canvasSpaceCorners.Length; ++i)
            {
                cornersHandleRect[i] = new Rect() { size = Vector2.one * 3 * HANDLE_SIZE, center = canvasSpaceCorners[i] };
                EditorGUIUtility.AddCursorRect(cornersHandleRect[i], MouseCursor.Pan);
            }

            Vector2 uvSpaceCenter = rectToUV.MultiplyPoint(Vector3.zero);

            if (Event.current.type == EventType.MouseDown)
            {
                for (int i = 0; i < cornersHandleRect.Length; ++i)
                {
                    if (cornersHandleRect[i].Contains(Event.current.mousePosition))
                    {
                        isDraggingRotationPoint = true;
                        rotationStartPoint = FlipY(uvToCanvas.inverse.MultiplyPoint(Event.current.mousePosition));
                        break;
                    }
                    isDraggingRotationPoint = false;
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (!isDraggingRotationPoint)
                    return;
                Vector2 uvSpaceMousePos = FlipY(uvToCanvas.inverse.MultiplyPoint(Event.current.mousePosition));
                Quaternion currentRotation = Quaternion.Euler(0, 0, n.RectRotation.value);
                Quaternion fromToRotation = Quaternion.FromToRotation(rotationStartPoint - uvSpaceCenter, uvSpaceMousePos - uvSpaceCenter);
                currentRotation *= fromToRotation;
                n.RectRotation.value = currentRotation.eulerAngles.z;
                rotationStartPoint = uvSpaceMousePos;
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDraggingRotationPoint = false;
            }
        }
    }
}
