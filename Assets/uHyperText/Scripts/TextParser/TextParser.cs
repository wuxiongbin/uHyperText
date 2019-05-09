using System.Text;
using UnityEngine;
using System.Collections.Generic;

// 文本解析
namespace WXB
{
    public enum Anchor
    {
        UpperLeft = 0,
        UpperCenter = 1,
        UpperRight = 2,
        MiddleLeft = 3,
        MiddleCenter = 4,
        MiddleRight = 5,
        LowerLeft = 6,
        LowerCenter = 7,
        LowerRight = 8,
        Null,
    }

    public partial class TextParser
    {
        public T CreateNode<T>() where T : NodeBase, new()
        {
            T t = new T();
            t.Reset(mOwner, currentConfig.anchor);

            return t;
        }

        static bool Get(char c, out Anchor a)
        {
            switch (c)
            {
            case '1': a = Anchor.MiddleLeft; return true;
            case '2': a = Anchor.MiddleCenter; return true;
            case '3': a = Anchor.MiddleRight; return true;
            }

            a = Anchor.MiddleCenter;
            return false;
        }

        static bool Get(char c, out LineAlignment a)
        {
            switch (c)
            {
            case '1': a = LineAlignment.Top; return true;
            case '2': a = LineAlignment.Center; return true;
            case '3': a = LineAlignment.Bottom; return true;
            }

            a = LineAlignment.Default;
            return false;
        }

        public TextParser()
        {
            clear();

            Reg();
            RegTag();
        }

        Owner mOwner;

        static bool ParserInt(ref int d_curPos, string text, ref int value, int num = 3)
        {
            using (PD<StringBuilder> psb = Pool.GetSB())
            {
                StringBuilder sb = psb.value;
                d_curPos++;
                while (text.Length > d_curPos && ((text[d_curPos] >= '0' && text[d_curPos] <= '9')))
                {
                    sb.Append(text[d_curPos]);
                    d_curPos++;

                    if (sb.Length >= num)
                        break;
                }

                value = Tools.stringToInt(sb.ToString(), -1);
                if (sb.Length == 0)
                {
                    d_curPos--;
                    return false;
                }

                return true;
            }
        }

        static bool ParserFloat(ref int d_curPos, string text, ref float value, int num = 3)
        {
            using (PD<StringBuilder> psb = Pool.GetSB())
            {
                var sb = psb.value;
                d_curPos++;
                bool bInPoint = false;
                while (text.Length > d_curPos && ((text[d_curPos] >= '0' && text[d_curPos] <= '9') || (text[d_curPos] == '.')))
                {
                    if (text[d_curPos] == '.')
                        bInPoint = true;

                    sb.Append(text[d_curPos]);
                    d_curPos++;

                    int size = (bInPoint == true ? (num + 1) : num);
                    if (sb.Length >= size)
                        break;
                }

                value = Tools.stringToFloat(sb.ToString(), 0);
                if (sb.Length == 0)
                {
                    d_curPos--;
                    return false;
                }

                return true;
            }
        }
        public struct Config
        {
            public Anchor anchor;
            public Font font;
            public FontStyle fontStyle;
            public int fontSize;
            public Color fontColor;
            public bool isUnderline;
            public bool isStrickout;
            public bool isBlink;
            public bool isDyncUnderline; // 动态下划线
            public bool isDyncStrickout; // 动态删除线
            public int dyncSpeed;
            public bool isOffset;
            public Rect offsetRect;

            public EffectType effectType;
            public Color effectColor;
            public Vector2 effectDistance;

            public LineAlignment lineAlignment;
            public int nextLineX; // 下一行的起始偏移量

