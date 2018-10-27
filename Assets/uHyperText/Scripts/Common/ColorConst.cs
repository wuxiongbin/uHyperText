using System.Collections.Generic;
using UnityEngine;

namespace WXB
{
    public static class ColorConst
    {
        public static Color aqua = new Color(0f, 0xff / 255f, 0xff / 255f, 1f);
        public static Color brown = new Color(0xa5 / 255f, 0x2a / 255f, 0x2a / 255f, 1f);
        public static Color darkblue = new Color(0f, 0f, 0xa0 / 255f, 1f);
        public static Color fuchsia = new Color(1f, 0f, 1f, 1f);
        public static Color lightblue = new Color(0xad / 255f, 0xd8 / 255f, 0xe6 / 255f, 1f);
        public static Color lime = new Color(0f, 1f, 0f, 1f);
        public static Color maroon = new Color(0x80 / 255f, 0f, 0f, 1f);
        public static Color navy = new Color(0f, 0f, 0x80 / 255f, 1f);
        public static Color olive = new Color(0x80 / 255f, 0x80 / 255f, 0f, 1f);
        public static Color orange = new Color(1f, 0xa5 / 255f, 0f, 1f);
        public static Color purple = new Color(0x80/255f, 0f, 0x80 / 255f, 1f);
        public static Color silver = new Color(0xc0 / 255f, 0xc0 / 255f, 0xc0 / 255f, 1f);
        public static Color teal = new Color(0f, 0x80 / 255f, 0x80 / 255f, 1f);

        static public Dictionary<string, Color> NameToColors = new Dictionary<string, Color>();

        // 设置颜色的名称
        public static void Set(string name, Color c)
        {
            NameToColors[name] = c;
        }

        public static bool Get(string name, out Color color)
        {
            if (NameToColors.TryGetValue(name, out color))
                return true;

            switch (name)
            {
            case "aqua": color = aqua; return true;
            case "brown": color = brown; return true;
            case "darkblue": color = darkblue; return true;
            case "fuchsia": color = fuchsia; return true;
            case "lightblue": color = lightblue; return true;
            case "lime": color = lime; return true;
            case "maroon": color = maroon; return true;
            case "navy": color = navy; return true;
            case "olive": color = olive; return true;
            case "orange": color = orange; return true;
            case "purple": color = purple; return true;
            case "silver": color = silver; return true;
            case "teal": color = teal; return true;

            case "black": color = Color.black; return true;
            case "blue": color = Color.blue; return true;
            case "cyan": color = Color.cyan; return true;
            case "green": color = Color.green; return true;
            case "grey": color = Color.grey; return true;
            case "magenta": color = Color.magenta; return true;
            case "red": color = Color.red; return true;
            case "white": color = Color.white; return true;
            case "yellow": color = Color.yellow; return true;

            case "R": color = Color.red;return true;
            case "G": color = Color.green; return true;
            case "B": color = Color.blue; return true;
            case "K": color = Color.black; return true;
            case "Y": color = Color.yellow; return true;
            case "W": color = Color.white; return true;
            }

            color = Color.white;
            return false;
        }

        public static Color Get(string name, Color d)
        {
            Color c;
            if (Get(name, out c))
                return c;

            return d;
        }
    }
}