﻿using UnityEngine;

namespace WXB
{
    public class AlphaEffect : IEffect
    {
        protected float last_update_time = -1f;
        protected bool isFoward = false; // 是否递增
        protected float space_timer = 0.05f;
        protected float alpha = 1f;

        public void UpdateEffect(Draw draw, float deltaTime)
        {
            if (last_update_time == -1f)
            {
                last_update_time = Time.realtimeSinceStartup;
                return;
            }

            float realtimeSinceStartup = Time.realtimeSinceStartup;
            float delta = realtimeSinceStartup - last_update_time;
            if (delta <= space_timer)
                return;

            space_timer = 0.05f;
            delta = space_timer;

            last_update_time = realtimeSinceStartup;

            if (isFoward)
            {
                alpha += delta;
                if (alpha > 1f)
                {
                    alpha = 1f;
                    isFoward = false;

                    space_timer = 0.5f;
                }
            }
            else
            {
                alpha -= delta;
                if (alpha < 0f)
                {
                    alpha = -alpha;
                    isFoward = true;
                }
            }

            draw.canvasRenderer.SetAlpha(alpha);
        }

        public void Release()
        {
            last_update_time = -1f;
            isFoward = false; // 是否递增
            space_timer = 0.05f;
            alpha = 1f;
        }
    }
}