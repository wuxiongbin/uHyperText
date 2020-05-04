﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    // 缓存渲染元素
    public partial class RenderCache
    {
        class SpriteData : BaseData
        {
            public ISprite sprite;

            public void Reset(NodeBase n, ISprite s, Rect r, Line l)
            {
                if (sprite != null)
                    sprite.SubRef();

                node = n;
                sprite = s;
                rect = r;
                line = l;
                if (sprite != null)
                    sprite.AddRef();
            }

            protected override void OnRelease()
            {
                if (sprite != null)
                {
                    sprite.SubRef();
                    sprite = null;
                }
            }

            public override void Render(VertexHelper vh, Rect area, Vector2 offset, float pixelsPerUnit)
            {
                Color currentColor = node.d_color;
                if (currentColor.a <= 0.01f)
                    return;

                var uv = UnityEngine.Sprites.DataUtility.GetOuterUV(sprite.Get());

                Vector2 leftPos = GetStartLeftBottom(1f) + offset;
                Tools.LB2LT(ref leftPos, area.height);

                float width = rect.width;
                float height = rect.height;

                int count = vh.currentVertCount;
                vh.AddVert(new Vector3(leftPos.x, leftPos.y), currentColor, new Vector2(uv.x, uv.y));
                vh.AddVert(new Vector3(leftPos.x, leftPos.y + height), currentColor, new Vector2(uv.x, uv.w));
                vh.AddVert(new Vector3(leftPos.x + width, leftPos.y + height), currentColor, new Vector2(uv.z, uv.w));
                vh.AddVert(new Vector3(leftPos.x + width, leftPos.y), currentColor, new Vector2(uv.z, uv.y));

                vh.AddTriangle(count, count + 1, count + 2);
                vh.AddTriangle(count + 2, count + 3, count);
            }
        }

        class ISpriteData : SpriteData
        {
            public override void Render(VertexHelper vh, Rect area, Vector2 offset, float pixelsPerUnit)
            {
                if (sprite == null)
                    return;

                Color currentColor = node.d_color;
                if (currentColor.a <= 0.01f)
                    return;

                Vector2 leftPos = GetStartLeftBottom(1f) + offset;
                Tools.LB2LT(ref leftPos, area.height);

                ISpriteDraw cd = node.owner.GetDraw(DrawType.ISprite, node.keyPrefix + sprite.GetHashCode(),
                    (Draw d, object p) =>
                    {
                        ISpriteDraw cad = d as ISpriteDraw;
                        cad.isOpenAlpha = node.d_bBlink;
                        cad.isOpenOffset = false;
                    }) as ISpriteDraw;

                cd.Add(sprite, leftPos, rect.width, rect.height, currentColor);
            }
        }
    }
}