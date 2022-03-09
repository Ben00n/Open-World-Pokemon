using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Pinwheel.TextureGraph
{
    public static class TDrawing
    {
        public static readonly Vector2[] fullRectUvPoints = new Vector2[]
        {
            Vector2.zero,
            Vector2.up,
            Vector2.one,
            Vector2.right
        };

        public static readonly Rect unitRect = new Rect(0, 0, 1, 1);

        public static void CopyToRT(Texture t, RenderTexture rt)
        {
            RenderTexture.active = rt;
            Graphics.Blit(t, rt);
            RenderTexture.active = null;
        }

        public static void CopyFromRT(Texture2D t, RenderTexture rt)
        {
            RenderTexture.active = rt;
            t.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            t.Apply();
            RenderTexture.active = null;
        }

        public static void CopyTexture(Texture src, Texture2D des)
        {
            RenderTexture rt;
            rt = new RenderTexture(des.width, des.height, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear); //Create a new RT because src and des may have different resolution
            CopyToRT(src, rt);
            CopyFromRT(des, rt);
            rt.Release();
            TUtilities.DestroyObject(rt);
        }

        public static Texture2D CloneTexture(Texture2D t)
        {
            RenderTexture rt = new RenderTexture(t.width, t.height, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
            CopyToRT(t, rt);
            Texture2D result = new Texture2D(t.width, t.height, TextureFormat.ARGB32, false, true);
            result.filterMode = t.filterMode;
            result.wrapMode = t.wrapMode;
            CopyFromRT(result, rt);
            rt.Release();
            Object.DestroyImmediate(rt);
            return result;
        }

        public static void FillTexture(Texture2D t, Color c)
        {
            Color[] colors = new Color[t.width * t.height];
            TUtilities.Fill(colors, c);
            t.SetPixels(colors);
            t.Apply();
        }

        public static void FillTexture(RenderTexture rt, Color c)
        {
            Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            tex.SetPixel(0, 0, c);
            tex.Apply();
            CopyToRT(tex, rt);
            TUtilities.DestroyObject(tex);
        }

        public static Texture2D CloneAndResizeTexture(Texture2D t, int width, int height)
        {
            RenderTexture rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
            CopyToRT(t, rt);
            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
            result.filterMode = t.filterMode;
            result.wrapMode = t.wrapMode;
            CopyFromRT(result, rt);
            rt.Release();
            Object.DestroyImmediate(rt);
            return result;
        }

        public static void DrawQuad(RenderTexture rt, Vector2[] quadCorners, Material mat, int pass)
        {
            RenderTexture.active = rt;
            GL.PushMatrix();
            mat.SetPass(pass);
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.TexCoord(new Vector3(0, 0, 0));
            GL.Vertex3(quadCorners[0].x, quadCorners[0].y, 0);
            GL.TexCoord(new Vector3(0, 1, 0));
            GL.Vertex3(quadCorners[1].x, quadCorners[1].y, 0);
            GL.TexCoord(new Vector3(1, 1, 0));
            GL.Vertex3(quadCorners[2].x, quadCorners[2].y, 0);
            GL.TexCoord(new Vector3(1, 0, 0));
            GL.Vertex3(quadCorners[3].x, quadCorners[3].y, 0);
            GL.End();
            GL.PopMatrix();
            RenderTexture.active = null;
        }

        public static void DrawFullQuad(RenderTexture rt, Material mat, int pass)
        {
            DrawQuad(rt, fullRectUvPoints, mat, pass);
        }

        public static void DrawQuad(RenderTexture rt, Vector2[] quadCorners, Vector2[] uvs, Material mat, int pass)
        {
            RenderTexture.active = rt;
            GL.PushMatrix();
            mat.SetPass(pass);
            GL.LoadOrtho();
            GL.Begin(GL.QUADS);
            GL.TexCoord(uvs[0]);
            GL.Vertex3(quadCorners[0].x, quadCorners[0].y, 0);
            GL.TexCoord(uvs[1]);
            GL.Vertex3(quadCorners[1].x, quadCorners[1].y, 0);
            GL.TexCoord(uvs[2]);
            GL.Vertex3(quadCorners[2].x, quadCorners[2].y, 0);
            GL.TexCoord(uvs[3]);
            GL.Vertex3(quadCorners[3].x, quadCorners[3].y, 0);
            GL.End();
            GL.PopMatrix();
            RenderTexture.active = null;
        }

        public static void DrawDoubleTris(RenderTexture rt, Vector2[] quadCorners, Vector2[] uvs, Material mat, int pass)
        {
            RenderTexture.active = rt;
            GL.PushMatrix();
            mat.SetPass(pass);
            GL.LoadOrtho();
            GL.Begin(GL.TRIANGLES);
            GL.TexCoord(uvs[0]);
            GL.Vertex3(quadCorners[0].x, quadCorners[0].y, 0);
            GL.TexCoord(uvs[1]);
            GL.Vertex3(quadCorners[1].x, quadCorners[1].y, 0);
            GL.TexCoord(uvs[2]);
            GL.Vertex3(quadCorners[2].x, quadCorners[2].y, 0);
            GL.TexCoord(uvs[3]);
            GL.Vertex3(quadCorners[3].x, quadCorners[3].y, 0);
            GL.TexCoord(uvs[4]);
            GL.Vertex3(quadCorners[4].x, quadCorners[4].y, 0);
            GL.TexCoord(uvs[5]);
            GL.Vertex3(quadCorners[5].x, quadCorners[5].y, 0);
            GL.End();
            GL.PopMatrix();
            RenderTexture.active = null;
        }
    }
}
