using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.SocialPlatforms;

namespace Pinwheel.Poseidon
{
    public class PAreaEditingGUIDrawer
    {
        private PWater water;

        private int selectedAnchorIndex;

        public PAreaEditingGUIDrawer(PWater water)
        {
            this.water = water;
            selectedAnchorIndex = -1;
        }

        public void Draw()
        {
            if (Event.current == null)
                return;
            HandleSelectTranslateRemoveAnchors();
            HandleAddAnchor();
            DrawInstruction();
            CatchHotControl();
        }

        private void HandleSelectTranslateRemoveAnchors()
        {
            List<Vector3> localPositions = water.AreaMeshAnchors;
            if (localPositions.Count == 0)
                return;
            if (localPositions.Count >= 2)
            {
                List<Vector3> worldPositions = new List<Vector3>();
                for (int i = 0; i < localPositions.Count; ++i)
                {
                    worldPositions.Add(water.transform.TransformPoint(localPositions[i]));
                }
                Handles.DrawPolyLine(worldPositions.ToArray());
                Handles.DrawLine(worldPositions[0], worldPositions[worldPositions.Count - 1]);
            }

            for (int i = 0; i < localPositions.Count; ++i)
            {
                Vector3 localPos = localPositions[i];
                Vector3 worldPos = water.transform.TransformPoint(localPos);
                float handleSize = HandleUtility.GetHandleSize(worldPos) * 0.2f;
                if (i == selectedAnchorIndex)
                {
                    Handles.color = Handles.selectedColor;
                    Handles.SphereHandleCap(0, worldPos, Quaternion.identity, handleSize, EventType.Repaint);
                    worldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                    localPos = water.transform.InverseTransformPoint(worldPos);
                    localPos.y = 0;
                    localPositions[i] = localPos;
                }
                else
                {
                    Handles.color = Color.cyan;
                    if (Handles.Button(worldPos, Quaternion.identity, handleSize, handleSize * 0.5f, Handles.SphereHandleCap))
                    {
                        if (Event.current.control)
                        {
                            selectedAnchorIndex = -1;
                            localPositions.RemoveAt(i);
                        }
                        else
                        {
                            selectedAnchorIndex = i;
                        }
                    }
                }
            }

            if (Event.current.type == EventType.MouseUp && Event.current.button == 0)
            {
                selectedAnchorIndex = -1;
            }
        }

        private void HandleAddAnchor()
        {
            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                Plane plane = new Plane(Vector3.up, water.transform.position);
                Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
                float distance = -1;
                if (plane.Raycast(r, out distance))
                {
                    Vector3 hitWorldPos = r.origin + r.direction * distance;
                    Vector3 hitLocalPos = water.transform.InverseTransformPoint(hitWorldPos);
                    if (Event.current.shift)
                    {
                        water.AreaMeshAnchors.Add(hitLocalPos);
                    }
                }
            }
        }

        private void VisualizeIntersections()
        {
            if (water.AreaMeshAnchors.Count < 3)
                return;

            Plane plane = new Plane(Vector3.up, water.transform.position);
            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float distance = -1;
            if (plane.Raycast(r, out distance))
            {
                Vector3 hitWorldPos = r.origin + r.direction * distance;

                float lineY = hitWorldPos.z;
                Handles.color = Color.red;
                Handles.DrawLine(new Vector3(-100, 0, lineY), new Vector3(100, 0, lineY));

                List<Vector3> intersections = new List<Vector3>();
                List<Vector3> anchors = new List<Vector3>(water.AreaMeshAnchors);
                anchors.Add(water.AreaMeshAnchors[0]);
                for (int i = 0; i < anchors.Count - 1; ++i)
                {
                    Vector3 a0 = anchors[i];
                    Vector3 a1 = anchors[i + 1];
                    Vector2 inter;
                    if (PGeometryUtilities.IsIntersectHorizontalLine(a0.x, a0.z, a1.x, a1.z, lineY, out inter))
                    {
                        intersections.Add(new Vector3(inter.x, 0, inter.y));
                    }
                }

                for (int i = 0; i < intersections.Count; ++i)
                {
                    Handles.color = Color.red;
                    Handles.CubeHandleCap(0, intersections[i], Quaternion.identity, 1, EventType.Repaint);
                }
            }
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
                    "\nClick End Editing Tiles when done.");

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
    }
}
