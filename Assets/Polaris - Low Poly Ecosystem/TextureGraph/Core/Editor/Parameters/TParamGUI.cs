using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System;
namespace Pinwheel.TextureGraph
{
    public static class TParamGUI
    {
        public static TColorParameter ColorField(GUIContent guiContent, TColorParameter param)
        {
            param.value = EditorGUILayout.ColorField(guiContent, param.value);
            return param;
        }

        public static TFloatParameter FloatField(GUIContent guiContent, TFloatParameter param)
        {
            param.value = EditorGUILayout.FloatField(guiContent, param.value);
            return param;
        }

        public static TFloatParameter FloatSlider(GUIContent guiContent, TFloatParameter param, float min, float max)
        {
            param.value = EditorGUILayout.Slider(guiContent, param.value, min, max);
            return param;
        }

        public static TIntParameter IntField(GUIContent guiContent, TIntParameter param)
        {
            param.value = EditorGUILayout.IntField(guiContent, param.value);
            return param;
        }

        public static TIntParameter IntSlider(GUIContent guiContent, TIntParameter param, int min, int max)
        {
            param.value = EditorGUILayout.IntSlider(guiContent, param.value, min, max);
            return param;
        }

        public static TBoolParameter Toggle(GUIContent guiContent, TBoolParameter param)
        {
            param.value = EditorGUILayout.Toggle(guiContent, param.value);
            return param;
        }

        public static TCurveParameter CurveField(GUIContent guiContent, TCurveParameter param)
        {
            param.value = EditorGUILayout.CurveField(guiContent, param.value);
            return param;
        }

        public static TCurveParameter CurveField(GUIContent guiContent, TCurveParameter param, Color color, Rect ranges)
        {
            param.value = EditorGUILayout.CurveField(guiContent, param.value, color, ranges);
            return param;
        }

        public static TGradientParameter GradientField(GUIContent guiContent, TGradientParameter param, bool hdr = false)
        {
            param.value = EditorGUILayout.GradientField(guiContent, param.value, hdr);
            return param;
        }

        public static TStringParameter TextField(GUIContent guiContent, TStringParameter param)
        {
            param.value = EditorGUILayout.TextField(guiContent, param.value);
            return param;
        }

        public static TRectParameter RectField(GUIContent guiContent, TRectParameter param)
        {
            param.value = EditorGUILayout.RectField(guiContent, param.value);
            return param;
        }

        public static TVector2Parameter Vector2Field(GUIContent guiContent, TVector2Parameter param)
        {
            EditorGUIUtility.wideMode = true;
            param.value = EditorGUILayout.Vector2Field(guiContent, param.value);
            EditorGUIUtility.wideMode = false;
            return param;
        }

        public static TVector2Parameter Vector2Slider(GUIContent guiContent, TVector2Parameter param, float min, float max)
        {
            EditorGUIUtility.wideMode = true;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel(guiContent);
            EditorGUILayout.BeginVertical();
            float labelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 14;
            param.value.x = EditorGUILayout.Slider("X", param.value.x, min, max);
            param.value.y = EditorGUILayout.Slider("Y", param.value.y, min, max);
            EditorGUIUtility.labelWidth = labelWidth;
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.wideMode = false;
            return param;
        }
    }
}
