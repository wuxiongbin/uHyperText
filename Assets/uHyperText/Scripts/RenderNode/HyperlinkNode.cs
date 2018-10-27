﻿using UnityEngine;

namespace WXB
{
    public class HyperlinkNode : TextNode
    {
        bool isEnter = false;

        public Color hoveColor = Color.red; // 超链接时的悬浮颜色

        public string d_link; // 链接文本

        public override void onMouseEnter()
        {
            isEnter = true;
            owner.SetRenderDirty();
        }

        public override Color currentColor
        {
            get { return isEnter ? hoveColor : d_color; }
        }

        public override void onMouseLeave()
        {
            isEnter = false;
            owner.SetRenderDirty();
        }

        public override bool IsHyText()
        {
            return true;
        }

        public override void Release()
        {
            base.Release();
            isEnter = false;
        }
    };
}