            public void Clear()
            {
                anchor = Anchor.Null;
                font = null;
                fontStyle = FontStyle.Normal;
                fontSize = 0;
                fontColor = Color.white;
                isUnderline = false;
                isStrickout = false;
                isBlink = false;
                isDyncUnderline = false;
                isDyncStrickout = false;
                dyncSpeed = 0;
                isOffset = false;
                offsetRect.Set(0, 0, 0, 0);

                effectType = EffectType.Null;
                effectColor = Color.black;
                effectDistance = Vector2.zero;

                lineAlignment = LineAlignment.Default;
                nextLineX = 0;
            }

            public void Set(Config c)
            {
                anchor = c.anchor;
                font = c.font;
                fontStyle = c.fontStyle;
                fontSize = c.fontSize;
                fontColor = c.fontColor;
                isUnderline = c.isUnderline;
                isStrickout = c.isStrickout;
                isBlink = c.isBlink;
                dyncSpeed = c.dyncSpeed;

                isOffset = c.isOffset;
                offsetRect = c.offsetRect;

                effectType = c.effectType;
                effectColor = c.effectColor;
                effectDistance = c.effectDistance;
                isDyncUnderline = c.isDyncUnderline;
                isDyncStrickout = c.isDyncStrickout;
                lineAlignment = c.lineAlignment;
                nextLineX = c.nextLineX;
            }

            public bool isSame(Config c)
            {
                return anchor == c.anchor &&
                       font == c.font &&
                       fontStyle == c.fontStyle &&
                       isUnderline == c.isUnderline &&
                       fontColor == c.fontColor &&
                       isStrickout == c.isStrickout &&
                       isBlink == c.isBlink &&
                       fontSize == c.fontSize &&
                       lineAlignment == c.lineAlignment &&
                       isDyncUnderline == c.isDyncUnderline &&
                       isDyncStrickout == c.isDyncStrickout &&
                       nextLineX == c.nextLineX &&
                       dyncSpeed == c.dyncSpeed &&
                       (
                       (effectType == EffectType.Null && c.effectType == EffectType.Null) ||
                       (effectType == c.effectType && effectColor == c.effectColor && effectDistance == c.effectDistance)
                       ) &&
                       (
                       (isOffset == false && c.isOffset == false) ||
                       (isOffset == c.isOffset && offsetRect == c.offsetRect)
                       );
            }
        }

        public void parser(Owner owner, string text, Config config, List<NodeBase> vList)
        {
            clear();

            mOwner = owner;
            d_nodeList = vList;
            startConfig.Set(config);
            currentConfig.Set(config);

            if (currentConfig.font == null)
            {
                Debug.LogError("TextParser pFont == null");
                return;
            }

            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            int lenght = text.Length;
            while (lenght > d_curPos)
            {
                if (d_bBegin == false)
                {
                    switch (text[d_curPos])
                    {
                    case '#':
                        {
                            // 未遇到功能字符，开始功能字符的解析
                            d_bBegin = true;
                            ++d_curPos;
                        }
                        break;
                    case '<':
                        {
                            int endpos = text.IndexOf('>', d_curPos);
                            if (endpos != -1)
                            {
                                string tag = null;
                                string param = null;

                                int tagend = text.IndexOfAny(new char[] { ' ', '=' }, d_curPos);
                                if (tagend != -1 && tagend < endpos)
                                {
                                    tag = text.Substring(d_curPos + 1, tagend - d_curPos);
                                    param = text.Substring(tagend + 1, endpos - tagend - 1);
                                }
                                else
                                {
                                    tag = text.Substring(d_curPos + 1, endpos - d_curPos - 1);
                                }

                                if (d_text.Length != 0)
                                    save(false);

                                TagParam(tag, param);

                                d_curPos = endpos + 1;
                                break;
                            }
                            else
                            {
                                d_text.Append(text[d_curPos]);
                            }

                            ++d_curPos;
                        }
                        break;
                    case '\n':
                        {
                            // 这个是换行
                            save(true);
                            d_curPos++;
                        }
                        break;
                    default:
                        {
                            d_text.Append(text[d_curPos]);
                            ++d_curPos;
                        }
                        break;
                    }
                }
                else
                {
                    char c = text[d_curPos];
                    OnFun fun = null;
                    if (c < 128 && ((fun = OnFuns[c]) != null))
                    {
                        fun(text);
                    }
                    else
                    {
                        d_text.Append(text[d_curPos]);
                        ++d_curPos;
                    }

                    d_bBegin = false;
                }
            }

            if (d_text.Length != 0)
                save(false);

            clear();
        }

