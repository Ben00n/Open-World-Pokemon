using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    public static class TLevelsEditorDrawer
    {
        public enum THistogramChannel
        {
            Luminance = -1,
            Red = 0,
            Green = 1,
            Blue = 2,
            Alpha = 3
        }

        private static readonly ComputeShader histogramShader = Resources.Load<ComputeShader>("TextureGraph/Shaders/HistogramLuminance");
        private static readonly int HISTOGRAM_BUFFER_LENGTH = 256;
        private const string KERNEL_INIT = "Init";
        private const string KERNEL_COMPUTE_HISTOGRAM_LUMINANCE = "ComputeHistogramLuminance";
        private const string KERNEL_COMPUTE_HISTOGRAM_CHANNEL = "ComputeHistogramChannel";
        private const string BUFFER_HISTOGRAM = "_Histogram";
        private const string BUFFER_MAX = "_Max";
        private const string TEXTURE_TO_COMPUTE_HISTOGRAM = "_Texture";
        private const string MAX_HISTOGRAM_VALUE = "_Max";
        private const string CHANNEL = "_Channel";
        private const string COLOR = "_Color";

        private static readonly float HANDLE_SIZE = 10;
        private static readonly Texture HANDLE_ICON = Resources.Load<Texture>("TextureGraph/Textures/UpArrow");

        private static bool isDraggingInLow = false;
        private static bool isDraggingInMid = false;
        private static bool isDraggingInHigh = false;

        private static bool isDraggingOutLow = false;
        private static bool isDraggingOutHigh = false;

        private static Material outGradientMaterial;
        private static Material OutGradientMaterial
        {
            get
            {
                if (outGradientMaterial == null)
                {
                    outGradientMaterial = new Material(Shader.Find("Hidden/TextureGraph/GradientLinear"));
                }
                outGradientMaterial.SetMatrix("_UvToGradientMatrix", Matrix4x4.identity);
                outGradientMaterial.SetFloat("_MidPoint", 1);
                return outGradientMaterial;
            }
        }

        private static Material histogramGraphMaterial;
        private static Material HistogramGraphMaterial
        {
            get
            {
                if (histogramGraphMaterial == null)
                {
                    histogramGraphMaterial = new Material(Shader.Find("Hidden/TextureGraph/HistogramGraph"));
                }
                return histogramGraphMaterial;
            }
        }

        public static TLevelsNode.TLevelsControlParam Draw(TLevelsNode.TLevelsControlParam param, Texture texture, THistogramChannel channel, Color histogramColor)
        {
            Rect box = EditorGUILayout.BeginVertical();
            Rect histogramCanvas = EditorGUILayout.GetControlRect(GUILayout.Height(128));
            TEditorCommon.DrawOutlineBox(histogramCanvas, TEditorCommon.midGrey);
            ComputeAndDrawHistogram(histogramCanvas, texture, channel, histogramColor);

            Rect inRemapCanvas = EditorGUILayout.GetControlRect(GUILayout.Height(HANDLE_SIZE));
            DrawInputRemapSlider(inRemapCanvas, param);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            param.value.inLow = EditorGUILayout.FloatField(param.value.inLow, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            param.value.inMid = EditorGUILayout.FloatField(param.value.inMid, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            param.value.inHigh = EditorGUILayout.FloatField(param.value.inHigh, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
            Rect outRemapGradientRect = EditorGUILayout.GetControlRect();
            EditorGUI.DrawPreviewTexture(outRemapGradientRect, Texture2D.whiteTexture, OutGradientMaterial);
            Rect outRemapCanvas = EditorGUILayout.GetControlRect(GUILayout.Height(HANDLE_SIZE));
            DrawOutputRemapSlider(outRemapCanvas, param);

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            param.value.outLow = EditorGUILayout.FloatField(param.value.outLow, GUILayout.Width(50));
            GUILayout.FlexibleSpace();
            param.value.outHigh = EditorGUILayout.FloatField(param.value.outHigh, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            return param;
        }

        private static void ComputeAndDrawHistogram(Rect canvas, Texture tex, THistogramChannel channel, Color color)
        {
            if (histogramShader == null)
                return;
            int initKernel = histogramShader.FindKernel(KERNEL_INIT);
            int computeHistogramKernel = histogramShader.FindKernel(channel == THistogramChannel.Luminance ? KERNEL_COMPUTE_HISTOGRAM_LUMINANCE : KERNEL_COMPUTE_HISTOGRAM_CHANNEL);
            ComputeBuffer histogramBuffer = new ComputeBuffer(HISTOGRAM_BUFFER_LENGTH, sizeof(uint));
            ComputeBuffer maxBuffer = new ComputeBuffer(1, sizeof(uint));
            maxBuffer.SetData(new uint[] { 0 });

            histogramShader.SetBuffer(initKernel, BUFFER_HISTOGRAM, histogramBuffer);
            histogramShader.SetBuffer(initKernel, BUFFER_MAX, maxBuffer);
            histogramShader.SetTexture(computeHistogramKernel, TEXTURE_TO_COMPUTE_HISTOGRAM, tex);
            histogramShader.SetBuffer(computeHistogramKernel, BUFFER_HISTOGRAM, histogramBuffer);
            histogramShader.SetBuffer(computeHistogramKernel, BUFFER_MAX, maxBuffer);
            histogramShader.SetInt(CHANNEL, (int)channel);
            histogramShader.Dispatch(initKernel, HISTOGRAM_BUFFER_LENGTH / 64, 1, 1);
            histogramShader.Dispatch(computeHistogramKernel, (tex.width + 7) / 8, (tex.height + 7) / 8, 1);

            uint[] max = new uint[1];
            maxBuffer.GetData(max);
            HistogramGraphMaterial.SetColor(COLOR, color);
            HistogramGraphMaterial.SetFloat(MAX_HISTOGRAM_VALUE, max[0]);
            HistogramGraphMaterial.SetBuffer(BUFFER_HISTOGRAM, histogramBuffer);
            EditorGUI.DrawPreviewTexture(new RectOffset(1, 1, 1, 1).Remove(canvas), tex, HistogramGraphMaterial);
            histogramBuffer.Dispose();
            maxBuffer.Dispose();
        }

        private static void DrawInputRemapSlider(Rect canvas, TLevelsNode.TLevelsControlParam param)
        {
            float inLow = param.value.inLow;
            float inMid = Mathf.Lerp(param.value.inLow, param.value.inHigh, param.value.inMid);
            float inHigh = param.value.inHigh;

            Rect inLowRect = new Rect()
            {
                size = Vector2.one * HANDLE_SIZE,
                center = new Vector2(Mathf.Lerp(canvas.min.x, canvas.max.x, inLow), canvas.center.y)
            };

            Rect inMidRect = new Rect()
            {
                size = Vector2.one * HANDLE_SIZE,
                center = new Vector2(Mathf.Lerp(canvas.min.x, canvas.max.x, inMid), canvas.center.y)
            };

            Rect inHighRect = new Rect()
            {
                size = Vector2.one * HANDLE_SIZE,
                center = new Vector2(Mathf.Lerp(canvas.min.x, canvas.max.x, inHigh), canvas.center.y)
            };

            GUI.DrawTexture(inLowRect, HANDLE_ICON, ScaleMode.ScaleToFit, true, 1, Color.black, 0, 0);
            GUI.DrawTexture(inMidRect, HANDLE_ICON, ScaleMode.ScaleToFit, true, 1, Color.gray, 0, 0);
            GUI.DrawTexture(inHighRect, HANDLE_ICON, ScaleMode.ScaleToFit, true, 1, Color.white, 0, 0);

            if (Event.current.type == EventType.MouseDown)
            {
                isDraggingInLow = inLowRect.Contains(Event.current.mousePosition);
                isDraggingInMid = inMidRect.Contains(Event.current.mousePosition);
                isDraggingInHigh = inHighRect.Contains(Event.current.mousePosition);
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (isDraggingInLow)
                {
                    inLow = Rect.PointToNormalized(canvas, Event.current.mousePosition).x;
                    param.value.inLow = inLow;
                    GUI.changed = true;
                }
                else if (isDraggingInMid)
                {
                    inMid = Rect.PointToNormalized(canvas, Event.current.mousePosition).x;
                    param.value.inMid = Mathf.InverseLerp(inLow, inHigh, inMid);
                    GUI.changed = true;
                }
                else if (isDraggingInHigh)
                {
                    inHigh = Rect.PointToNormalized(canvas, Event.current.mousePosition).x;
                    param.value.inHigh = inHigh;
                    GUI.changed = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDraggingInLow = false;
                isDraggingInMid = false;
                isDraggingInHigh = false;
            }
        }

        private static void DrawOutputRemapSlider(Rect canvas, TLevelsNode.TLevelsControlParam param)
        {
            float outLow = param.value.outLow;
            float outHigh = param.value.outHigh;

            Rect outLowRect = new Rect()
            {
                size = Vector2.one * HANDLE_SIZE,
                center = new Vector2(Mathf.Lerp(canvas.min.x, canvas.max.x, outLow), canvas.center.y)
            };

            Rect outHighRect = new Rect()
            {
                size = Vector2.one * HANDLE_SIZE,
                center = new Vector2(Mathf.Lerp(canvas.min.x, canvas.max.x, outHigh), canvas.center.y)
            };

            GUI.DrawTexture(outLowRect, HANDLE_ICON, ScaleMode.ScaleToFit, true, 1, Color.black, 0, 0);
            GUI.DrawTexture(outHighRect, HANDLE_ICON, ScaleMode.ScaleToFit, true, 1, Color.white, 0, 0);

            if (Event.current.type == EventType.MouseDown)
            {
                isDraggingOutLow = outLowRect.Contains(Event.current.mousePosition);
                isDraggingOutHigh = outHighRect.Contains(Event.current.mousePosition);
            }
            else if (Event.current.type == EventType.MouseDrag)
            {
                if (isDraggingOutLow)
                {
                    outLow = Rect.PointToNormalized(canvas, Event.current.mousePosition).x;
                    param.value.outLow = outLow;
                    GUI.changed = true;
                }
                else if (isDraggingOutHigh)
                {
                    outHigh = Rect.PointToNormalized(canvas, Event.current.mousePosition).x;
                    param.value.outHigh = outHigh;
                    GUI.changed = true;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                isDraggingOutLow = false;
                isDraggingOutHigh = false;
            }
        }
    }
}
