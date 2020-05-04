using System.Text;
using System.Collections.Generic;

namespace WXB
{
    internal class PoolData<T> where T : new()
    {
        static public List<T> bufs = new List<T>();
        
        static public T Get()
        {
            if (bufs.Count == 0)
            {
                return new T();
            }

            T t = bufs[bufs.Count - 1];
            bufs.RemoveAt(bufs.Count - 1);

            return t;
        }

        static public void Free(T t)
        {
            bufs.Add(t);
        }

        static public void FreeList(List<T> list)
        {
            bufs.AddRange(list);
            list.Clear();
        }
    }

    internal struct PD<T> : System.IDisposable where T : new() 
    {
        public PD(System.Action<T> free)
        {
            value = PoolData<T>.Get();
            this.free = free;
        }

        public T value;
        private System.Action<T> free;

        public void Dispose()
        {
            free(value);
            PoolData<T>.Free(value);
        }
    }

    interface IFactory
    {
        object create();
    }

    internal class Factory<T> : IFactory where T : new()
    {
        public Factory(System.Action<T> f)
        {
            free = f;
        }

        public System.Action<T> free;

        public object create()
        {
            return new PD<T>(free);
        }
    }

    internal static class Pool
    {
        static Factory<StringBuilder> sb_factory = null;
        public static PD<StringBuilder> GetSB()
        {
            if (sb_factory == null)
            {
                sb_factory = new Factory<StringBuilder>((StringBuilder sb) => { sb.Length = 0; });
            }

            return (PD<StringBuilder>)sb_factory.create();
        }
    }
}