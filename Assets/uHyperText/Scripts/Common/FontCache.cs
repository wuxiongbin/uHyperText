using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    public static class FontCache
    {
        static CharacterInfo s_Info;

        // 行高缓存
        static Dictionary<long, int> FontLineHeight = new Dictionary<long, int>();

        static TextGenerator sCachedTextGenerator = new TextGenerator();

        static public int GetLineHeight(Font font, int size, FontStyle fs)
        {
            long key = (long)(((ulong)fs) | (((ulong)size) << 24) | (((ulong)font.GetInstanceID()) << 32));
            int lineHeight = 0;
            if (FontLineHeight.TryGetValue(key, out lineHeight))
                return lineHeight;

            var settings = new TextGenerationSettings();
            settings.generationExtents = new Vector2(1000, 1000);
            if (font != null && font.dynamic)
            {
                settings.fontSize = size;
            }

            // Other settings
            settings.textAnchor = TextAnchor.LowerLeft;
            settings.lineSpacing = 1f;
            settings.alignByGeometry = false;
            settings.scaleFactor = 1f;
            settings.font = font;
            settings.fontSize = size;
            settings.fontStyle = fs;
            settings.resizeTextForBestFit = false;
            settings.updateBounds = false;
            settings.horizontalOverflow = HorizontalWrapMode.Overflow;
            settings.verticalOverflow = VerticalWrapMode.Truncate;

            string text = "a\na";
            sCachedTextGenerator.Populate(text, settings);

            IList<UIVertex> verts = sCachedTextGenerator.verts;
#if UNITY_2019
            lineHeight = (int)(verts[0].position.y - verts[4].position.y);
#else
            lineHeight = (int)(verts[0].position.y - verts[8].position.y);
#endif
            FontLineHeight.Add(key, lineHeight);
            return lineHeight;
        }

        static Dictionary<long, int> FontAdvances = new Dictionary<long, int>();

        static public int GetAdvance(Font font, int size, FontStyle fs, char ch)
        {
            long key = (long)(((ulong)((uint)ch)) | (((ulong)fs) << 16) | (((ulong)size) << 24) | (((ulong)font.GetInstanceID()) << 32));
            int advance = 0;
            if (FontAdvances.TryGetValue(key, out advance))
                return advance;

            for (int i = 0; i < 2; ++i)
            {
                if (font.GetCharacterInfo(ch, out s_Info, size, fs))
                {
                    advance = (short)(s_Info.advance);
                    FontAdvances.Add(key, advance);
                    return advance;
                }
                else
                {
                    font.RequestCharactersInTexture(new string(ch, 1), size, fs);
                }
            }

            FontAdvances.Add(key, 0);
            return 0;
        }
    }
}