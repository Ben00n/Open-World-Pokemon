using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pinwheel.TextureGraph;
using System.Reflection;
using System;
using UnityEditor;

namespace Pinwheel.TextureGraph
{
    [InitializeOnLoad]
    public class TDefaultParamsDrawer : TParametersDrawer
    {
        public override void DrawGUI(TAbstractTextureNode target)
        {
            List<FieldInfo> fields = GetSerializedFields(target.GetType());
            if (fields.Count == 0)
            {
                EditorGUILayout.LabelField("There is no parameter.", TEditorCommon.WordWrapItalicLabel);
            }
            else
            {
                foreach (FieldInfo f in fields)
                {
                    DrawField(target, f);
                }
            }
        }


        private static Dictionary<Type, List<FieldInfo>> serializedFieldsByType;
        private static Dictionary<Type, List<FieldInfo>> SerializedFieldsByType
        {
            get
            {
                if (serializedFieldsByType == null)
                {
                    serializedFieldsByType = new Dictionary<Type, List<FieldInfo>>();
                }
                return serializedFieldsByType;
            }
            set
            {
                serializedFieldsByType = value;
            }
        }

        [InitializeOnLoadMethod]
        private static void Init()
        {
            SerializedFieldsByType.Clear();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                Type[] types = assembly.GetTypes();
                foreach (Type t in types)
                {
                    if (t.IsSubclassOf(typeof(TAbstractTextureNode)))
                    {
                        CacheSerializedMembers(t);
                    }
                }
            }
        }

        private static void CacheSerializedMembers(Type t)
        {
            List<FieldInfo> result = new List<FieldInfo>();

            FieldInfo[] publicFields = t.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (FieldInfo m in publicFields)
            {
                Type fieldType = m.FieldType;
                if (!fieldType.IsSubclassOf(typeof(UnityEngine.Object)) &&
                    fieldType.GetCustomAttribute<SerializableAttribute>() == null)
                    continue;

                if (m.GetCustomAttribute<NonSerializedAttribute>() == null &&
                    m.GetCustomAttribute<HideInInspector>() == null)
                {
                    result.Add(m);
                }
            }

            FieldInfo[] nonPublicFields = t.GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (FieldInfo m in nonPublicFields)
            {
                if (m.GetCustomAttribute<SerializeField>() != null &&
                    m.GetCustomAttribute<HideInInspector>() == null)
                {
                    result.Add(m);
                }
            }

            SerializedFieldsByType.Add(t, result);
        }

        private static List<FieldInfo> GetSerializedFields(Type t)
        {
            List<FieldInfo> fields = null;
            SerializedFieldsByType.TryGetValue(t, out fields);
            if (fields == null)
            {
                fields = new List<FieldInfo>();
            }
            return fields;
        }

        private static void DrawField(TAbstractTextureNode target, FieldInfo f)
        {
            GUIContent label = new GUIContent(ObjectNames.NicifyVariableName(f.Name));
            object value = f.GetValue(target);

            EditorGUI.BeginChangeCheck();
            if (f.FieldType == typeof(int))
            {
                value = EditorGUILayout.IntField(label, (int)value);
            }
            else if (f.FieldType == typeof(float))
            {
                value = EditorGUILayout.FloatField(label, (float)value);
            }
            else if (f.FieldType == typeof(string))
            {
                value = EditorGUILayout.TextField(label, (string)value);
            }
            else if (f.FieldType == typeof(Vector2))
            {
                value = EditorGUILayout.Vector2Field(label, (Vector2)value);
            }
            else if (f.FieldType == typeof(Vector3))
            {
                value = EditorGUILayout.Vector3Field(label, (Vector3)value);
            }
            else if (f.FieldType == typeof(Vector4))
            {
                value = EditorGUILayout.Vector4Field(label, (Vector4)value);
            }
            else if (f.FieldType == typeof(AnimationCurve))
            {
                value = EditorGUILayout.CurveField(label, (AnimationCurve)value);
            }
            else if (f.FieldType == typeof(Gradient))
            {
                value = EditorGUILayout.GradientField(label, (Gradient)value);
            }
            else if (f.FieldType == typeof(Color))
            {
                value = EditorGUILayout.ColorField(label, (Color)value);
            }
            else if (f.FieldType == typeof(Color32))
            {
                value = EditorGUILayout.ColorField(label, (Color32)value);
            }
            else if (f.FieldType == typeof(Rect))
            {
                value = EditorGUILayout.RectField(label, (Rect)value);
            }
            else if (f.FieldType.IsSubclassOf(typeof(UnityEngine.Object)))
            {
                value = EditorGUILayout.ObjectField(label, (UnityEngine.Object)value, f.FieldType, true);
            }
            else if (f.FieldType.IsEnum)
            {
                value = EditorGUILayout.EnumPopup(label, (Enum)value);
            }
            else if (f.FieldType == typeof(TColorParameter))
            {
                value = TParamGUI.ColorField(label, (TColorParameter)value);
            }
            else if (f.FieldType == typeof(TFloatParameter))
            {
                value = TParamGUI.FloatField(label, (TFloatParameter)value);
            }
            else if (f.FieldType == typeof(TIntParameter))
            {
                value = TParamGUI.IntField(label, (TIntParameter)value);
            }
            else if (f.FieldType == typeof(TBoolParameter))
            {
                value = TParamGUI.Toggle(label, (TBoolParameter)value);
            }
            else if (f.FieldType == typeof(TCurveParameter))
            {
                value = TParamGUI.CurveField(label, (TCurveParameter)value);
            }
            else if (f.FieldType == typeof(TGradientParameter))
            {
                value = TParamGUI.GradientField(label, (TGradientParameter)value);
            }
            else if (f.FieldType == typeof(TStringParameter))
            {
                value = TParamGUI.TextField(label, (TStringParameter)value);
            }
            else if (f.FieldType == typeof(TRectParameter))
            {
                value = TParamGUI.RectField(label, (TRectParameter)value);
            }
            else if (f.FieldType == typeof(TVector2Parameter))
            {
                value = TParamGUI.Vector2Field(label, (TVector2Parameter)value);
            }
            else
            {
                EditorGUILayout.LabelField(string.Format("Field {0} of type {1}", label.text, f.FieldType.Name));
            }

            if (EditorGUI.EndChangeCheck())
            {
                f.SetValue(target, value);
            }
        }
    }
}
