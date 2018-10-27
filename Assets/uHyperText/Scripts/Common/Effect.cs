using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WXB
{
    static public class Effect
    {
        static void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }

        static void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            UIVertex vt;

            var neededCapacity = verts.Count + end - start;
            if (verts.Capacity < neededCapacity)
                verts.Capacity = neededCapacity;

            for (int i = start; i < end; ++i)
            {
                vt = verts[i];
                verts.Add(vt);

                Vector3 v = vt.position;
                v.x += x;
                v.y += y;
                vt.position = v;
                var newColor = color;
                newColor.a = (byte)((newColor.a * verts[i].color.a) / 255);
                vt.color = newColor;
                verts[i] = vt;
            }
        }

        static public void Shadow(VertexHelper vh, int start, Color effectColor, Vector2 effectDistance)
        {
            var output = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(output);

            ApplyShadow(output, effectColor, start, output.Count, effectDistance.x, effectDistance.y);
            vh.Clear();
            vh.AddUIVertexTriangleStream(output);
            ListPool<UIVertex>.Release(output);
        }

        static public void Outline(VertexHelper vh, int start, Color effectColor, Vector2 effectDistance)
        {
            var verts = ListPool<UIVertex>.Get();
            vh.GetUIVertexStream(verts);

            var neededCpacity = (verts.Count - start) * 5;
            if (verts.Capacity < neededCpacity)
                verts.Capacity = neededCpacity;

            var end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, effectDistance.x, -effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, effectDistance.y);

            start = end;
            end = verts.Count;
            ApplyShadowZeroAlloc(verts, effectColor, start, verts.Count, -effectDistance.x, -effectDistance.y);

            vh.Clear();
            vh.AddUIVertexTriangleStream(verts);
            ListPool<UIVertex>.Release(verts);
        }
    }

    public interface IEffect
    {
        void UpdateEffect(Draw draw, float deltaTime);

        void Release();
    }
}