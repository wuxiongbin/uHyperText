using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace WXB
{
    public static class DrawFactory
    {
        static DrawFactory()
        {
            factorys.Add(DrawType.Outline, new Factory<OutlineDraw>());
            factorys.Add(DrawType.Alpha, new Factory<AlphaDraw>());
            factorys.Add(DrawType.Offset, new Factory<OffsetDraw>());

            factorys.Add(DrawType.OffsetAndAlpha, new Factory<AlphaOffsetDraw>());

            factorys.Add(DrawType.Cartoon, new Factory<CartoonDraw>());
        }

        public interface IFactory
        {
            Draw Create(GameObject go);

            void Free(Draw d);
        }

        static int s_total = 0;

        public static class Pool<T> where T : MonoBehaviour, Draw, new ()
        {
            public static List<T> EmptyList = new List<T>();
            public static List<T> UsedList = new List<T>();

            public static T GetOrCreate(GameObject parent)
            {
                T obj = null;
                while (EmptyList.Count != 0)
                {
                    obj = EmptyList.pop_back();
                    if (obj != null)
                        break;
                }

                if (obj == null)
                {
                    string name = (++s_total).ToString();
#if UNITY_EDITOR
                    // If we're in the editor, create the game object with hide flags set right away
                    GameObject go = UnityEditor.EditorUtility.CreateGameObjectWithHideFlags(name, HideFlags.DontSave);
                    obj = go.AddComponent<T>();
                    parent.AddChild(go);
                    obj.name = string.Format("{0}-{1}", typeof(T).Name, name);
#else
                    obj = parent.AddChild<T>();
                    obj.name = name;
#endif
                }

                obj.transform.SetParent(parent.transform);
                obj.gameObject.layer = parent.layer;
                obj.OnInit();
                return obj;
            }

            public static void Free(T d)
            {
                d.Release();

                UsedList.Remove(d);

                if (EmptyList.Count <= 10)
                {
                    EmptyList.Add(d);
                }
                else
                {
                    d.DestroySelf();
                }

#if UNITY_EDITOR
                //if (!Application.isPlaying)
                //{
                //    for (int i = 0; i < EmptyList.Count; ++i)
                //    {
                //        if (EmptyList[i] != null)
                //            EmptyList[i].DestroySelf();
                //    }

                //    EmptyList.Clear();
                //}
#endif
            }
        }

        public class Factory<T> : IFactory where T : MonoBehaviour, Draw, new()
        {
            public Draw Create(GameObject go)
            {
                return Pool<T>.GetOrCreate(go);
            }

            public void Free(Draw d)
            {
                Pool<T>.Free((T)d);
            }
        }

        static IFactory DefaultFactory = new Factory<DrawObject>();

        static Dictionary<DrawType, IFactory> factorys = new Dictionary<DrawType, IFactory>();

        static IFactory Get(DrawType type)
        {
            if (type == 0)
                return DefaultFactory;

            IFactory f = null;
            if (factorys.TryGetValue(type, out f))
                return f;

            Debug.LogErrorFormat("type:{0} not find!", type);
            return null;
        }

        static public Draw Create(GameObject go, DrawType type)
        {
            IFactory f = Get(type);
            if (f == null)
                return null;

            return f.Create(go);
        }

        static public void Free(Draw go)
        {
            if (go == null)
                return;

            IFactory f = Get(go.type);
            if (f == null)
            {
                go.DestroySelf();
                return;
            }

            f.Free(go);
        }
    }
}