﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
	public class SetPosNode : NodeBase
	{
        public TypePosition type = TypePosition.Relative;
        public float d_value = 0f;

        public override float getHeight()
		{
			return 0.0f;
		}

		public override float getWidth()
		{
            return 0;
		}

        protected override void AlterX(ref float x, float maxWidth)
        {
            switch (type)
            {
            case TypePosition.Absolute:
                x = d_value;
                break;
            case TypePosition.Relative:
                x = maxWidth * d_value;
                break;
            }
        }

        public override void render(float maxWidth, RenderCache cache, ref float x, ref uint yline, List<Line> lines, float offsetX, float offsetY)
		{
            AlterX(ref x, maxWidth);
        }

        public override void Release()
        {
            base.Release();

            d_value = 0f;
        }
	};
}
