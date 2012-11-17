using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FxTask
{
    public class FxQueue<T>
    {
        Queue<T> lst = new Queue<T>();

        public void Add(T item)
        {
            if (item != null && !lst.Contains(item))
            {
                lst.Enqueue(item);
            }
        }

        public void AddRange(ICollection<T> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public object lockd = new object();

        public T GetItem(Func<T,bool> func)
        {
            try
            {
                if (lst.Count == 0)
                {
                    return default(T);
                }
                else
                {
                    lock (lockd)
                    {
                        T t = lst.Dequeue();
                        func(t);
                        return t;
                    }
                }
            }
            catch (Exception ex)
            {
                ex.LogEx("FxTask.FxQueue<T> GetItem()");
            }
            return default(T);
        }

        public bool HasItem()
        {
            return lst.Count > 0;
        }

        public int Count
        {
            get { return lst.Count; }
        }
    }
}
