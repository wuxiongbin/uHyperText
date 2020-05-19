using UnityEngine;
using System;

namespace WXB
{
    public interface Owner
    {
        // 最小行高
        int minLineHeight { get; set; }

        Around around { get; }

        RenderCache renderCache { get; }
        int wordSpacing { get; }
        Anchor anchor { get; }

        void SetRenderDirty();

        // 元素分割
        ElementSegment elementSegment { get; }

        // 通过纹理获取渲染对象,会考虑合并的情况
        Draw GetDraw(DrawType type, long key, Action<Draw, object> onCreate, object para = null);

        Material material { get; }

        LineAlignment lineAlignment { get; }
    }
}