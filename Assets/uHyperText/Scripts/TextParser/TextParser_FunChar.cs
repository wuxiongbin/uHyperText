using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 文本解析
namespace WXB
{
    public partial class TextParser
    {
        delegate void OnFun(string c);
        OnFun[] OnFuns = null;

        void Reg()
        {
            OnFuns = new OnFun[128];

            // 特定的颜色
            OnFuns['R'] = ParserSureColor;
            OnFuns['G'] = ParserSureColor;
            OnFuns['B'] = ParserSureColor;
            OnFuns['K'] = ParserSureColor;
            OnFuns['Y'] = ParserSureColor;
            OnFuns['W'] = ParserSureColor;

            // 输出一个#
            OnFuns['#'] = ParserOutputChar;

            OnFuns['['] = ParserFontColorS; // 自定义的颜色名
            OnFuns['b'] = ParserBlink; // 闪烁
            OnFuns['c'] = ParserFontColor; // 颜色
            OnFuns['d'] = ParserFontStyle; // 字体风格
            OnFuns['e'] = ParserStrickout; // 删除线
            OnFuns['m'] = ParserDynStrickout; // 动态删除线

            OnFuns['f'] = ParserFont; // 换字体
            OnFuns['n'] = ParserRestoreColor; // 恢复初始颜色
            OnFuns['g'] = ParserRestore; // 所有设置恢复默认
            OnFuns['r'] = ParserNewLine; // 换行
            OnFuns['u'] = ParserUnderLine; // 下划线
            OnFuns['t'] = ParserDynUnderline; // 动态下划线
            OnFuns['l'] = ParserDynSpeed; // 动态线速度

            OnFuns['h'] = ParserHyperlink; // 超链接

            OnFuns['s'] = ParserFontSize; // 字体大小
            OnFuns['x'] = ParserXYZ; // xyz的偏移
            OnFuns['y'] = ParserXYZ;
            OnFuns['z'] = ParserXYZ;

            OnFuns['w'] = ParserFormatting; // 对齐格式

            OnFuns['a'] = ParserLineAlignment; // 行对齐

            OnFuns['&'] = ParserNextLineX;
        }

        void ParserNextLineX(string text)
        {
            int linex = -1;
            if (!ParserInt(ref d_curPos, text, ref linex, 5))
                return;

            if (linex == currentConfig.nextLineX)
                return;

            save(false);
            currentConfig.nextLineX = linex;
        }

        public static int max_cartoon_length = 5;

        // #后面接5个字符，为动画的名称
        bool ParserCartoon(string text)
        {
            Cartoon cartoon = null; 
            int currentPos = d_curPos;
            for (int i = max_cartoon_length; i >= 1; --i)
            {
                if (currentPos + i > text.Length)
                    continue;

                string name = text.Substring(currentPos, i);
                cartoon = Tools.GetCartoon(name);
                if (cartoon != null)
                {
                    currentPos += i;
                    break;
                }
            }

            if (cartoon == null)
                return false;

            d_curPos = currentPos;
            save(false);

            CartoonNode cn = CreateNode<CartoonNode>();
            cn.cartoon = cartoon;
            cn.width = cartoon.width * 0.6f;
            cn.height = cartoon.height * 0.6f;
            cn.SetConfig(currentConfig);
            // 表情不变色
            cn.d_color = Color.white;
            d_nodeList.Add(cn);

            return true;
        }

        void ParserDynSpeed(string text)
        {
            int size = -1;
            if (!ParserInt(ref d_curPos, text, ref size, 5))
                return;

            if (size <= 0 || size == currentConfig.dyncSpeed)
                return;

            save(false);
            currentConfig.dyncSpeed = (int)size;
        }

        // 动态下划线
        void ParserDynUnderline(string text)
        {
            save(false); // 保存内容
            currentConfig.isDyncUnderline = !currentConfig.isDyncUnderline; // 下划线
            d_curPos++;
        }

        // 动态删除线
        void ParserDynStrickout(string text)
        {
            save(false); // 保存内容
            currentConfig.isDyncStrickout = !currentConfig.isDyncStrickout; // 下划线
            d_curPos++;
        }

        void ParserRestoreColor(string text)
        {
            if (currentConfig.fontColor != startConfig.fontColor)
            {
                save(false); // 保存内容
                d_curPos++;
                currentConfig.Set(startConfig);
            }
            else
            {
                d_curPos++;
            }
        }

        void ParserRestore(string text)
        {
            if (!currentConfig.isSame(startConfig))
            {
                save(false); // 保存内容
                d_curPos++;
                currentConfig.Set(startConfig);
            }
            else
            {
                d_curPos++;
            }
        }

