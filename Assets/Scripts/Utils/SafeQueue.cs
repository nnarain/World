using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SafeQueue<T> : Queue<T>
{
    private readonly object lockObj = new object();

    public bool Empty { get { return base.Count > 0; } }

    public SafeQueue()
    {

    }

    public new void Enqueue(T item)
    {
        lock(lockObj)
        {
            base.Enqueue(item);
        }
    }

    public new T Dequeue()
    {
        T item = default(T);

        lock(lockObj)
        {
            item = base.Dequeue();
        }

        return item;
    }
}
