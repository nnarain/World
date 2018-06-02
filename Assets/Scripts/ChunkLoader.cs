using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Load chunks in a controlled manner
/// </summary>
public class ChunkLoader : MonoBehaviour
{
    public enum LoadMode
    {
        Full,
        Reserved
    }

    [System.Serializable]
    public struct LoadParams
    {
        public int maxThreads;
        public int framesBeforeDequeue;
    }

    public LoadParams[] loadParams = new LoadParams[2];

    public bool debugMode = false;

    private LoadMode loadMode;

    private int maxThreads = 1;
    private int frameBeforeDequeue = 0;


    private PriorityQueue<Chunk> loadQueue;
    private PriorityQueue<Chunk> buildQueue;

    private int frameCounter;
    private int activeThreads;

    // Use this for initialization
    void Start()
    {
        loadQueue = new PriorityQueue<Chunk>();
        buildQueue = new PriorityQueue<Chunk>();

        loadMode = LoadMode.Full;
        frameCounter = 0;
        activeThreads = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (frameCounter >= frameBeforeDequeue)
        {
            frameCounter = 0;

            DequeueLoadChunks();
        }

        frameCounter++;
    }
    
    public void Requeue(Func<Chunk, float> eval)
    {
        loadQueue = Requeue(loadQueue, eval);
    //    buildQueue = Requeue(buildQueue, eval);
    }
    
    private PriorityQueue<Chunk> Requeue(PriorityQueue<Chunk> queue, Func<Chunk, float> eval)
    {
        PriorityQueue<Chunk> newQueue = new PriorityQueue<Chunk>();

        while (!queue.Empty)
        {
            // get a chunk
            var chunk = queue.Dequeue();
            // evaluate its priority
            var priority = eval(chunk);

            // A priority of less than 0 is not valid and indicates that the chunk should not be requeued
            if (priority >= 0.0f)
            {
                newQueue.Enqueue(chunk, priority);
            }
        }

        return newQueue;
    }

    public void SetLoadMode(LoadMode mode)
    {
        loadMode = mode;
        maxThreads = loadParams[(int)mode].maxThreads;
        frameBeforeDequeue = loadParams[(int)mode].framesBeforeDequeue;
    }

    private void DequeueLoadChunks()
    {
        // check if there are items in the queue
        while (AvailableThreads() > 0 && !loadQueue.Empty)
        {
            // dequeue and start loading the chunk
            Chunk chunk = loadQueue.Dequeue();

            if (debugMode)
            {
                chunk.Load();
            }
            else
            {
                Interlocked.Increment(ref activeThreads);
                ThreadPool.QueueUserWorkItem(new WaitCallback(LoadChunkWorker), chunk);
            }
        }
    }

    private void DequeueBuildChunks()
    {
        if (AvailableThreads() > 0)
        {
            // check if there are items in the queue
            while (!buildQueue.Empty)
            {
                // dequeue and start loading the chunk
                Chunk chunk = buildQueue.Dequeue();

                if (debugMode)
                {
                    chunk.Build();
                }
                else
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(BuildChunkWorker), chunk);
                }
            }
        }
    }

    public int AvailableThreads()
    {
        return maxThreads - activeThreads;
    }

    /// <summary>
    /// Load a chunk of the worker thread
    /// </summary>
    /// <param name="data"></param>
    void LoadChunkWorker(object data)
    {
        Chunk chunk = (Chunk)data;
        chunk.Load();

        Interlocked.Decrement(ref activeThreads);
    }

    void BuildChunkWorker(object data)
    {
        Interlocked.Increment(ref activeThreads);

        Chunk chunk = (Chunk)data;
        chunk.Build();

        Interlocked.Decrement(ref activeThreads);
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

    }
}