        protected void save(bool isNewLine)
        {
            if (d_text.Length == 0)
            {
                if (isNewLine == true)
                {
                    if (d_nodeList.Count != 0)
                    {
                        NodeBase node = d_nodeList.back();
                        if (node.isNewLine() == false)
                        {
                            node.setNewLine(true);
                            return;
                        }
                    }

                    // 添加一个换行的结点
                    LineNode nodeY = CreateNode<LineNode>();
                    nodeY.SetConfig(currentConfig);
                    nodeY.font = currentConfig.font;
                    nodeY.fontSize = currentConfig.fontSize;
                    nodeY.fs = currentConfig.fontStyle;
                    nodeY.setNewLine(true);
                    d_nodeList.Add(nodeY);
                    return;
                }
                else
                {
                    return;
                }
            }

            // 为文本 
            TextNode textNode = CreateNode<TextNode>();
            {
                textNode.d_text = d_text.ToString();
                textNode.SetConfig(currentConfig);
            }
            textNode.setNewLine(isNewLine);

            d_nodeList.Add(textNode);
            d_text.Remove(0, d_text.Length);
        }

        protected void saveX(float value)
        {
            XSpaceNode node = CreateNode<XSpaceNode>();
            node.d_offset = value;

            d_nodeList.Add(node);
        }

        protected void saveY(float value)
        {
            if (d_nodeList.Count != 0 && d_nodeList.back().isNewLine() == false)
            {
                d_nodeList.back().setNewLine(true);
            }

            YSpaceNode node = CreateNode<YSpaceNode>();
            node.d_offset = value;
            node.setNewLine(true);
            d_nodeList.Add(node);
        }

        protected void saveZ(float value)
        {
            YSpaceNode node = CreateNode<YSpaceNode>();
            node.d_offset = value;
            node.setNewLine(false);
            d_nodeList.Add(node);
        }

        protected void saveHy()
        {
            if (d_text.Length == 0)
                return;

            string text = d_text.ToString();
            d_text.Remove(0, d_text.Length);
            HyperlinkNode node = CreateNode<HyperlinkNode>();
            string hytext = string.Empty;
            if (text[text.Length - 1] == '}')
            {
                int beginPos = text.IndexOf('{', 0);
                if (beginPos != -1)
                {
                    hytext = text.Substring(beginPos, text.Length - beginPos);
                    node.d_link = hytext.Replace("{", "").Replace("}", "");
                    text = text.Remove(beginPos, text.Length - beginPos);
                }
            }

            node.d_text = "";
            node.SetConfig(currentConfig);
            ParseHyText(text, node);

            d_nodeList.Add(node);
        }

        protected void clear()
        {
            startConfig.Clear();
            currentConfig.Clear();

            d_nodeList = null;
            d_curPos = 0;
            d_text.Remove(0, d_text.Length);
            d_bBegin = false;
            mOwner = null;
        }

        protected int d_curPos = 0;
        protected Config startConfig;
        protected Config currentConfig;
        protected List<NodeBase> d_nodeList;

        protected StringBuilder d_text = new StringBuilder();
        protected bool d_bBegin;

        static Color GetColour(uint code)
        {
            switch (code)
            {
            case 'R': return Color.red;
            case 'G': return Color.green;
            case 'B': return Color.blue;
            case 'K': return Color.black;
            case 'Y': return Color.yellow;
            case 'W': return Color.white;
            }

            return Color.white;
        }
    }
}
