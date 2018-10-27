﻿using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

namespace WXB
{
    public class DrawStruct
    {

    }

    public class DrawLineStruct : DrawStruct
    {
        public struct Line
        {
            public Vector2 leftPos;
            public float width;
            public float height;
            public Vector2 uv;
            public Color color;
            public int dynSpeed;
            public TextNode node;

            public void Render(VertexHelper vh, ref float curwidth)
            {
                int start = vh.currentIndexCount;

                Color c = color;
                if (c.a <= 0.01f)
                    return;

                if (curwidth >= width)
                {
                    // 完整显示
                    Tools.AddLine(vh, leftPos, uv, width, height, c);
                }
                else
                {
                    Tools.AddLine(vh, leftPos, uv, curwidth, height, c);
                }

                switch (node.effectType)
                {
                case EffectType.Outline:
                    c = node.effectColor;
                    Effect.Outline(vh, start, c, node.effectDistance);
                    break;
                case EffectType.Shadow:
                    c = node.effectColor;
                    Effect.Shadow(vh, start, c, node.effectDistance);
                    break;
                }

                curwidth -= width;
            }
        }

        public List<Line> lines = new List<Line>();

        public void Render(float width, VertexHelper vh)
        {
            for (int i = 0; i < lines.Count; ++i)
            {
                lines[i].Render(vh, ref width);
                if (width <= 0f)
                    break;
            }
        }
    }
}