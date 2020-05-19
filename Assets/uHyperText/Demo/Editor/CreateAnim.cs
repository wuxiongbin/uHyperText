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

    class Frame
    {
        public int index;
        public Cartoon.Frame frame;
    }

    static int Sorted(Cartoon x, Cartoon y)
    {
        int xv, yv;
        bool xvv = int.TryParse(x.name, out xv);
        bool yvv = int.TryParse(y.name, out yv);

        if (xvv == yvv)
        {
            if (xvv)
            {
                return xv.CompareTo(yv);
            }

            return x.name.CompareTo(y.name);
        }

        if (xvv)
            return -1;

        return 1;
    }

    [MenuItem("Assets/更新动画数据")]
    static void UpdateAnim()
    {
        Dictionary<string, List<Frame>> anims = new Dictionary<string, List<Frame>>();
        {
            HashSet<string> guids = new HashSet<string>(AssetDatabase.FindAssets("", new string[] { "Assets" }));
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                Object[] objs = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
                foreach (Object o in objs)
                {
                    if (!(o is Sprite) || !assetPath.Contains("/anim_"))
                        continue;

                    string name;
                    int frame;
                    int time;
                    {
                        string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath);
                        int sp = fileName.LastIndexOf('_');
                        name = fileName.Substring(5, sp - 5);
                        int sp1 = fileName.LastIndexOf('#');
                        if (sp1 == -1)
                        {
                            frame = int.Parse(fileName.Substring(sp + 1));
                            time = (int)(1f / 6f * 1000);
                        }
                        else
                        {
                            frame = int.Parse(fileName.Substring(sp + 1, sp1 - sp - 1));
                            time = int.Parse(fileName.Substring(sp1 + 1));
                        }
                    }

                    if (!anims.TryGetValue(name, out var f))
                    {
                        anims.Add(name, f = new List<Frame>());
                    }

                    f.Add(new Frame
                    {
                        index = frame,
                        frame = new Cartoon.Frame
                        {
                            sprite = new DSprite(o as Sprite),
                            delay = time * 0.001f,
                        },
                    });
                }
            }
        }

        Dictionary<string, Cartoon> Cartoons = new Dictionary<string, Cartoon>();
        {
            foreach (var ator in anims)
            {
                Cartoon cartoon = new Cartoon();
                Cartoons.Add(ator.Key, cartoon);
                cartoon.name = ator.Key;

                var frames = ator.Value;
                int count = frames.Count;
                cartoon.frames = new Cartoon.Frame[count];
                for (int i = 0; i < count; ++i)
                {
                    var f = frames[i];
                    cartoon.frames[f.index - 1] = f.frame;
                }

                var fs = cartoon.frames[0].sprite;
                cartoon.width = fs.width;
                cartoon.height = fs.height;
            }
        }

        SymbolTextInit sti = Resources.Load<SymbolTextInit>("SymbolTextInit");
        FieldInfo info = typeof(SymbolTextInit).GetField("cartoons", BindingFlags.Instance | BindingFlags.NonPublic);
        List<Cartoon> cartoons = new List<Cartoon>(Cartoons.Values);
        cartoons.Sort(Sorted);

        info.SetValue(sti, cartoons.ToArray());
        EditorUtility.SetDirty(sti);
    }

    static Cartoon.Frame[] To(IList<Sprite> sprites)
    {
        int count = sprites.Count;
        Cartoon.Frame[] ds = new Cartoon.Frame[count];
        for (int i = 0; i < count; ++i)
        {
            var frame = new Cartoon.Frame();
            frame.sprite = new DSprite(sprites[i]);
            frame.delay = 1f / 6f;

            ds[i] = frame;
        }

        return ds;
    }
}
