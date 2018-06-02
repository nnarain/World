﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ChunkLoader))]
public class ChunkManager : MonoBehaviour
{
    public GameObject player;

    public Chunk chunkPrefab;

    public float generalRenderDistance;
    public float moveThreshold;
    public float distanceToDestroy;

    public int framesBeforeLoad;
    public int maxChunksToCreatePerFrame;

    [Range(0, 1)]
    public float requeueViewThreshold;

    public float fullLoadDuration = 0;
    private float fullLoadTimer = 0;
    private bool doneLoading = false;
    private bool searchForDistantChunks = false;

    [Range(0, 1f)]
    public float viewportMargin;

    private Vector3 lastPlayerPosition;

    private SafeDictionary<Vector3Int, Chunk> chunkList;
    private Camera playerCamera;
    private ChunkLoader chunkLoader;

    private PriorityQueue<Vector3Int> premptiveLoadQueue;
    private Queue<Chunk> removeChunksQueue;

    private bool loadVisibleChunks = false;
    private bool loadLocalVisibleChunks = false;

    private int frameCounter = 0;


    // Use this for initialization
    private void Start()
    {
        chunkLoader = GetComponent<ChunkLoader>();
        chunkList = new SafeDictionary<Vector3Int, Chunk>();
        playerCamera = player.GetComponentInChildren<Camera>();

        premptiveLoadQueue = new PriorityQueue<Vector3Int>();
        removeChunksQueue = new Queue<Chunk>();

        lastPlayerPosition = player.transform.position;

        chunkLoader.SetLoadMode(ChunkLoader.LoadMode.Full);
        UpdateSurroundingChunks(player.transform.position);
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePlayerPosition(player.transform.position);
        HandleLoadingChunks();

        fullLoadTimer += Time.deltaTime;
        if (fullLoadTimer >= fullLoadDuration && !doneLoading)
        {
            chunkLoader.SetLoadMode(ChunkLoader.LoadMode.Reserved);
            doneLoading = true;
        }
    }

    private void HandleLoadingChunks()
    {
        if (frameCounter >= framesBeforeLoad)
        {
            frameCounter = 0;

            // create chunks from queue
            int createCount = 0;
            while (!premptiveLoadQueue.Empty && createCount < maxChunksToCreatePerFrame)
            {
                var p = premptiveLoadQueue.Dequeue();
                var chunk = CreateChunk(p.x, p.z);
                chunkList.Add(p, chunk);

                createCount++;

                chunkLoader.Load(chunk, CalculateChunkPriority(chunk));
            }
        }
        frameCounter++;
    }

    private void HandleRemovingChunks()
    {
        while (removeChunksQueue.Count > 0)
        {
            var chunk = removeChunksQueue.Dequeue();
            Destroy(chunk);
        }
    }

    private void UpdatePlayerPosition(Vector3 playerPosition)
    {
        // check if the player has moved the threshold distance.
        var displacement = playerPosition - lastPlayerPosition;

        if (displacement.magnitude > moveThreshold)
        {
            lastPlayerPosition = playerPosition;

            // re-evalutate chunk priority loading
            //chunkLoader.Requeue(EvaluateChunksForLoading);

            // start loading chunks from a point in the direction the player is heading
            var searchDistance = (generalRenderDistance - displacement.magnitude) * .75f;
            UpdateSurroundingChunks(playerPosition + (displacement.normalized * searchDistance));

            // start removing chunks
            if (!searchForDistantChunks)
            {
                searchForDistantChunks = true;
                ThreadPool.QueueUserWorkItem(c => QueueChunksForRemoval());
            }

            if (!loadLocalVisibleChunks)
            {
                loadLocalVisibleChunks = true;
                ThreadPool.QueueUserWorkItem(c => UpdateVisibleChunks(playerPosition, generalRenderDistance, () => loadLocalVisibleChunks = false));
            }
        }
    }

    private float EvaluateChunksForLoading(Chunk chunk)
    {
        var position = player.transform.position;
        var forward = player.transform.forward;

        // check displacement from the player
        var chunkDisplacementFromPlayer = chunk.transform.position - position;

        // postive - in front of player, negative - behind the player
        var dot = Vector3.Dot(forward, chunkDisplacementFromPlayer.normalized);

        // do not bother loading chunks that are behind the player
        if (dot > 0 || (Mathf.Abs(dot) > requeueViewThreshold))
        {
            return chunkDisplacementFromPlayer.magnitude;
        }
        else
        {
            // this chunk will be removed from loading, remove it from the chunk list
            chunkList.Remove(GetChunkPosition(chunk.transform.position));

            return -1.0f;
        }
    }

    /// <summary>
    /// Update chunks around the player
    /// </summary>
    /// <param name="position"></param>
    private void UpdateSurroundingChunks(Vector3 position)
    {
        if (!loadVisibleChunks)
        {
            loadVisibleChunks = true;
            ThreadPool.QueueUserWorkItem(c => UpdateVisibleChunks(position, generalRenderDistance, () => loadVisibleChunks = false));
        }
    }

    /// <summary>
    /// Use a breath-first search to queue surrounding chunks for loading
    /// </summary>
    /// <param name="position"></param>
    private void UpdateVisibleChunks(Vector3 position, float maxRadius, Action fn)
    {
        // the player's chunk position
        Vector3Int playerChunkPosition = GetChunkPosition(position);

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

                if (distanceFromPlayer <= maxRadius)
                {
                    queue.Enqueue(neighbor);
                }
            }

            explored.Add(p);