        void ParserSureColor(string text)
        {
            Color pCol = GetColour(text[d_curPos]);
            if (currentConfig.fontColor != pCol)
            {
                save(false);

                // 改变字体的颜色
                currentConfig.fontColor = pCol;
            }

            d_curPos++;
        }

        void ParserBlink(string text)
        {
            save(false); // 保存内容
            currentConfig.isBlink = !currentConfig.isBlink; // 下划线
            d_curPos++;
        }

        void ParserLineAlignment(string text)
        {
            if (text.Length > (d_curPos + 1))
            {
                LineAlignment a;
                if (Get(text[d_curPos + 1], out a))
                {
                    ++d_curPos;
                    if (currentConfig.lineAlignment != a)
                    {
                        currentConfig.lineAlignment = a;
                        save(false);
                    }
                }
            }

            ++d_curPos;
        }

        void ParserFormatting(string text)
        {
            if (text.Length > (d_curPos + 1))
            {
                Anchor a;
                if (Get(text[d_curPos + 1], out a))
                {
                    ++d_curPos;
                    if (currentConfig.anchor != a)
                    {
                        currentConfig.anchor = a;
                        save(false);
                    }
                }
            }

            ++d_curPos;
        }

        static bool GetFontStyle(char c, out FontStyle fs)
        {
            switch (c)
            {
            case '1': fs = FontStyle.Normal; return true;
            case '2': fs = FontStyle.Bold; return true;
            case '3': fs = FontStyle.Italic; return true;
            case '4': fs = FontStyle.BoldAndItalic; return true;
            }

            fs = FontStyle.Normal;
            return false;
        }

        void ParserFontStyle(string text)
        {
            if (text.Length > (d_curPos + 1))
            {
                FontStyle fs;
                if (GetFontStyle(text[d_curPos + 1], out fs))
                {
                    d_curPos += 2;
                    if (currentConfig.fontStyle != fs)
                    {
                        save(false); // 保存内容
                        currentConfig.fontStyle = fs;
                    }
                }
                else
                {
                    ++d_curPos;
                }
            }
            else
            {
                ++d_curPos;
            }
        }
        void ParserStrickout(string text)
        {
            save(false); // 保存内容
            currentConfig.isStrickout = !currentConfig.isStrickout; // 下划线
            d_curPos++;
        }

        void ParserFontColorS(string text)
        {
            --d_curPos;
            Color c = Tools.ParserColorName(text, ref d_curPos, currentConfig.fontColor);
            if (c != currentConfig.fontColor)
            {
                save(false);
                currentConfig.fontColor = c;
            }
        }

        void ParserFontColor(string text)
        {
            Color c = Tools.ParserColorName(text, ref d_curPos, currentConfig.fontColor);
            if (c != currentConfig.fontColor)
            {
                save(false);
                currentConfig.fontColor = c;
            }
        }

        void ParserUnderLine(string text)
        {
            save(false); // 保存内容
            currentConfig.isUnderline = !currentConfig.isUnderline; // 下划线
            d_curPos++;
        }

        void ParserNewLine(string text)
        {
            save(true);
            d_curPos++;
        }

        void ParserOutputChar(string text)
        {
            d_text.Append('#');
            d_curPos++;
        }

        void ParserHyperlink(string text)
        {
            // 结束的标识
            int endPos = text.IndexOf("#h", d_curPos + 1);
            if (endPos == -1)
            {
                // 查找失败，说明是错误的超链接
                d_curPos++;
            }
            else
            {
                // 保存之前的文本
                save(false);

                // 保存超链接
                d_text.Remove(0, d_text.Length);
                d_text.Append(text, d_curPos + 1, endPos - d_curPos - 1);
                saveHy();
                d_curPos = endPos + 2;
            }
        }

        void ParserFontSize(string text)
        {
            float size = 1f;
            if (!ParserFloat(ref d_curPos, text, ref size, 2))
                return;

            save(false);
            currentConfig.fontSize = (int)size;
        }

        void ParserXYZ(string text)
        {
            int pos = d_curPos;
            float offset = 0f;
            if (!ParserFloat(ref d_curPos, text, ref offset))
                return;

            if (offset == 0f)
                return;

            if (text[pos] == 'x')
            {
                save(false);
                saveX(offset);
            }
            else if (text[pos] == 'y')
            {
                if (d_text.Length != 0)
                    save(true);

                saveY(offset);
            }
            else if (text[pos] == 'z')
            {
                if (d_text.Length != 0)
                    save(false);

                saveZ(offset);
            }
        }

        void ParserFont(string text)
        {
            d_curPos++;
            Font font = Tools.ParserFontName(text, ref d_curPos);
            if (font != null)
            {
                if (currentConfig.font != font)
                {
                    save(false);
                    currentConfig.font = font;
                }
            }
            else
            {
                d_curPos--;
            }
        }
    }
}

