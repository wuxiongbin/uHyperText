using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;
using WXB;

public class CreateAnim
{
    [MenuItem("Assets/CheckFileName")]
    static void CheckFileName()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        HashSet<string> guilds = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { assetPath }));
        foreach (string guid in guilds)
        {
            string ap = AssetDatabase.GUIDToAssetPath(guid);
            //if (ap.EndsWith("_.png"))
            //{
            //    Debug.LogFormat(ap);
            //}

            if (string.IsNullOrEmpty(ap) || !ap.EndsWith(".png", true, null))
                continue;

            int pos = ap.LastIndexOf('/');
            if (!ap.Substring(pos + 1).StartsWith("anim_"))
                continue;

            int pos_ = ap.LastIndexOf('_');
            if (pos_ == -1)
                continue;

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append(ap.Substring(0, pos_ + 1));
            int m = pos_ + 1;
            int e = ap.LastIndexOf('.') - 1;
            for (; m < e; ++m)
            {
                if (ap[m] != '0')
                    break;
            }

            sb.Append(ap.Substring(m, ap.Length - m));
            //Debug.LogFormat("{0}->{1}", ap.Substring(7), sb.ToString().Substring(7));
            AssetDatabase.CopyAsset(ap, sb.ToString());
            AssetDatabase.DeleteAsset(ap);
        }
    }


    [MenuItem("Assets/更新动画数据")]
    static void UpdateAnim()
    {
        string assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);

        HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { assetPath }));

        Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();
        foreach (string guid in guids)
        {
            Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GUIDToAssetPath(guid));
            foreach (Object o in objs)
            {
                if (o is Sprite)
                    sprites.Add(o.name, o as Sprite);
            }
        }

        Dictionary<string, Cartoon> Cartoons = new Dictionary<string, Cartoon>();
        List<Sprite> tempss = new List<Sprite>();
        for (int i = 0; i < 1000; ++i)
        {
            string animName = string.Format("anim_{0}_", i);
            for (int j = 0; j < 100; ++j)
            {
                string frameName = animName + j;
                Sprite s = null;
                if (sprites.TryGetValue(frameName, out s))
                    tempss.Add(s);
             }

            if (tempss.Count != 0)
            {
                Cartoon c = new Cartoon();
                c.name = i.ToString();
                c.fps = 5f;
                c.sprites = tempss.ToArray();
                c.width = (int)c.sprites[0].rect.width;
                c.height = (int)c.sprites[0].rect.height;

                Cartoons.Add(i.ToString(), c);

                tempss.Clear();
            }
        }

        SymbolTextInit sti = Resources.Load<SymbolTextInit>("SymbolTextInit");
        FieldInfo info = typeof(SymbolTextInit).GetField("cartoons", BindingFlags.Instance | BindingFlags.NonPublic);
        List<Cartoon> cartoons = new List<Cartoon>(Cartoons.Values);
        cartoons.Sort((Cartoon x, Cartoon y) => 
        {
            return int.Parse(x.name).CompareTo(int.Parse(y.name));
        });

        info.SetValue(sti, cartoons.ToArray());
        EditorUtility.SetDirty(sti);
    }
}
