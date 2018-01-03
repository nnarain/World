using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Load chunks in a controlled manner
/// </summary>
public class ChunkLoader : MonoBehaviour
{
    public int maxThreads = 10;

    private Queue<Chunk> buildQueue;
    private List<Chunk> currentlyBuildingChunks;

    // Use this for initialization
    void Start()
    {
        buildQueue = new Queue<Chunk>();
        currentlyBuildingChunks = new List<Chunk>();
    }

    // Update is called once per frame
    void Update()
    {
        // check if there are items in the queue
        if (buildQueue.Count > 0)
        {
            // check if the number of currently building chunks is less than the max
            if (currentlyBuildingChunks.Count < maxThreads)
            {
                // dequeue and start building the chunk
                Chunk chunk = buildQueue.Dequeue();
                chunk.Build();
                //Debug.Log(string.Format("Building the chunk ({0},{1})", chunk.transform.position.x, chunk.transform.position.z));


                // add the chunk to the active list
                currentlyBuildingChunks.Add(chunk);
            }
        }

        // check each chunk to see if it is finished loading
        if (currentlyBuildingChunks.Count > 0)
        {
            for (int i = 0; i < currentlyBuildingChunks.Count; ++i)
            {
                var chunk = currentlyBuildingChunks[i];

                if (chunk.IsLoaded)
                {
                //    Debug.Log(string.Format("The chunk ({0},{1}) is done building", chunk.transform.position.x, chunk.transform.position.z));
                    currentlyBuildingChunks.RemoveAt(i);
                }
            }
        }
    }

    public void Enqueue(Chunk chunk)
    {
        buildQueue.Enqueue(chunk);
    }
}
