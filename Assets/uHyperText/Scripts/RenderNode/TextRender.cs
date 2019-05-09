using System.Text;
using UnityEngine;
using System.Collections.Generic;

namespace WXB
{
    public partial class TextNode : NodeBase
    {
        struct Helper
        {
            public RenderCache cache;
            public float x;
            public uint yline;
            public List<Line> lines;
            public Anchor xFormatting;
            public float offsetX;
            public float offsetY;
            public StringBuilder sb;
            
            public Helper(float maxWidth, RenderCache cache, float x, uint yline, List<Line> lines, Anchor xFormatting, float offsetX, float offsetY, StringBuilder sb)
            {
                this.maxWidth = maxWidth;
                this.cache = cache;
                this.x = x;
                this.yline = yline;
                this.lines = lines;
                this.xFormatting = xFormatting;
                this.offsetX = offsetX;
                this.offsetY = offsetY;

                pixelsPerUnit = 1f;
                alignedX = 0;

                pt = Vector2.zero;
                node = null;
                fHeight = 0f;

                this.sb = sb;
            }

            public float pixelsPerUnit;

            Vector2 pt;
            float alignedX;

            TextNode node;

            float maxWidth;

            float fHeight;

            void DrawCurrent(bool isnewLine, Around around, float lineX)
            {
                if (sb.Length != 0)
                {
                    Rect area_rect = new Rect(pt.x + alignedX, pt.y, x - pt.x + offsetX, node.getHeight());

                    cache.cacheText(lines[(int)yline], node, sb.ToString(), area_rect);

                    sb.Remove(0, sb.Length);
                }

                if (isnewLine)
                {
                    // 再换行
                    yline++;
                    x = lineX;

                    pt.x = offsetX;
                    pt.y = offsetY;
                    for (int n = 0; n < yline; ++n)
                        pt.y += lines[n].y;

                    if (yline >= lines.Count)
                    {
                        --yline;
                        //Debug.LogError("yline >= vLineSize.Count!yline:" + yline + " vLineSize:" + lines.Count);
                    }

                    alignedX = AlignedFormatting(node.owner, xFormatting, maxWidth, lines[(int)(yline)].x, lineX);

                    float newx; 
                    if (!around.isContain(pt.x + alignedX, pt.y, 1, node.getHeight(), out newx))
                    {
                        pt.x = newx - alignedX;
                        x = pt.x;
                    }
                }
            }

            public void Draw(TextNode n, float lineX)
            {
                node = n;
                pt = new Vector2(x + offsetX, offsetY);
                for (int i = 0; i < yline; ++i)
                    pt.y += lines[i].y;

                if (maxWidth == 0)
                    return;

                alignedX = AlignedFormatting(n.owner, xFormatting, maxWidth, lines[(int)(yline)].x, 0);
                fHeight = node.getHeight();

                sb.Remove(0, sb.Length);

                Around around = n.owner.around;

                int textindex = 0;
                float newx = 0f;
                for (int k = 0; k < node.d_widthList.Count; ++k)
                {
                    Element e = node.d_widthList[k];
                    float totalwidth = e.totalwidth;
                    if ((x + totalwidth) > maxWidth)
                    {
                        if (x != 0f)
                        {
                            DrawCurrent(true, around, lineX);
                        }

                        if (e.widths == null)
                        {
                            if ((x + e.totalwidth > maxWidth))
                            {
                                DrawCurrent(true, around, lineX);
                            }
                            else
                            {
                                x += e.totalwidth;
                                sb.Append(node.d_text[textindex++]);
                            }
                        }
                        else
                        {
                            for (int m = 0; m < e.widths.Count;)
                            {
                                if (x != 0 && x + e.widths[m] > maxWidth)
                                {
                                    DrawCurrent(true, around, lineX);
                                }
                                else
                                {
                                    x += e.widths[m];
                                    sb.Append(node.d_text[textindex++]);
                                    ++m;
                                }
                            }
                        }
                    }
                    else if (!around.isContain(x, pt.y, totalwidth, fHeight, out newx))
                    {
                        DrawCurrent(false, around, lineX);

                        x = newx;
                        pt.x = newx;
                        --k;
                    }
                    else
                    {
                        int ec = e.count;
                        sb.Append(node.d_text.Substring(textindex, ec));
                        textindex += ec;
                        x += totalwidth;
                    }
                }

                if (sb.Length != 0)
                {
                    DrawCurrent(false, around, lineX);
                }

                if (node.d_bNewLine == true)
                {
                    yline++;
                    x = lineX;
                }
            }
        }
    }
}
