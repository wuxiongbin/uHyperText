﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public abstract class RectNode : NodeBase
	{
        public float width = 0;
        public float height = 0;

        public override float getHeight()
        {
            return height;
        }

        public override float getWidth()
        {
            return width;
        }

        public override void Release()
        {
            base.Release();
            width = 0f;
            height = 0f;
        }

        protected abstract void OnRectRender(RenderCache cache, Line line, Rect rect);

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
        {
			float width  = getWidth();
			float height = getHeight();

            if (x + width > maxWidth)
            {
				x = NextLineX;
				yline++;
			}

			float alignedX = AlignedFormatting(owner, formatting, maxWidth, lines[(int)(yline)].x, 0);

			float y_offset = offsetY;
			for (int i = 0; i < yline; ++i)
				y_offset += lines[i].y;

			//y_offset += lines[(int)(yline)].y;
            Rect areaRect = new Rect(x + offsetX + alignedX, y_offset, width, height);

            float newfx = 0f;
            while (!owner.around.isContain(areaRect, out newfx))
            {
                areaRect.x = newfx;
                x = newfx - alignedX - offsetX;
                if (x + width > maxWidth)
                {
                    x = NextLineX;
                    yline++;
                    y_offset += lines[(int)yline].y;
                    areaRect = new Rect(x + offsetX + alignedX, y_offset, width, height);
                }
            }

            OnRectRender(cache, lines[(int)yline], areaRect);

            x += width;

			if (d_bNewLine)
			{
				x = 0;
				yline++;
			}
		}
	};
}
