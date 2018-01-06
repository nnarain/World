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
    public bool loadAll = false;
    public bool buildAll = false;

    private Queue<Chunk> loadQueue;
    private Queue<Chunk> buildQueue;

    // Use this for initialization
    void Start()
    {
        loadQueue = new Queue<Chunk>();
        buildQueue = new Queue<Chunk>();
    }

    // Update is called once per frame
    void Update()
    {
        int workItemCount = maxWorkersPerFrame;

        // check if there are items in the queue
        while (loadQueue.Count > 0 && (workItemCount > 0 || loadAll))
        {
            // dequeue and start loading the chunk
            Chunk chunk = loadQueue.Dequeue();
            ThreadPool.QueueUserWorkItem(new WaitCallback(LoadChunkWorker), chunk);

            workItemCount--;
        }

        workItemCount = maxWorkersPerFrame;

        while (buildQueue.Count > 0 && (workItemCount > 0 || buildAll))
        {
            Chunk chunk = buildQueue.Dequeue();
            ThreadPool.QueueUserWorkItem(new WaitCallback(BuildChunkWorker), chunk);

            workItemCount--;
        }
    }

    static void LoadChunkWorker(object data)
    {
        // Load the chunk
        Chunk chunk = (Chunk)data;
        chunk.Load();
    }

    static void BuildChunkWorker(object data)
    {
        // build the chunk
        Chunk chunk = (Chunk)data;
        chunk.Build();
    }

    public void Load(Chunk chunk)
    {
        // flag the chunk is pending to be loading
        chunk.MarkLoadPending();
        // queue the chunk
        loadQueue.Enqueue(chunk);
    }

    public void Build(Chunk chunk)
    {
        buildQueue.Enqueue(chunk);
    }

    private void OnValidate()
    {
        if (maxWorkersPerFrame <= 0) maxWorkersPerFrame = 1;
    }
}
