﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public enum TypePosition
    {
        Absolute, // 绝对坐标
        Relative, // 相对坐标
        Auto, // 正常位置，没有任何的变化
    }

    // 行对齐方式
    public enum LineAlignment
    {
        Top, // 顶对齐
        Center, // 中心对齐
        Bottom, // 底对齐

        Default, // 默认,为顶对齐
    }
}
