using UnityEngine;
using System.Text;

// 文本解析
namespace WXB
{
    public partial class TextParser
    {
        class HyConfig
        {
            public string text = "";
            public HyperlinkNode node = null;
            public int startPos = 0;
            public int lenght = 0;

            StringBuilder sb;

            public TextParser parser;

            delegate void OnFunHy(string text);
            OnFunHy[] OnFunHys = null;

            public HyConfig(TextParser p)
            {
                parser = p;
                OnFunHys = new OnFunHy[128];

                // 特定的颜色
                OnFunHys['R'] = ParserSureColor;
                OnFunHys['G'] = ParserSureColor;
                OnFunHys['B'] = ParserSureColor;
                OnFunHys['K'] = ParserSureColor;
                OnFunHys['Y'] = ParserSureColor;
                OnFunHys['W'] = ParserSureColor;
                OnFunHys['P'] = ParserSureColor;

                OnFunHys['c'] = ParserFontColor;
                OnFunHys['n'] = ParserRestore;
                OnFunHys['s'] = ParserFontSize;
                OnFunHys['f'] = ParserFont;
                OnFunHys['#'] = ParserOutputChar;

                OnFunHys['u'] = (string text) => 
                {
                    node.d_bUnderline = !node.d_bUnderline; // 下划线
                    startPos++;
                };

                OnFunHys['e'] = (string text) =>
                {
                    node.d_bStrickout = !node.d_bStrickout; // 下划线
                    startPos++;
                };
            }

            void ParserOutputChar(string text)
            {
                sb.Append("#");
                ++startPos;
            }

            void ParserSureColor(string text)
            {
                node.d_color = GetColour(text[startPos]);
                ++startPos;
            }

            void ParserFontColor(string text)
            {
                node.d_color = Tools.ParserColorName(text, ref startPos, node.d_color);
            }

            void ParserRestore(string text)
            {
                node.SetConfig(parser.startConfig);
            }

            void ParserFontSize(string text)
            {
                float size = 1f;
                if (!ParserFloat(ref startPos, text, ref size))
                    return;

                node.d_fontSize = (int)size;
            }

            void ParserFont(string text)
            {
                ++startPos;

                // 字体
                Font pFont = Tools.ParserFontName(text, ref startPos);
                if (pFont != null)
                {
                    node.d_font = pFont;
                }
                else
                {
                    --startPos;
                }
            }

            public void Clear()
            {
                text = null;
                node = null;
                sb = null;
                startPos = 0;
            }

            public void BeginParser(StringBuilder s)
            {
                sb = s;
                bool bBegin = false;
                while (lenght > startPos)
                {
                    if (bBegin == false)
                    {
                        if (text[startPos] == '#')
                        {
                            // 未遇到功能字符，开始功能字符的解析
                            bBegin = true;
                            startPos++;
                        }
                        else
                        {
                            sb.Append(text[startPos]);
                            startPos++;
                        }
                    }
                    else
                    {
                        char c = text[startPos];
                        OnFunHy fun = null;
                        if (c < 128 && ((fun = OnFunHys[c]) != null))
                        {
                            fun(text);
                        }
                        else
                        {
                            sb.Append(text[startPos]);
                            startPos++;
                        }

                        bBegin = false;
                    }
                }

                node.d_text = sb.ToString();
                node.hoveColor = node.d_color;
            }
        }

        static HyConfig hyConfig = null;

        void ParseHyText(string text, HyperlinkNode data)
        {
            if (hyConfig == null)
                hyConfig = new HyConfig(this);

            // 初始化数据
            {
                hyConfig.text = text;
                hyConfig.node = data;
                hyConfig.startPos = 0;
                hyConfig.lenght = text.Length;
            }

            using (PD<StringBuilder> psb = Pool.GetSB())
            {
                hyConfig.BeginParser(psb.value);
                hyConfig.Clear();
            }
        }
    }
}

