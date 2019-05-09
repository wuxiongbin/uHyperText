using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    public enum EffectType
    {
        Null, // 无类型

        // 特效类型
        Shadow, // 阴影
        Outline, // 描边
    }

    public abstract class NodeBase
    {
        public Owner owner;

        float d_nextLineX = 0;
        protected float NextLineX = 0;

        public virtual void Reset(Owner o, Anchor hf)
        {
            d_bNewLine = false;

            owner = o;

            formatting = hf;
            d_nextLineX = 0;
        }

        protected static float AlignedFormatting(Owner owner, Anchor formatting, float maxWidth, float curWidth, float lineX)
        {
            if (formatting == Anchor.Null)
                formatting = owner.anchor;

            float value = 0.0f;
            switch (formatting)
            {
            case Anchor.UpperRight:
            case Anchor.MiddleRight:
            case Anchor.LowerRight:
                value = (maxWidth - curWidth);
                break;

            case Anchor.UpperLeft:
            case Anchor.MiddleLeft:
            case Anchor.LowerLeft:
                value = lineX;
                break;

            case Anchor.MiddleCenter:
            case Anchor.UpperCenter:
            case Anchor.LowerCenter:
                value = (maxWidth - curWidth) / 2;
                break;
            }

            return value;
        }

        public abstract float getHeight();

        public abstract float getWidth();

        public void setNewLine(bool line)
        {
            d_bNewLine = line;
        }

        public bool isNewLine()
        {
            return d_bNewLine;
        }

        public Anchor formatting = Anchor.Null;

        public abstract void render(
            float maxWidth,
            RenderCache cache,
            ref float x,
            ref uint yline,
            List<Line> lines,
            float offsetX,
            float offsetY);

        protected virtual void AlterX(ref float x, float maxWidth)
        {

        }

        // lineX下一行的偏移量
        public virtual void fill(ref Vector2 currentpos, List<Line> lines, float maxWidth, float pixelsPerUnit)
        {
            NextLineX = d_nextLineX * pixelsPerUnit;
            List<Element> TempList;
            UpdateWidthList(out TempList, pixelsPerUnit);
            float height = getHeight();
            AlterX(ref currentpos.x, maxWidth);
            if (TempList.Count == 0)
                return;

            Around around = owner.around;
            bool isContain = false; // 当前行是否包含此元素
            for (int i = 0; i < TempList.Count;)
            {
                float totalwidth = TempList[i].totalwidth;
                float newx = 0f;
                if (((currentpos.x + totalwidth) > maxWidth))
                {
                    currentpos = TempList[i].Next(this, currentpos, lines, maxWidth, NextLineX, height, around, totalwidth, ref isContain);
                    ++i;
                }
                else if (around != null && !around.isContain(currentpos.x, currentpos.y, totalwidth, height, out newx))
                {
                    // 放置不下了
                    currentpos.x = newx;
                }
                else
                {
                    currentpos.x += totalwidth;
                    isContain = true;
                    ++i;
                }
            }

            Line bl = lines.back();
            bl.x = currentpos.x;
            bl.y = Mathf.Max(height, bl.y);

            if (d_bNewLine)
            {
                lines.Add(new Line(Vector2.zero));
                currentpos.y += height;
                currentpos.x = NextLineX;
            }
        }

        // 一个元素，此元素尽量要在同一行显示，如果当前是在行首，一行还放不下，那只能分行处理了
        public struct Element
        {
            public Element(List<float> ws)
            {
#if UNITY_EDITOR
                text = string.Empty;
#endif
                widthList = ws;
                totalWidth = 0f;
                for (int i = 0; i < widthList.Count; ++i)
                {
                    totalWidth += ws[i];
                }
            }

            public Element(float width)
            {
#if UNITY_EDITOR
                text = string.Empty;
#endif
                totalWidth = width;

                widthList = null;
            }

            List<float> widthList;

            float totalWidth;

            public float totalwidth
            {
                get
                {
                    return totalWidth;
                }
            }

            public List<float> widths
            {
                get { return widthList; }
            }

            public int count
            {
                get { return widthList == null ? 1 : widthList.Count; }
            }

#if UNITY_EDITOR
            public string text;

            public override string ToString()
            {
                return string.Format("text:{0} w:{1}", text, totalwidth);
            }
#endif
            public Vector2 Next(NodeBase n, Vector2 currentPos, List<Line> lines, float maxWidth, float lineX, float height, Around round, float tw, ref bool currentLineContain)
            {
                if (currentPos.x != 0f)
                {
                    Line bl = lines.back();
                    bl.x = currentPos.x;
                    if (currentLineContain)
                    {
                        bl.y = Mathf.Max(bl.y, height);
                    }

                    // 当前行有数据，在新行里处理
                    currentPos.x = lineX;
                    currentPos.y += bl.y;
                    currentLineContain = false;
                    lines.Add(new Line(new Vector2(lineX, 0)));
                }
                else
                {
                    // 当前行没有数据，直接在此行处理
                }

                if (round != null)
                {
                    float newx = 0f;
                    while (!round.isContain(currentPos.x, currentPos.y, tw, height, out newx))
                    {
                        currentPos.x = newx;
                        if (currentPos.x + tw > maxWidth)
                        {
                            currentPos.x = lineX;
                            lines.Add(new Line(new Vector2(lineX, height)));
                            currentPos.y += height;
                        }
                    }
                }

                if (widthList != null)
                {
                    for (int i = 0; i < widthList.Count; ++i)
                    {
                        currentPos = Add(n, currentPos, widthList[i], maxWidth, lineX, lines, height, ref currentLineContain);
                    }
                }
                else
                {
                    currentPos = Add(n, currentPos, totalWidth, maxWidth, lineX, lines, height, ref currentLineContain);
                }

                lines.back().x = currentPos.x;

                return currentPos;
            }

            Vector2 Add(NodeBase n, Vector2 currentPos, float width, float maxWidth, float lineX, List<Line> lines, float height, ref bool currentLineContain)
            {
                if (currentPos.x + width > maxWidth)
                {
                    // 需要换新行了
                    Line bl = lines.back();
                    bl.x = currentPos.x;
                    if (currentLineContain)
                        bl.y = Mathf.Max(bl.y, height);

                    currentPos.x = lineX + width;
                    lines.Add(new Line(new Vector2(currentPos.x, height)));
                    currentPos.y += height;
                }
                else
                {
                    currentPos.x += width;
                }

                currentLineContain = true;
                return currentPos;
            }
        }

        protected static List<Element> TempElementList = new List<Element>();

        protected virtual void UpdateWidthList(out List<Element> widths, float pixelsPerUnit)
        {
            TempElementList.Clear();
            TempElementList.Add(new Element(getWidth()));

            widths = TempElementList;
        }

        public virtual void onMouseEnter()
        {

        }

        public virtual void onMouseLeave()
        {

        }

        protected bool d_bNewLine;
        public bool d_bBlink; // 是否闪烁
        public bool d_bOffset; // 偏移效果
        public Rect d_rectOffset; // 偏移范围
        public Color d_color;

        public LineAlignment lineAlignment = LineAlignment.Default;

        // 用户数据
        public object userdata { get; set; }

        public long keyPrefix
        {
            get
            {
                long key = 0;
                if (d_bBlink)
                    key = 1 << 63;

                if (d_bOffset)
                {
                    key += 1 << 62;
                    key += ((byte)(d_rectOffset.xMin)) << 58;
                    key += ((byte)(d_rectOffset.xMax)) << 54;
                    key += ((byte)(d_rectOffset.yMin)) << 50;
                    key += ((byte)(d_rectOffset.yMax)) << 46;
                }

                return key;
            }
        }

        public virtual void SetConfig(TextParser.Config c)
        {
            d_nextLineX = c.nextLineX;
            d_bBlink = c.isBlink;
            lineAlignment = c.lineAlignment;

            d_color = c.fontColor;
            d_bOffset = c.isOffset;
            if (c.isOffset)
                d_rectOffset = c.offsetRect;
            else
                d_rectOffset.Set(0, 0, 0, 0);
        }

        public virtual void Release()
        {
            d_color = Color.white;
            d_bNewLine = false;
            owner = null;
            formatting = Anchor.Null;
            d_bBlink = false;
            d_bOffset = false;
            d_rectOffset.Set(0, 0, 0, 0);
            userdata = null;
        }
    };
}