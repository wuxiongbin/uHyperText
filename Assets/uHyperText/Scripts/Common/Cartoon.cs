using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WXB
{
    [System.Serializable]
    public class DSprite : ISprite
    {
        public DSprite(Sprite sprite)
        {
            this.sprite = sprite;
        }

        public Sprite sprite;

        public int width { get { return (int)sprite.rect.height; } }
        public int height { get { return (int)sprite.rect.width; } }

        public void AddRef() { }

        public void SubRef() { }

        // 请求资源
        public Sprite Get() { return sprite; }
    }

    [System.Serializable]
    public class Cartoon
    {
        [System.Serializable]
        public class Frame
        {
            public DSprite sprite;
            public float delay;
        }

        public string name; // 动画名
        public Frame[] frames; // 精灵序列桢
        public float space = 2f; // 与其他元素之间的间隔

        public int width;
        public int height;
    }
}