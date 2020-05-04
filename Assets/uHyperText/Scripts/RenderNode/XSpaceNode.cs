﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
	public class XSpaceNode : NodeBase
	{
        public override float getHeight()
		{
			return 0.01f;
		}

        public override float getWidth()
		{
			return d_offset;
		}

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
		{
            Vector2 pt = new Vector2(x + offsetX, offsetY);
			for (int i = 0; i < yline; ++i)
				pt.y += lines[i].y;

			// 因对齐，X轴偏移量
			float alignedX = AlignedFormatting(owner, formatting, maxWidth, lines[(int)(yline)].x, 0);

			if (x + d_offset + alignedX > maxWidth)
			{
				yline++;
				x = NextLineX;
			}
			else
			{
				x += d_offset;
			}

// 			d_beginLine = yline;
// 			d_endLine   = yline;

			if (d_bNewLine == true)
			{
				yline++;
				x = NextLineX;
			}
		}

		public float d_offset;

        protected override void ReleaseSelf()
        {
            d_offset = 0f;
        }
	};
}
