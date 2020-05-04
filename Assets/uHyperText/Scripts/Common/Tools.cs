using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace WXB
{
    public interface ISprite
    {
        int width { get; }
        int height { get; }
        
        void AddRef();
 
        void SubRef();

        // 请求资源
        Sprite Get();
    }

    static public partial class Tools
    {
        public const int s_dyn_default_speed = 320;

        public const int s_offset_default_speed = 320;

        private static readonly VertexHelper s_VertexHelper = new VertexHelper();

        public static VertexHelper vertexHelper
        {
            get { return s_VertexHelper; }
        }

        // 左下角(Y轴朝下)坐标系转换为左上角(Y轴朝上)坐标系
        public static void LB2LT(ref Vector2 pos, float height)
        {
            pos.y = height - pos.y;
        }

        public static void LT2LB(ref Vector2 pos, float height)
        {
            pos.y += height;
        }

        static public Font GetFont(string name)
        {
            if (s_get_font == null)
            {
                Font f = SymbolTextInit.GetFont(name);
                if (f == null)
                    return DefaultFont;
            }

            return s_get_font(name);
        }

        static Font DefaultFont;

        static public Font GetDefaultFont()
        {
            if (DefaultFont == null)
                DefaultFont = Object.FindObjectOfType<Font>();
            return DefaultFont;
        }

        // 得到精灵的接口
        public static System.Func<string, ISprite> s_get_sprite { set; private get;  }

        // 得到字体的接口
        public static System.Func<string, Font> s_get_font = null;

        static public ISprite GetSprite(string name)
        {
            if (s_get_sprite == null)
                return SymbolTextInit.GetSprite(name);
            return s_get_sprite(name);
        }

        public static System.Func<string, Cartoon> s_get_cartoon = null;

        static public Cartoon GetCartoon(string name)
        {
            if (s_get_cartoon == null)
                return SymbolTextInit.GetCartoon(name);

            return s_get_cartoon(name);
        }

        public static System.Action<List<Cartoon>> s_get_cartoons = null;

        static public void GetAllCartoons(List<Cartoon> cartoons)
        {
            if (s_get_cartoons == null)
            {
                SymbolTextInit.GetCartoons(cartoons);
                return;
            }

            s_get_cartoons(cartoons);
        }

        public static void AddLine(VertexHelper vh, Vector2 leftPos, Vector2 uv, float width, float height, Color color)
        {
            // 有下划线
            Vector2 leftTop = new Vector2(leftPos.x, leftPos.y);

            int count = vh.currentVertCount;
            vh.AddVert(leftTop, color, uv);
            vh.AddVert(new Vector3(leftTop.x + width, leftTop.y, 0f), color, uv);
            vh.AddVert(new Vector3(leftTop.x + width, leftTop.y - height, 0f), color, uv);
            vh.AddVert(new Vector3(leftTop.x, leftTop.y - height, 0f), color, uv);

            vh.AddTriangle(count, count + 1, count + 2);
            vh.AddTriangle(count + 2, count + 3, count);
        }

        public static bool IsHexadecimal(char c)
        {
            if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F'))
                return true;
            return false;
        }

        public static float stringToFloat(string str, float df)
        {
            float value = 0;
            if (float.TryParse(str, out value) == false)
                return df;

            return value;
        }

        public static int stringToInt(string str, int df)
        {
            int value = 0;
            if (int.TryParse(str, out value) == false)
                return df;

            return value;
        }

        public static Font ParserFontName(string text, ref int startpos)
        {
            int lenght = text.Length;
            string fontName = "";
            while (lenght > startpos && fontName.Length < 10)
            {
                fontName += text[startpos];
                startpos++;
            }

            Font font = null;
            while (fontName.Length != 0)
            {
                if ((font = GetFont(fontName)) != null)
                {
                    break;
                }
                else
                {
                    fontName = fontName.Remove(fontName.Length - 1, 1);
                    startpos--;
                }
            }

            return font;
        }

        public static bool ScreenPointToWorldPointInRectangle(RectTransform rectTrans, Vector2 screenPoint, Camera cam, out Vector2 worldPoint)
        {
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, screenPoint, cam, out worldPoint))
                return false;

            LB2LT(ref worldPoint, rectTrans.rect.height);
            return true;

            //             worldPoint = Vector2.zero;
            //             Ray ray = RectTransformUtility.ScreenPointToRay(cam, screenPoint);
            //             float enter;
            //             if (!new Plane(rectTrans.rotation * Vector3.back, rectTrans.position).Raycast(ray, out enter))
            //                 return false;

//             worldPoint = ray.GetPoint(enter);
//             worldPoint = rectTrans.worldToLocalMatrix.MultiplyPoint(worldPoint);
        }

        static public Color ParseColor(string text, int offset, Color defc)
        {
            Color c;
            if (ParseColor(text, offset, out c))
                return c;

            return defc;
        }

        static public bool ParseColor(string text, int offset, out Color color)
        {
            try
            {
                color = Color.white;
                int l = text.Length - offset;
                if (l >= 8)
                {
                    color = ParseColor32(text, offset);
                    return true;
                }
                else if (l >= 6)
                {
                    color = ParseColor24(text, offset);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                color = Color.white;
                Debug.LogException(ex);
            }

            return false;
        }

        static public Color ParseColor24(string text, int offset)
        {
            int r = (HexToDecimal(text[offset]) << 4) | HexToDecimal(text[offset + 1]);
            int g = (HexToDecimal(text[offset + 2]) << 4) | HexToDecimal(text[offset + 3]);
            int b = (HexToDecimal(text[offset + 4]) << 4) | HexToDecimal(text[offset + 5]);
            float f = 1f / 255f;
            return new Color(f * r, f * g, f * b);
        }

        static public Color ParseColor32(string text, int offset)
        {
            int r = (HexToDecimal(text[offset]) << 4) | HexToDecimal(text[offset + 1]);
            int g = (HexToDecimal(text[offset + 2]) << 4) | HexToDecimal(text[offset + 3]);
            int b = (HexToDecimal(text[offset + 4]) << 4) | HexToDecimal(text[offset + 5]);
            int a = (HexToDecimal(text[offset + 6]) << 4) | HexToDecimal(text[offset + 7]);
            float f = 1f / 255f;
            return new Color(f * r, f * g, f * b, f * a);
        }

        static public int HexToDecimal(char ch)
        {
            switch (ch)
            {
            case '0': return 0x0;
            case '1': return 0x1;
            case '2': return 0x2;
            case '3': return 0x3;
            case '4': return 0x4;
            case '5': return 0x5;
            case '6': return 0x6;
            case '7': return 0x7;
            case '8': return 0x8;
            case '9': return 0x9;
            case 'a':
            case 'A': return 0xA;
            case 'b':
            case 'B': return 0xB;
            case 'c':
            case 'C': return 0xC;
            case 'd':
            case 'D': return 0xD;
            case 'e':
            case 'E': return 0xE;
            case 'f':
            case 'F': return 0xF;
            }
            return 0xF;
        }

        static public Color ParserColorName(string text, Color dc)
        {
            int startpos = 0;
            return ParserColorName(text, ref startpos, dc);
        }

        static public Color ParserColorName(string text, ref int startpos, Color dc)
        {
            if (startpos >= text.Length - 1)
                return dc;

            if (text[startpos + 1] == '[')
            {
                int endpos = text.IndexOf(']', startpos + 1);
                if (endpos != -1)
                {
                    string name = text.Substring(startpos + 2, endpos - startpos - 2);
                    startpos = endpos+1;
                    return ColorConst.Get(name, dc);
                }

                ++startpos;
                return dc;
            }
            else
            {
                // c,后面接的字符定义为字体的颜色，如果后面字符不是数字，则颜色恢复为默认的颜色,最多六个数字
                int start_color = ++startpos;
                int color_lenght = 0;
                int lenght = text.Length;
                while (lenght > (startpos + color_lenght) && IsHexadecimal(text[startpos + color_lenght]) && color_lenght < 8)
                {
                    ++color_lenght;
                }

                Color newCol;
                if (ParseColor(text.Substring(start_color, color_lenght), 0, out newCol))
                {
                    startpos += color_lenght;
                    return newCol;
                }
                else
                {
                    startpos += color_lenght;
                    return dc;
                }
            }
        }

        public static T AddChild<T>(this GameObject go) where T : MonoBehaviour
        {
            GameObject child = new GameObject();
            var t = child.AddComponent<T>();
            AddChild(go, child);

            return t;
        }

        public static void AddChild(this GameObject go, GameObject child)
        {
            Transform ctf = child.transform;
            ctf.SetParent(go.transform);

            ctf.localScale = Vector3.one;
            ctf.localPosition = Vector3.zero;
            ctf.localEulerAngles = Vector3.zero;

            child.layer = go.layer;
        }

        static public void Destroy(GameObject go)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
            {
                Object.Destroy(go);
                //DelayDestory.Destroy(go);
            }
            else
            {
                Object.DestroyImmediate(go);
                //DelayDestory.DestroyImmediate(go);
            }
#else
            Object.Destroy(go);
            //DelayDestory.Destroy(go);            
#endif
        }

        public static void UpdateRect(RectTransform child, Vector2 offset)
        {
            RectTransform parent = child.parent as RectTransform;
            if (parent == null)
                return;

            child.pivot = parent.pivot;
            child.anchorMin = Vector2.zero;
            child.anchorMax = Vector2.one;

            child.offsetMax = Vector2.zero;
            child.offsetMin = Vector2.zero;

            child.localPosition = offset;
            child.localScale = Vector3.one;
            child.localEulerAngles = Vector3.zero;
        }
    }
}