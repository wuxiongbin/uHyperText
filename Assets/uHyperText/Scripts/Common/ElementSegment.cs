using System;
using System.Text;
using System.Collections.Generic;

namespace WXB
{
    // 一句语句，可能会分多行显示，此接口为分割字符串的接口，把希望在同一行显示的部分组合成一个Element
    public interface ElementSegment
    {
        void Segment(string text, List<NodeBase.Element> widths, Func<char, float> fontwidth);
    }

    // 默认的单词分割算法
    internal class DefaultES : ElementSegment
    {
        enum CharType
        {
            Null,
            English, // 英文单词
            Chinese, // 中文符号
            Punctuation, // 标点符号
        }

        static CharType GetCharType(char c)
        {
            switch (c)
            {
            case ',':
            case ' ':
            case '.':
            case ':':
            case ';':
            case '?':
            case '；':
            case '。':
            case '"':
            case '“':
            case '”':
            case '，':
                return CharType.Punctuation;
            }

            if (c > 128)
            {
                return CharType.Chinese; // 中文都算是隔断符号
            }

            return CharType.English;
        }

        static NodeBase.Element Create(StringBuilder sb, Func<char, float> fontwidth)
        {
            if (sb.Length == 1)
            {
                var e = new NodeBase.Element(fontwidth(sb[0]));
#if UNITY_EDITOR
                e.text = sb.ToString();
#endif
                sb.Length = 0;
                return e;
            }
            else
            {
                List<float> widths = new List<float>();
                for (int i = 0; i < sb.Length; ++i)
                    widths.Add(fontwidth(sb[i]));

                var e = new NodeBase.Element(widths);
#if UNITY_EDITOR
                e.text = sb.ToString();
#endif
                sb.Length = 0;
                return e;
            }
        }

        public void Segment(string text, List<NodeBase.Element> widths, Func<char, float> fontwidth)
        {
            // 判断标准，如果全英文的，那么都尽量在同一行显示
            using (PD<StringBuilder> psb = Pool.GetSB())
            {
                StringBuilder sb = psb.value;
                char current;
                CharType lasttype = CharType.Null;
                for (int i = 0; i < text.Length; ++i)
                {
                    current = text[i];
                    CharType ct = GetCharType(current);
                    switch (ct)
                    {
                    case CharType.English:
                        {
                            switch (lasttype)
                            {
                            case CharType.Chinese: // 中文接英文，那么先把中文的保存起来
                                widths.Add(Create(sb, fontwidth));
                                break;

                            case CharType.Punctuation: // 英文接标点符号的
                                widths.Add(Create(sb, fontwidth));
                                break;

                            case CharType.English:
                                break;
                            }

                            sb.Append(current);
                        }
                        break;
                    case CharType.Chinese:
                        {
                            switch (lasttype)
                            {
                            case CharType.Chinese: // 中文接英文，那么先把中文的保存起来
                                widths.Add(Create(sb, fontwidth));
                                break;

                            case CharType.Punctuation:
                            case CharType.English:
                                break;
                            }

                            sb.Append(current);
                        }
                        break;
                    case CharType.Punctuation: // 标点符号
                        {
                            sb.Append(current);

                            widths.Add(Create(sb, fontwidth));
                        }
                        break;
                    }

                    lasttype = ct;
                }

                if (sb.Length != 0)
                    widths.Add(Create(sb, fontwidth));
            }
        }
    }

    public class ESFactory
    {
        // 类型列表
        static Dictionary<string, ElementSegment> TypeList = new Dictionary<string, ElementSegment>();

        static ESFactory()
        {
            TypeList.Add("Default", new DefaultES());
        }

        static public void Add(string name, ElementSegment es)
        {
            TypeList.Add(name, es);
        }

        static public bool Remove(string name)
        {
            return TypeList.Remove(name);
        }

        public static ElementSegment Get(string name)
        {
            ElementSegment es = null;
            if (TypeList.TryGetValue(name, out es))
                return es;

            return null;
        }

        public static List<string> GetAllName()
        {
            List<string> names = new List<string>();
            foreach (var itor in TypeList)
                names.Add(itor.Key);

            return names;
        }
    }
}
