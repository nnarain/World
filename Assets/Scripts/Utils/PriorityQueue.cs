using System;
using System.Collections.Generic;

/// <summary>
/// PriorityQueue class
/// </summary>
/// <summary>
/// A priority queue
/// </summary>
/// <typeparam name="T">Type of the item stored in the queue</typeparam>
class PriorityQueue<T>
{
    private class Node<ItemT>
    {
        public ItemT item = default(ItemT);
        public float priority = 0.0f;
    }

    private LinkedList<Node<T>> data = null;

    public int Count { get { return data.Count; } }
    public bool Empty { get { return data.Count == 0; } }

    public PriorityQueue()
    {
        data = new LinkedList<Node<T>>();
    }

    public void Clear()
    {
        
        data.Clear();
    }

    /// <summary>
    /// Add an item to the queue. Smallest distance to largest distance
    /// </summary>
    /// <param name="item">Item to add</param>
    /// <param name="priority"></param>
    public void Enqueue(T item, float priority)
    {
        Node<T> node = new Node<T>();
        node.item = item;
        node.priority = priority;

        // if the list is empty add the initial node
        if (data.Count == 0)
        {
            data.AddFirst(node);
        }
        else
        {
            bool added = false;

            // iterate through the linked list
            for (var iter = data.First; iter != null; iter = iter.Next)
            {
                var currentNode = iter.Value;

                // insert the node before a node with a greater distance
                if (node.priority < currentNode.priority)
                {
                    data.AddBefore(iter, node);
                    added = true;
                    break;
                }
            }

            if (!added)
                data.AddLast(node);
        }
    }

    /// <summary>
    /// Get the first item in the queue
    /// </summary>
    /// <returns></returns>
    public T Dequeue()
    {
        // get the first node
        var node = data.First.Value;
        // remove the first node
        data.RemoveFirst();

        // return the first item in the queue
        return node.item;
    }

    public bool Contains(T item)
    {
        for (var iter = data.First; iter != null; iter = iter.Next)
        {
            if (iter.Value.item.Equals(item))
            {
                return true;
            }
        }

        return false;
    }
}