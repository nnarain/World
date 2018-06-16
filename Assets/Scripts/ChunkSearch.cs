using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

/// <summary>
/// Search for chunks
/// </summary>
public class ChunkSearch
{
    private Thread searchThread = null;

    private SafeDictionary<Vector3Int, Chunk> chunkList;
    private PriorityQueue<Vector3Int> loadQueue;

    private float searchRadius;
    private bool running = false;
    public bool Active { get { return running; } }

    private int chunkSizeX, chunkSizeY, chunkSizeZ;

    public ChunkSearch(SafeDictionary<Vector3Int, Chunk> chunkList, int chunkSizeX, int chunkSizeY, int chunkSizeZ, float radius)
    {
        searchRadius = radius;

        this.chunkSizeX = chunkSizeX;
        this.chunkSizeX = chunkSizeY;
        this.chunkSizeX = chunkSizeZ;
    }

    public Vector3Int Dequeue()
    {
        return loadQueue.Dequeue();
    }

    public void Start(Vector3 playerPosition, Vector3Int chunkPosition)
    {
        if (running)
            Stop();

        running = true;
        searchThread = new Thread(() => Search(playerPosition, chunkPosition));
        searchThread.Start();
    }

    public void Stop()
    {
        running = false;
        searchThread.Join();

        loadQueue.Clear();
    }

    private void Search(Vector3 position, Vector3Int startPosition)
    {
        // the player's chunk position
        Vector3Int playerChunkPosition = startPosition;

        // explored chunks
        HashSet<Vector3Int> explored = new HashSet<Vector3Int>();

        // queue of chunk positions
        Queue<Vector3Int> queue = new Queue<Vector3Int>();

        // load initial chunk positions to check
        queue.Enqueue(playerChunkPosition);
        foreach (var key in GetChunkNeighbors(playerChunkPosition))
        {
            queue.Enqueue(key);
        }

        // loop while items are in the queue
        while (queue.Count > 0)
        {
            // grab first position
            Vector3Int p = queue.Dequeue();

            if (explored.Contains(p)) continue;

            // explore neighbors
            Vector3Int[] neighbors = GetChunkNeighbors(p);

            foreach (var neighbor in neighbors)
            {
                // get the world position of the chunk
                Vector3 chunkPosition = GetChunkWorldCenter(neighbor);

                // check the chunk is in the render distance of the player
                var distanceFromPlayer = (position - chunkPosition).magnitude;

                if (distanceFromPlayer <= searchRadius)
                {
                    queue.Enqueue(neighbor);
                }
            }

            explored.Add(p);

            if (!chunkList.ContainsKey(p))
            {
                loadQueue.Enqueue(p, CalculateChunkPriority(p, position));
            }
        }
    }

    private float CalculateChunkPriority(Vector3Int p, Vector3 position)
    {
        var x = (float)chunkSizeX * p.x;
        var z = (float)chunkSizeZ * p.z;
        var dx = x - position.x;
        var dz = z - position.z;

        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    private Vector3 GetChunkWorldCenter(Vector3Int p)
    {
        Vector3 world = GetWorldPositionFromChunkPosition(p);
        world.x += chunkSizeX / 2;
        world.y += chunkSizeY / 2;
        world.z += chunkSizeZ / 2;

        return world;
    }

    /// <summary>
    /// Get the world space position of the chunk
    /// </summary>
    /// <param name="chunkPosition"></param>
    /// <returns></returns>
    private Vector3 GetWorldPositionFromChunkPosition(Vector3Int chunkPosition)
    {
        Vector3 worldPosition = new Vector3();

        worldPosition.x = chunkPosition.x * chunkSizeX;
        worldPosition.y = chunkPosition.y * chunkSizeY;
        worldPosition.z = chunkPosition.z * chunkSizeZ;

        return worldPosition;
    }

    /// <summary>
    /// Get the chunk neighbors in the order l, r, f, and n
    /// </summary>
    /// <param name="p"></param>
    /// <returns></returns>
    private Vector3Int[] GetChunkNeighbors(Vector3Int p)
    {
        return new Vector3Int[]
        {
            new Vector3Int(p.x - 1, 0, p.z),
            new Vector3Int(p.x + 1, 0, p.z),
            new Vector3Int(p.x, 0, p.z + 1),
            new Vector3Int(p.x, 0, p.z - 1)
        };
    }
}
