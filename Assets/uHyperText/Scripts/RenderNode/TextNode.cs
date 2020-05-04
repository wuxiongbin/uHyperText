﻿using System;
using System.Text;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public partial class TextNode : NodeBase
	{
        public bool isFontSame(TextNode n)
        {
            if (d_font == n.d_font && d_fontSize == n.d_fontSize && d_fontStyle == n.d_fontStyle)
                return true;

            return false;
        }

        public override void Reset(Owner o, Anchor hf)
        {
            base.Reset(o, hf);

            d_font = null;
            d_bUnderline = false;
            d_bStrickout = false;
        }

		protected virtual bool isUnderLine()
		{
			return d_bUnderline;
		}

        public virtual Color currentColor
        {
            get { return d_color; }
        }

        public override float getHeight()
        {
            return size.y;
        }

        public override float getWidth()
        {
            return size.x;
        }

        Vector2 size = Vector2.zero;

        List<Element> d_widthList = new List<Element>();

        protected override void UpdateWidthList(out List<Element> widths, float pixelsPerUnit)
        {
            widths = d_widthList;
            d_widthList.Clear();
            if (d_text.Length == 0)
                return;

            float unitsPerPixel = 1f / pixelsPerUnit;
            int fontsize = (int)(d_fontSize * pixelsPerUnit);
            size.x = 0;
            size.y = FontCache.GetLineHeight(d_font, fontsize, d_fontStyle) * unitsPerPixel;

            Func<char, float> fontwidth = (char code) => { return FontCache.GetAdvance(d_font, fontsize, d_fontStyle, code) * unitsPerPixel; };
            ElementSegment es = owner.elementSegment;
            if (es == null)
            {
                for (int i = 0; i < d_text.Length; ++i)
                {
                    var e = new Element(fontwidth(d_text[i]));
#if UNITY_EDITOR
                    e.text = "" + d_text[i];
#endif
                    widths.Add(e);
                }
            }
            else
            {
                es.Segment(d_text, widths, fontwidth);
            }

            for (int i = 0; i < d_widthList.Count; ++i)
                size.x += d_widthList[i].totalwidth;

            //size.x *= pixelsPerUnit;
            //size.y *= pixelsPerUnit;
        }

        public virtual bool IsHyText()
        {
            return false;
        }

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
		{
            if (d_font == null)
                return;

            using (PD<StringBuilder> psb = Pool.GetSB())
            {
                Helper helper = new Helper(maxWidth, cache, x, yline, lines, formatting, offsetX, offsetY, psb.value);
                helper.Draw(this, NextLineX);

                x = helper.x;
                yline = helper.yline;
            }
        }

		public string d_text;
        public Font d_font;
        public int d_fontSize;
        public FontStyle d_fontStyle;
        public bool d_bUnderline;
        public bool d_bStrickout;
        public bool d_bDynUnderline;
        public bool d_bDynStrickout;
        public int d_dynSpeed;

        public EffectType effectType;
        public Color effectColor;
        public Vector2 effectDistance;

        public new long keyPrefix
        {
            get
            {
                long key = base.keyPrefix;
                if (d_bDynStrickout)
                    key += 1 << 45;

                if (d_bDynUnderline)
                    key += 1 << 45;

                return key;
            }
        }

        public override void SetConfig(TextParser.Config c)
        {
            base.SetConfig(c);

            d_font = c.font;
            d_bUnderline = c.isUnderline;
            d_fontSize = c.fontSize;
            d_fontStyle = c.fontStyle;
            d_bStrickout = c.isStrickout;
            d_bDynUnderline = c.isDyncUnderline;
            d_bDynStrickout = c.isDyncStrickout;
            d_dynSpeed = c.dyncSpeed;

            effectType = c.effectType;
            effectColor = c.effectColor;
            effectDistance = c.effectDistance;
        }

        public void GetLineCharacterInfo(out CharacterInfo info)
        {
            if (!d_font.GetCharacterInfo('_', out info, 20, FontStyle.Bold))
            {
                d_font.RequestCharactersInTexture("_", 20, FontStyle.Bold);
                d_font.GetCharacterInfo('_', out info, 20, FontStyle.Bold);
            }
        }

        protected override void ReleaseSelf()
        {
            d_text = null;
            d_font = null;
            d_fontSize = 0;
            d_bUnderline = false;
            d_bDynUnderline = false;
            d_bDynStrickout = false;
            d_dynSpeed = 0;
        }
	};
}