﻿using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public class RectSpriteNode : NodeBase
    {
        public Rect rect;

        ISprite sprite;

        public void SetSprite(ISprite sprite)
        {
#if UNITY_EDITOR
            if (this.sprite != null)
            {
                Debug.LogError("this.sprite != null");
                this.sprite.SubRef();
            }
#endif
            this.sprite = sprite;
            this.sprite.AddRef();
        }

        public Color color;

        public override float getHeight()
		{
            return rect.height;
		}

        public override float getWidth()
		{
            return rect.width;
        }

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
        {
            cache.cacheSprite(null, this, sprite, rect);
        }

        public override void fill(ref Vector2 currentpos, List<Line> Lines, float maxWidth, float pixelsPerUnit)
        {

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
