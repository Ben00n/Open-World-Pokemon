using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Poseidon
{
    public class PSplineEditingGUIDrawer
    {
        private const float BEZIER_SELECT_DISTANCE = 10;
        private const float BEZIER_WIDTH = 5;

        private PWater water;

        public int selectedAnchorIndex = -1;
        public int selectedSegmentIndex = -1;

        public PSplineEditingGUIDrawer(PWater water)
        {
            this.water = water;
        }

        public void Draw()
        {
            if (Event.current == null)
                return;

            //DrawDebug();

            HandleSelectTransformRemoveAnchor();
            HandleSelectTransformRemoveSegment();
            HandleAddAnchor();

            DrawPivot();
            DrawInstruction();
            CatchHotControl();
        }

        private void HandleSelectTransformRemoveAnchor()
        {
            List<PSplineAnchor> anchors = water.Spline.Anchors;

            for (int i = 0; i < anchors.Count; ++i)
            {
                PSplineAnchor a = anchors[i];
                Vector3 localPos = a.Position;
                Vector3 worldPos = water.transform.TransformPoint(localPos);
                float handleSize = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                if (i == selectedAnchorIndex)
                {
                    Handles.color = Handles.selectedColor;
                    Handles.SphereHandleCap(0, worldPos, Quaternion.identity, handleSize, EventType.Repaint);
                    bool isGlobalRotation = Tools.pivotRotation == PivotRotation.Global;

                    EditorGUI.BeginChangeCheck();
                    if (Tools.current == Tool.Move)
                    {
                        worldPos = Handles.PositionHandle(worldPos, isGlobalRotation ? Quaternion.identity : a.Rotation);
                        localPos = water.transform.InverseTransformPoint(worldPos);
                        a.Position = localPos;
                    }
                    else if (Tools.current == Tool.Rotate && !PSplineToolConfig.Instance.AutoTangent)
                    {
                        a.Rotation = Handles.RotationHandle(a.Rotation, worldPos);
                    }
                    else if (Tools.current == Tool.Scale)
                    {
                        a.Scale = Handles.ScaleHandle(a.Scale, worldPos, isGlobalRotation ? Quaternion.identity : a.Rotation, HandleUtility.GetHandleSize(worldPos));
                    }
                    anchors[i] = a;
                    if (EditorGUI.EndChangeCheck())
                    {
                        if (PSplineToolConfig.Instance.AutoTangent)
                        {
                            water.Spline.SmoothTangents(selectedAnchorIndex);
                        }
                        List<int> segmentIndices = water.Spline.FindSegments(selectedAnchorIndex);
                        water.GenerateSplineMeshAtSegments(segmentIndices);
                    }
                }
                else
                {
                    Handles.color = Color.cyan;
                    if (Handles.Button(worldPos, Quaternion.identity, handleSize, handleSize * 0.5f, Handles.SphereHandleCap))
                    {
                        if (Event.current.control)
                        {
                            selectedAnchorIndex = -1;
                            selectedSegmentIndex = -1;
                            water.Spline.RemoveAnchor(i);
                        }
                        else if (Event.current.shift)
                        {
                            if (selectedAnchorIndex != i &&
                                selectedAnchorIndex >= 0 &&
                                selectedAnchorIndex < anchors.Count)
                            {
                                water.Spline.AddSegment(selectedAnchorIndex, i);
                                if (PSplineToolConfig.Instance.AutoTangent)
                                {
                                    int[] segmentsIndices = water.Spline.SmoothTangents(selectedAnchorIndex, i);
                                    water.GenerateSplineMeshAtSegments(segmentsIndices);
                                }
                                else
                                {
                                    water.GenerateSplineMeshAtSegment(water.Spline.Segments.Count - 1);
                                }
                                selectedAnchorIndex = i;
                                selectedSegmentIndex = -1;
                            }
                        }
                        else
                        {
                            selectedAnchorIndex = i;
                            selectedSegmentIndex = -1;
                        }
                        Event.current.Use();
                    }
                }
            }
        }

        private void HandleAddAnchor()
        {
            bool isLeftMouseUp = Event.current.type == EventType.MouseUp && Event.current.button == 0;
            bool isShift = Event.current.shift;
            if (!isLeftMouseUp)
                return;
            int raycastLayer = PSplineToolConfig.Instance.RaycastLayer;
            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(r, out hit, 10000, LayerMask.GetMask(LayerMask.LayerToName(raycastLayer))))
            {
                if (!isShift)
                {
                    selectedAnchorIndex = -1;
                    return;
                }
                Vector3 offset = Vector3.up * PSplineToolConfig.Instance.YOffset;
                Vector3 worldPos = hit.point + offset;
                Vector3 localPos = water.transform.InverseTransformPoint(worldPos);
                PSplineAnchor a = new PSplineAnchor(localPos);
                water.Spline.AddAnchor(a);

                if (selectedAnchorIndex >= 0 && selectedAnchorIndex < water.Spline.Anchors.Count - 1)
                {
                    water.Spline.AddSegment(selectedAnchorIndex, water.Spline.Anchors.Count - 1);
                    if (PSplineToolConfig.Instance.AutoTangent)
                    {
                        int[] segmentIndices = water.Spline.SmoothTangents(selectedAnchorIndex, water.Spline.Anchors.Count - 1);
                        water.GenerateSplineMeshAtSegments(segmentIndices);
                    }
                    else
                    {
                        water.GenerateSplineMeshAtSegment(water.Spline.Segments.Count - 1);
                    }
                }

                selectedAnchorIndex = water.Spline.Anchors.Count - 1;
                Event.current.Use();
            }
            else
            {
                selectedAnchorIndex = -1;
            }
        }

        private void HandleSelectTransformRemoveSegment()
        {
            List<PSplineSegment> segments = water.Spline.Segments;
            List<PSplineAnchor> anchors = water.Spline.Anchors;
            for (int i = 0; i < segments.Count; ++i)
            {
                if (!water.Spline.IsSegmentValid(i))
                    continue;
                if (i == selectedSegmentIndex && !PSplineToolConfig.Instance.AutoTangent)
                    HandleSelectedSegmentModifications();
                int i0 = segments[i].StartIndex;
                int i1 = segments[i].EndIndex;
                PSplineAnchor a0 = anchors[i0];
                PSplineAnchor a1 = anchors[i1];
                Vector3 startPosition = water.transform.TransformPoint(a0.Position);
                Vector3 endPosition = water.transform.TransformPoint(a1.Position);
                Vector3 startTangent = water.transform.TransformPoint(segments[i].StartTangent);
                Vector3 endTangent = water.transform.TransformPoint(segments[i].EndTangent);
                Color color = (i == selectedSegmentIndex) ?
                    Handles.selectedColor :
                    Color.white;
                Color colorFade = new Color(color.r, color.g, color.b, color.a * 0.1f);

                Vector3[] bezierPoints = Handles.MakeBezierPoints(startPosition, endPosition, startTangent, endTangent, 11);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.color = color;
                Handles.DrawAAPolyLine(BEZIER_WIDTH, bezierPoints);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                Handles.color = colorFade;
                Handles.DrawAAPolyLine(BEZIER_WIDTH, bezierPoints);

                Matrix4x4 localToWorld = water.transform.localToWorldMatrix;
                Matrix4x4 splineToLocal = water.Spline.TRS(i, 0.5f);
                Matrix4x4 splineToWorld = localToWorld * splineToLocal;
                Vector3 arrow0 = splineToWorld.MultiplyPoint(Vector3.zero);
                splineToLocal = water.Spline.TRS(i, 0.45f);
                splineToWorld = localToWorld * splineToLocal;
                Vector3 arrow1 = splineToWorld.MultiplyPoint(Vector3.left * 0.5f);
                Vector3 arrow2 = splineToWorld.MultiplyPoint(Vector3.right * 0.5f);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.color = color;
                Handles.DrawAAPolyLine(BEZIER_WIDTH, arrow1, arrow0, arrow2);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                Handles.color = colorFade;
                Handles.DrawAAPolyLine(BEZIER_WIDTH, arrow1, arrow0, arrow2);

                if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
                {
                    float d0 = DistanceMouseToSpline(Event.current.mousePosition, bezierPoints);
                    float d1 = DistanceMouseToPoint(Event.current.mousePosition, bezierPoints[0]);
                    float d2 = DistanceMouseToPoint(Event.current.mousePosition, bezierPoints[bezierPoints.Length - 1]);
                    if (d0 <= BEZIER_SELECT_DISTANCE &&
                        d1 > BEZIER_SELECT_DISTANCE &&
                        d2 > BEZIER_SELECT_DISTANCE)
                    {
                        selectedSegmentIndex = i;
                        if (Event.current.control)
                        {
                            water.Spline.RemoveSegment(selectedSegmentIndex);
                            selectedSegmentIndex = -1;
                            GUI.changed = true;
                        }
                        //don't Use() the event here
                    }
                    else
                    {
                        if (selectedSegmentIndex == i)
                        {
                            selectedSegmentIndex = -1;
                        }
                    }
                }
            }
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
        }

        private void HandleSelectedSegmentModifications()
        {
            if (selectedSegmentIndex < 0 || selectedSegmentIndex >= water.Spline.Segments.Count)
                return;
            if (!water.Spline.IsSegmentValid(selectedSegmentIndex))
                return;
            PSplineSegment segment = water.Spline.Segments[selectedSegmentIndex];
            PSplineAnchor startAnchor = water.Spline.Anchors[segment.StartIndex];
            PSplineAnchor endAnchor = water.Spline.Anchors[segment.EndIndex];

            Vector3 worldStartPosition = water.transform.TransformPoint(startAnchor.Position);
            Vector3 worldEndPosition = water.transform.TransformPoint(endAnchor.Position);
            Vector3 worldStartTangent = water.transform.TransformPoint(segment.StartTangent);
            Vector3 worldEndTangent = water.transform.TransformPoint(segment.EndTangent);

            EditorGUI.BeginChangeCheck();
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            worldStartTangent = Handles.PositionHandle(worldStartTangent, Quaternion.identity);
            worldEndTangent = Handles.PositionHandle(worldEndTangent, Quaternion.identity);

            segment.StartTangent = water.transform.InverseTransformPoint(worldStartTangent);
            segment.EndTangent = water.transform.InverseTransformPoint(worldEndTangent);
            if (EditorGUI.EndChangeCheck())
            {
                water.GenerateSplineMeshAtSegment(selectedSegmentIndex);
            }

            Handles.color = Color.white;
            Handles.DrawLine(worldStartPosition, worldStartTangent);
            Handles.DrawLine(worldEndPosition, worldEndTangent);
        }

        private void DrawPivot()
        {
            Vector3 pivot = water.transform.position;
            float size = HandleUtility.GetHandleSize(pivot);

            Vector3 xStart = pivot + Vector3.left * size;
            Vector3 xEnd = pivot + Vector3.right * size;
            Handles.zTest = UnityEngine.Rendering.CompareFunction.Always;
            Handles.color = Handles.xAxisColor;
            Handles.DrawLine(xStart, xEnd);

            Vector3 yStart = pivot + Vector3.down * size;
            Vector3 yEnd = pivot + Vector3.up * size;
            Handles.color = Handles.yAxisColor;
            Handles.DrawLine(yStart, yEnd);

            Vector3 zStart = pivot + Vector3.back * size;
            Vector3 zEnd = pivot + Vector3.forward * size;
            Handles.color = Handles.zAxisColor;
            Handles.DrawLine(zStart, zEnd);
        }

        private void DrawInstruction()
        {
            string s = string.Format(
                    "{0}" +
                    "{1}" +
                    "{2}" +
                    "{3}",
                    "Click to select,",
                    "\nShift+Click to add,",
                    "\nCtrl+Click to remove anchor.",
                    "\nClick End Editing Spline when done.");

            GUIContent mouseMessage = new GUIContent(s);
            PEditorCommon.SceneViewMouseMessage(mouseMessage);
        }

        private void CatchHotControl()
        {
            int controlId = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);
            if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    GUIUtility.hotControl = controlId;
                    //OnMouseDown(Event.current);
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (GUIUtility.hotControl == controlId)
                {
                    //OnMouseUp(Event.current);
                    //Return the hot control back to Unity, use the default
                    GUIUtility.hotControl = 0;
                }
            }
            else if (Event.current.type == EventType.KeyDown)
            {
                //OnKeyDown(Event.current);
            }
        }

        public static float DistanceMouseToSpline(Vector2 mousePosition, params Vector3[] splinePoint)
        {
            float d = float.MaxValue;
            for (int i = 0; i < splinePoint.Length - 1; ++i)
            {
                float d1 = HandleUtility.DistancePointToLineSegment(
                    mousePosition,
                    HandleUtility.WorldToGUIPoint(splinePoint[i]),
                    HandleUtility.WorldToGUIPoint(splinePoint[i + 1]));
                if (d1 < d)
                    d = d1;
            }
            return d;
        }

        public static float DistanceMouseToPoint(Vector2 mousePosition, Vector3 worldPoint)
        {
            float d = Vector2.Distance(
                mousePosition,
                HandleUtility.WorldToGUIPoint(worldPoint));
            return d;
        }

    }
}
