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

        public T GetItem()
        {
            try
            {
                if (lst.Count == 0)
                {
                    return default(T);
                }
                return lst.Dequeue();
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
