﻿using UnityEngine;
using System.Collections.Generic;

namespace WXB
{
    public class SymbolTextInit : MonoBehaviour
    {
        static Dictionary<string, Font> Fonts; // 当前所有的字库
        static Dictionary<string, ISprite> Sprites; // 当前所有的精灵
        static Dictionary<string, Cartoon> Cartoons; // 当前所有的动画

        [SerializeField]
        Font[] fonts = null;

        [SerializeField]
        Sprite[] sprites = null;

        [SerializeField]
        Cartoon[] cartoons = null; // 所有的动画

        void init()
        {
            if (Fonts == null)
                Fonts = new Dictionary<string, Font>();
            else
                Fonts.Clear();

            if (fonts != null)
            {
                for (int i = 0; i < fonts.Length; ++i)
                    Fonts.Add(fonts[i].name, fonts[i]);
            }

            if (Sprites == null)
                Sprites = new Dictionary<string, ISprite>();
            else
                Sprites.Clear();

            if (sprites != null)
            {
                for (int i = 0; i < sprites.Length; ++i)
                    Sprites.Add(sprites[i].name, new DSprite(sprites[i]));
            }

            if (Cartoons == null)
                Cartoons = new Dictionary<string, Cartoon>();
            else
                Cartoons.Clear();

            if (cartoons != null)
            {
                for (int i = 0; i < cartoons.Length; ++i)
                    Cartoons.Add(cartoons[i].name, cartoons[i]);
            }
        }

        static void Init()
        {
            Resources.Load<SymbolTextInit>("SymbolTextInit").init();
        }

        public static Font GetFont(string name)
        {
            if (Fonts == null)
                Init();

            Font font;
            if (Fonts.TryGetValue(name, out font))
                return font;

            return null;
        }

        public static ISprite GetSprite(string name)
        {
            if (Sprites == null)
                Init();

            ISprite sprite;
            if (Sprites.TryGetValue(name, out sprite))
                return sprite;

            return null;
        }

        public static Cartoon GetCartoon(string name)
        {
            if (Cartoons == null)
                Init();

            Cartoon cartoon;
            if (Cartoons.TryGetValue(name, out cartoon))
                return cartoon;

            return null;
        }

        public static void GetCartoons(List<Cartoon> cartoons)
        {
            if (Cartoons == null)
                Init();
            
            foreach (var itor in Cartoons)
            {
                cartoons.Add(itor.Value);
            }
        }
    }
}