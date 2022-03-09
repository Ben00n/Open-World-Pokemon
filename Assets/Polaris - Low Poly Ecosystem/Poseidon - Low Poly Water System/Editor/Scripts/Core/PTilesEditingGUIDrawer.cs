using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.Poseidon
{
    public class PTilesEditingGUIDrawer
    {
        private PWater water;

        public PTilesEditingGUIDrawer(PWater water)
        {
            this.water = water;
        }

        public void Draw()
        {
            if (Event.current == null)
                return;
            DrawExistingTiles();
            HandleAddRemoveTiles();
            CatchHotControl();            
        }

        private void DrawExistingTiles()
        {
            for (int i = 0; i < water.TileIndices.Count; ++i)
            {
                PIndex2D index = water.TileIndices[i];
                Vector3 rectCenter = water.transform.TransformPoint(new Vector3(
                    (index.X + 0.5f) * water.TileSize.x,
                    0,
                    (index.Z + 0.5f) * water.TileSize.y));
                Vector3 rectSize = water.transform.TransformVector(new Vector3(
                    water.TileSize.x,
                    0,
                    water.TileSize.y));

                Handles.color = new Color(1, 1, 1, 0.05f);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.Greater;
                Handles.DrawWireCube(rectCenter, rectSize);

                Handles.color = new Color(1, 1, 1, 1);
                Handles.zTest = UnityEngine.Rendering.CompareFunction.LessEqual;
                Handles.DrawWireCube(rectCenter, rectSize);
            }
        }

        private void HandleAddRemoveTiles()
        {
            Plane plane = new Plane(Vector3.up, water.transform.position);
            Ray r = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
            float distance = -1;
            if (plane.Raycast(r, out distance))
            {
                Vector3 hitWorldPos = r.origin + r.direction * distance;
                Vector3 hitLocalPos = water.transform.InverseTransformPoint(hitWorldPos);
                PIndex2D index = new PIndex2D(
                    Mathf.FloorToInt(hitLocalPos.x / water.TileSize.x),
                    Mathf.FloorToInt(hitLocalPos.z / water.TileSize.y));

                Vector3 rectCenter = water.transform.TransformPoint(new Vector3(
                    (index.X + 0.5f) * water.TileSize.x,
                    hitLocalPos.y,
                    (index.Z + 0.5f) * water.TileSize.y));
                Vector3 rectSize = water.transform.TransformVector(new Vector3(
                    water.TileSize.x,
                    0,
                    water.TileSize.y));

                if (Event.current.type == EventType.MouseDrag || Event.current.type == EventType.MouseUp)
                {
                    if (Event.current.button != 0)
                        return; 
                    if (Event.current.control)
                    {
                        water.TileIndices.Remove(index);
                        water.ReCalculateBounds();
                    }
                    else if (Event.current.shift)
                    {
                        if (!water.TileIndices.Contains(index))
                        {
                            water.TileIndices.Add(index);
                            water.ReCalculateBounds();
                        }
                    }
                    PUtilities.MarkCurrentSceneDirty();
                    EditorUtility.SetDirty(water);
                }

                Handles.color = Handles.selectedColor;
                Handles.DrawWireCube(rectCenter, rectSize);

                string s = string.Format(
                    "{0}" +
                    "{1}" +
                    "{2}",
                    index.ToString(),
                    "\nShift+Click to pin, Ctrl+Click to unpin water planes.",
                    "\nClick End Editing Tiles when done.");

                GUIContent mouseMessage = new GUIContent(s);
                PEditorCommon.SceneViewMouseMessage(mouseMessage);
            }
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
