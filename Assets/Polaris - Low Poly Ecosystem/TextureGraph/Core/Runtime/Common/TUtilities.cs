using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Experimental.Rendering;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Pinwheel.TextureGraph
{
    public static class TUtilities
    {
        public static void DestroyObject(Object o)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
                Object.Destroy(o);
            else
                Object.DestroyImmediate(o, true);
#else
            GameObject.Destroy(o);
#endif
        }

        public static void DestroyGameobject(GameObject g)
        {
#if UNITY_EDITOR
            if (UnityEditor.EditorApplication.isPlaying)
                GameObject.Destroy(g);
            else
                GameObject.DestroyImmediate(g);
#else
            GameObject.Destroy(g);
#endif
        }

        public static void Fill<T>(T[] array, T value)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] = value;
            }
        }

        public static Color GetColor(Color baseColor, float alpha)
        {
            return new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        }

        public static Gradient CreateLinearBWGradient()
        {
            Gradient g = new Gradient();
            g.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.black, 0), new GradientColorKey(Color.white, 1) },
                new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) });
            return g;
        }

        public static int[] GetIndicesArray(int length)
        {
            int[] indices = new int[length];
            for (int i = 0; i < length; ++i)
            {
                indices[i] = i;
            }
            return indices;
        }

        public static int[] GetIndicesArray(int from, int to)
        {
            int length = to - from + 1;
            int[] indices = new int[length];
            for (int i = 0; i < length; ++i)
            {
                indices[i] = from + i;
            }
            return indices;
        }

        public static bool IsGrayscaleFormat(GraphicsFormat format)
        {
            return format == GraphicsFormat.R8_UNorm || format == GraphicsFormat.R16_UNorm || format == GraphicsFormat.R32_SFloat;
        }

        public static string ListElementsToString<T>(this IEnumerable<T> list, string separator)
        {
            IEnumerator<T> i = list.GetEnumerator();
            System.Text.StringBuilder s = new System.Text.StringBuilder();
            if (i.MoveNext())
                s.Append(i.Current.ToString());
            while (i.MoveNext())
                s.Append(separator).Append(i.Current.ToString());
            return s.ToString();
        }

        public static float ApplyThreshold(float value, float threshold)
        {
            if (value > threshold)
                return 1;
            else
                return value;
        }
    }
}
