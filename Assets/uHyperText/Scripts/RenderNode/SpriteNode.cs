﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public class SpriteNode : RectNode
    {
        public Sprite sprite;

        protected override void OnRectRender(RenderCache cache, Line line, Rect rect)
        {
            cache.cacheSprite(line, this, sprite, rect);
        }
    };
}
