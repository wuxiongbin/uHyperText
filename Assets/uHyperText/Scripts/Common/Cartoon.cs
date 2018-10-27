using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WXB
{
    [System.Serializable]
    public class Cartoon
    {
        public string name; // 动画名
        public float fps; // 播放速度
        public Sprite[] sprites; // 精灵序列桢
        public float space = 2f; // 与其他元素之间的间隔

        public int width;
        public int height;
    }
}