            if (!chunkList.ContainsKey(p))
            {
                premptiveLoadQueue.Enqueue(p, CalculateChunkPriority(p));
            }
        }

        fn();
    }

    private void QueueChunksForRemoval()
    {
        var cx = chunkPrefab.chunkSizeX;
        var cz = chunkPrefab.chunkSizeZ;

        foreach (var item in chunkList)
        {
            // chunk position
            var cp = item.Key;
            var position = new Vector3(cp.x * cx + cx / 2, 0, cp.z * cz + cz / 2);

            if ((lastPlayerPosition - position).magnitude > distanceToDestroy)
            {
                removeChunksQueue.Enqueue(item.Value);
            }

            Thread.Sleep(50);
        }

        searchForDistantChunks = false;
    }

    private void OnChunkLoad(Chunk chunk)
    {
        chunk.Build();
    }

    private void OnChunkBuild(Chunk chunk)
    {

    }

    private float CalculateChunkPriority(Chunk chunk)
    {
        return (chunk.transform.position - player.transform.position).magnitude;
    }

    private float CalculateChunkPriority(Vector3Int p)
    {
        var x = (float)chunkPrefab.chunkSizeX * p.x;
        var z = (float)chunkPrefab.chunkSizeZ * p.z;
        var dx = x - lastPlayerPosition.x;
        var dz = z - lastPlayerPosition.z;

        return Mathf.Sqrt(dx * dx + dz * dz);
    }

    private Chunk CreateChunk(int x, int z)
    {
        int chunkSizeX = chunkPrefab.chunkSizeX;
        //int chunkSizeY = chunkPrefab.chunkSizeY;
        int chunkSizeZ = chunkPrefab.chunkSizeZ;

        // if the chunk does not exist yet, create it.
        var chunk = Instantiate(chunkPrefab);
        chunk.SetPosition(new Vector3(x * chunkSizeX, 0, z * chunkSizeZ));
        chunk.transform.SetParent(transform, false);
        chunk.SetOnLoadCallback(OnChunkLoad);
        chunk.SetOnBuildCallback(OnChunkBuild);

        // Set chunk neighbors

        Vector3Int[] neighbors = GetChunkNeighbors(new Vector3Int(x, 0, z));
        Direction[] directions = { Direction.Left, Direction.Right, Direction.Far, Direction.Near };

        for (int i = 0; i < neighbors.Length; ++i)
        {
            if (chunkList.ContainsKey(neighbors[i]))
            {
                chunk.SetNeighbor(chunkList.Get(neighbors[i]), directions[i]);
            }
        }

        return chunk;
    }

    private bool IsChunkInFrustum(Vector3 chunkCenter)
    {
        float offsetX = (float)chunkPrefab.chunkSizeX / 2.0f;
        float offsetY = (float)chunkPrefab.chunkSizeY / 2.0f;
        float offsetZ = (float)chunkPrefab.chunkSizeZ / 2.0f;

        Vector3[] corners =
        {
            new Vector3(chunkCenter.x - offsetX, chunkCenter.y + offsetY, chunkCenter.z + offsetZ),
            new Vector3(chunkCenter.x + offsetX, chunkCenter.y + offsetY, chunkCenter.z + offsetZ),
            new Vector3(chunkCenter.x - offsetX, chunkCenter.y + offsetY, chunkCenter.z - offsetZ),
            new Vector3(chunkCenter.x + offsetX, chunkCenter.y + offsetY, chunkCenter.z - offsetZ)
        };

        foreach (var corner in corners)
        {
            if (playerCamera.IsPointInFrustum(corner, viewportMargin))
            {
                return true;
            }
        }

        return false;
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

    public Vector3Int GetChunkPosition(Vector3 position)
    {
        Vector3Int chunkPosition = new Vector3Int();

        chunkPosition.x = ((int)position.x / chunkPrefab.chunkSizeX) - ((position.x < 0) ? 1 : 0);
        chunkPosition.y = ((int)position.y / chunkPrefab.chunkSizeY) - ((position.y < 0) ? 1 : 0);
        chunkPosition.z = ((int)position.z / chunkPrefab.chunkSizeZ) - ((position.z < 0) ? 1 : 0);

        return chunkPosition;
    }

    private Vector3 GetChunkWorldCenter(Vector3Int p)
    {
        Vector3 world = GetWorldPositionFromChunkPosition(p);
        world.x += chunkPrefab.chunkSizeX / 2;
        world.y += chunkPrefab.chunkSizeY / 2;
        world.z += chunkPrefab.chunkSizeZ / 2;

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

        worldPosition.x = chunkPosition.x * chunkPrefab.chunkSizeX;
        worldPosition.y = chunkPosition.y * chunkPrefab.chunkSizeY;
        worldPosition.z = chunkPosition.z * chunkPrefab.chunkSizeZ;

        return worldPosition;
    }

    public Chunk GetChunkFromWorldPosition(Vector3 worldPosition)
    {
        Vector3Int chunkPosition = GetChunkPosition(worldPosition);

        if (chunkList.ContainsKey(chunkPosition))
        {
            return chunkList.Get(chunkPosition);
        }
        else
        {
            return null;
        }
    }

    public void UpdateChunk(Chunk chunk)
    {
        float priority = CalculateChunkPriority(chunk);
        chunkLoader.Build(chunk, priority);
    }

    private void OnDrawGizmosSelected()
    {

    }

    public Vector3Int GetPlayerChunk()
    {
        return GetChunkPosition(player.transform.position);
    }

    public bool IsPlayerChunkCached()
    {
        return chunkList.ContainsKey(GetPlayerChunk());
    }

    private void OnValidate()
    {
        if (generalRenderDistance < 0) generalRenderDistance = 0;
        if (moveThreshold <= 0) moveThreshold = 50f;
        if (distanceToDestroy <= 0) distanceToDestroy = 1f;
    }
}
