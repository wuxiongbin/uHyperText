﻿using UnityEngine;
using System;

namespace WXB
{
    public enum DrawType
    {
        Default, // 默认
        Alpha, // 透明
        Offset, // 位置偏移

        Outline, // 画线
        OffsetAndAlpha, // 透明+位置
        Cartoon, // 动画
    }

    public interface Draw
    {
        DrawType type { get; } // 类型

        long key { get; set; } // 名称

        Material srcMat { get; set; } // 源材质

        Texture texture { get; set; } // 源贴图

        CanvasRenderer canvasRenderer { get; }

        RectTransform rectTransform { get; }

        void UpdateSelf(float deltaTime);

        void FillMesh(Mesh workerMesh);

        void UpdateMaterial(Material mat);

        void Release();

        void DestroySelf();

        void OnInit();
    }
}