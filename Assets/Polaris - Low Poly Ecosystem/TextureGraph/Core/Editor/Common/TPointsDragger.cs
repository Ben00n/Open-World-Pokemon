using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;

namespace Pinwheel.TextureGraph
{
    public class TPointsDragger
    {
        public struct TArguments
        {
            public Texture background;
            public Material backgroundMaterial;
            public Action<Vector2> pointGizmosFunction;
            public Action<Vector2[]> drawConnectorFunction;
            public Color gizmosColor;

            public static TArguments Create()
            {
                TArguments args = new TArguments()
                {
                    background = null,
                    backgroundMaterial = null,
                    pointGizmosFunction = null,
                    drawConnectorFunction = null,
                    gizmosColor = new Color(0f, 0.9f, 0f, 1f),
                };
                return args;
            }
        }

        private static readonly float HANDLE_SIZE = 10;
        private static int draggedPointsIndex = -1;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="background"></param>
        /// <param name="points">In UV space</param>
        public static void DrawCanvas(TArguments args, params TVector2Parameter[] points)
        {
            Rect r = GUILayoutUtility.GetAspectRect(1);
            if (args.background != null)
            {
                EditorGUI.DrawPreviewTexture(r, args.background, args.backgroundMaterial);
            }
            else
            {
                EditorGUI.DrawRect(r, Color.black);
            }
            GUI.BeginClip(r);
            Rect canvas = new Rect(0, 0, r.width, r.height);
            Handles.BeginGUI();
            Color color = Handles.color;
            Handles.color = args.gizmosColor;
            HandlePointDragging(canvas, args, points);
            Handles.color = color;
            Handles.EndGUI();
            GUI.EndClip();
            TEditorCommon.DrawOutlineBox(r, TEditorCommon.boxBorderColor);
        }

        private static Vector2 FlipY(Vector2 v)
        {
            return new Vector2(v.x, 1 - v.y);
        }

        private static void HandlePointDragging(Rect canvas, TArguments args, params TVector2Parameter[] points)
        {
            Vector2[] uvSpaceCorners = new Vector2[points.Length];
            for (int i = 0; i < uvSpaceCorners.Length; ++i)
            {
                uvSpaceCorners[i] = points[i].value;
            }

            Matrix4x4 uvToCanvas = Matrix4x4.TRS(new Vector3(canvas.position.x, canvas.position.y, 0), Quaternion.identity, new Vector3(canvas.width, canvas.height, 1));
            Vector2[] canvasSpaceCorners = new Vector2[points.Length];
            Rect[] cornersHandleRect = new Rect[points.Length];
            for (int i = 0; i < uvSpaceCorners.Length; ++i)
            {
                canvasSpaceCorners[i] = uvToCanvas.MultiplyPoint(FlipY(uvSpaceCorners[i]));
                cornersHandleRect[i] = new Rect() { size = Vector2.one * HANDLE_SIZE, center = canvasSpaceCorners[i] };
                EditorGUIUtility.AddCursorRect(cornersHandleRect[i], MouseCursor.MoveArrow);
            }

            for (int i = 0; i < canvasSpaceCorners.Length; ++i)
            {
                if (args.pointGizmosFunction != null)
                {
                    args.pointGizmosFunction.Invoke(canvasSpaceCorners[i]);
                }
                else
                {
                    Handles.DrawSolidDisc(canvasSpaceCorners[i], Vector3.forward, HANDLE_SIZE * 0.5f);
                }
            }

            if (args.drawConnectorFunction != null)
            {
                args.drawConnectorFunction.Invoke(canvasSpaceCorners);
            }
            else
            {
                Vector3[] p = new Vector3[canvasSpaceCorners.Length + 1];
                for (int i = 0; i < p.Length-1; ++i)
                {
                    p[i] = canvasSpaceCorners[i];
                }
                p[p.Length - 1] = canvasSpaceCorners[0];
                Handles.DrawPolyLine(p);
            }
            if (Event.current.type == EventType.MouseDown)
            {
                for (int i = 0; i < cornersHandleRect.Length; ++i)
                {
                    if (cornersHandleRect[i].Contains(Event.current.mousePosition))
                    {
                        draggedPointsIndex = i;
                        break;
                    }
                    draggedPointsIndex = -1;
                }
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (draggedPointsIndex == -1)
                    return;
                int index = draggedPointsIndex;
                points[index].value = FlipY(uvToCanvas.inverse.MultiplyPoint(Event.current.mousePosition));

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                draggedPointsIndex = -1;
            }
        }
    }
}
