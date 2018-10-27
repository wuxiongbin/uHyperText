using UnityEngine;
using System.Text;
using System.Collections.Generic;

namespace WXB
{
    static class SpriteMaterial
    {
        static Dictionary<Texture, Material> SpriteToMaterials = new Dictionary<Texture, Material>();

        static public Material Get(Texture t)
        {
            Material m = null;
            if (SpriteToMaterials.TryGetValue(t, out m))
                return m;

            m = new Material(Canvas.GetDefaultCanvasMaterial());
            m.mainTexture = t;
            SpriteToMaterials.Add(t, m);
            return m;
        }
    }

    static class FontMaterial
    {
        static Dictionary<Font, Material> FontToMaterials = new Dictionary<Font, Material>();

        static public Material Get(Font f)
        {
            Material m = null;
            if (FontToMaterials.TryGetValue(f, out m))
            {
                if (m == null)
                {
                    FontToMaterials.Remove(f);
                }
                else
                {
                    return m;
                }
            }

            m = new Material(Shader.Find("UI/Unlit/Text Detail"));
            m.mainTexture = f.material.mainTexture;
            FontToMaterials.Add(f, m);
            return m;
        }
    }
}