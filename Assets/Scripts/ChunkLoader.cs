using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Load chunks in a controlled manner
/// </summary>
public class ChunkLoader : MonoBehaviour
{
    public int maxWorkersPerFrame;

    private Queue<Chunk> loadQueue;

    // Use this for initialization
    void Start()
    {
        loadQueue = new Queue<Chunk>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if there are items in the queue
        if (loadQueue.Count > 0)
        {
            // dequeue and start loading the chunk
            Chunk chunk = loadQueue.Dequeue();
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadChunk), chunk);
        }
    }

    static void LoadChunk(object data)
    {
        // Load the chunk
        Chunk chunk = (Chunk)data;
        chunk.Load();
    }

    static void BuildChunk(object data)
    {
        // build the chunk
        Chunk chunk = (Chunk)data;
        chunk.Build();
    }

    public void Enqueue(Chunk chunk)
    {
        // flag the chunk is pending to be loading
        chunk.MarkLoadPending();
        // queue the chunk
        loadQueue.Enqueue(chunk);
    }
}
