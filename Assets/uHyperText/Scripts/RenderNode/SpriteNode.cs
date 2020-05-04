﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public class SpriteNode : RectNode
    {
        ISprite sprite;

        public void SetSprite(ISprite sprite)
        {
#if UNITY_EDITOR
            if(this.sprite != null)
            {
                Debug.LogError("this.sprite != null");
                this.sprite.SubRef();
            }
#endif
            this.sprite = sprite;
            this.sprite.AddRef();
        }

        protected override void OnRectRender(RenderCache cache, Line line, Rect rect)
        {
            cache.cacheISprite(line, this, sprite, rect);
        }

        protected override void ReleaseSelf()
        {
            if (sprite != null)
            {
                sprite.SubRef();
                sprite = null;
            }
        }
    }
}
