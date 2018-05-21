using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Load chunks in a controlled manner
/// </summary>
public class ChunkLoader : MonoBehaviour
{
    public int maxWorkersPerFrame = System.Environment.ProcessorCount;
    public bool loadAll = false;
    public bool buildAll = false;
    public bool debugMode = false;

    public int frameBeforeDequeue = 0;

    private PriorityQueue<Chunk> loadQueue;
    private PriorityQueue<Chunk> buildQueue;

    private int frameCounter = 0;

    // Use this for initialization
    void Start()
    {
        loadQueue = new PriorityQueue<Chunk>();
        buildQueue = new PriorityQueue<Chunk>();
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCounter >= frameBeforeDequeue)
        {
            frameCounter = 0;

            DequeueLoadChunks();
            DequeueBuildChunks();
        }

        frameCounter++;
    }

    private void DequeueLoadChunks()
    {
        int workItemCount = maxWorkersPerFrame;

        // check if there are items in the queue
        while (loadQueue.Count > 0 && (workItemCount > 0 || loadAll))
        {
            // dequeue and start loading the chunk
            Chunk chunk = loadQueue.Dequeue();

            if (debugMode)
            {
                chunk.Load();
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(LoadChunkWorker), chunk);
            }

            workItemCount--;
        }
    }

    private void DequeueBuildChunks()
    {
        int workItemCount = maxWorkersPerFrame;

        while (buildQueue.Count > 0 && (workItemCount > 0 || buildAll))
        {
            Chunk chunk = buildQueue.Dequeue();

            if (debugMode)
            {
                chunk.Build();
            }
            else
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(BuildChunkWorker), chunk);
            }

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

    public void Load(Chunk chunk, float priority = 0f)
    {
        // flag the chunk is pending to be loading
        chunk.MarkLoadPending();
        // queue the chunk
        loadQueue.Enqueue(chunk, priority);
    }

    public void Build(Chunk chunk, float priority = 0f)
    {
        buildQueue.Enqueue(chunk, priority);
    }

    private void OnValidate()
    {
        if (maxWorkersPerFrame <= 0) maxWorkersPerFrame = 1;
    }
}
