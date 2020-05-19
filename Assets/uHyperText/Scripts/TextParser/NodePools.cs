using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace WXB
{
    public static class NodePools<T> where T : NodeBase, new()
    {
        public interface IPool
        {
            T Get();
            void Free(T value);
            void Release();
        }

        class Normal : IPool
        {
            T IPool.Get()
            {
                return new T();
            }

            void IPool.Free(T value)
            {

            }

            void IPool.Release()
            {

            }
        }

        class Pool : IPool
        {
            public Pool(int num)
            {
                bufs = new Queue<T>(num);
                for (int i = 0; i < num; ++i)
                    bufs.Enqueue(new T());
            }

            Queue<T> bufs;

            T IPool.Get()
            {
                if (bufs.Count == 0)
                    return new T();

                T t = bufs.Dequeue();
                return t;
            }

            void IPool.Free(T value)
            {
#if UNITY_EDITOR || COM_DEBUG
                if (bufs.Contains(value))
                {
                    Debug.LogErrorFormat("Buff<{0}>对象被重复回收!", typeof(T).Name);
                }
#endif
                bufs.Enqueue(value);
            }

            void IPool.Release()
            {
                bufs.Clear();
            }
        }

        static IPool pool = null;

        // 非对象池
        public static void SetNoPool()
        {
            pool = new Normal();
        }

        public static T Get()
        {
            if (pool == null)
                pool = new Pool(8);

            return pool.Get();
        }

        public static void Free(T t)
        {
            pool.Free(t);
        }

        public static void Release()
        {
            if (pool != null)
                pool.Release();
        }

        public static void Reset(int num)
        {
            if (pool == null)
            {
                pool = new Pool(num);
            }
        }
    }
}