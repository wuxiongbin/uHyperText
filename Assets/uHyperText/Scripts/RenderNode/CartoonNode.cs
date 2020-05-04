﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public class CartoonNode : RectNode
    {
        public Cartoon cartoon;

        public override float getWidth()
        {
            return (int)((width + cartoon.space));
        }

        protected override void OnRectRender(RenderCache cache, Line line, Rect rect)
        {
            float space = cartoon.space;
            rect.x += space / 2f;
            rect.width -= space;
            cache.cacheCartoon(line, this, cartoon, rect);
        }

        protected override void ReleaseSelf()
        {
            cartoon = null;
        }
    }
}
