﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkLoader))]
public class ChunkManager : MonoBehaviour
{
    public GameObject player;

    public Chunk chunkPrefab;
    public int numChunks;

    public float forwardRenderDistance;
    public float generalRenderDistance;
    public float moveThreshold;
    [Range(0, 360)]
    public float rotationThreshold;
    public float distanceToInactive;
    public float distanceToDestroy;

    [Tooltip("Draw chunk debug info")]
    public bool drawDebug;

    private Vector3 lastPlayerPosition;
    private float lastPlayerRotation;

    private Dictionary<Vector3Int, Chunk> chunkList;
    private Camera playerCamera;
    private ChunkLoader chunkLoader;

    // Use this for initialization
    private void Start()
    {
        chunkLoader = GetComponent<ChunkLoader>();
        chunkList = new Dictionary<Vector3Int, Chunk>();
        playerCamera = player.GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePlayerPosition(player.transform.position);
        UpdatePlayerRotation();
    }

    private void UpdatePlayerPosition(Vector3 playerPosition)
    {
        // check if the player has moved the threshold distance.
        if ((playerPosition - lastPlayerPosition).magnitude > moveThreshold)
        {
            lastPlayerPosition = playerPosition;

            UpdateSurroundingChunks(playerPosition);
            //UpdateVisibleChunks(playerPosition);
            RemoveFarChunks(player.transform.position);
        }
    }

    public void UpdatePlayerRotation()
    {
        float playerRotation = player.transform.localRotation.eulerAngles.y;

        float rotateDiff = Mathf.Abs(playerRotation - lastPlayerRotation);

        if (rotateDiff >= rotationThreshold)
        {
            lastPlayerRotation = playerRotation;
        }
    }

    /// <summary>
    /// Use a breath-first search to queue surrounding chunks for loading
    /// </summary>
    /// <param name="playerPosition"></param>
    private void UpdateSurroundingChunks(Vector3 playerPosition)
    {
        // the player's chunk position
        Vector3Int playerChunkPosition = GetChunkPosition(playerPosition);

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
                // check that is doesn't already exist
                if (!chunkList.ContainsKey(neighbor))
                {
                    // get the world position of the chunk
                    Vector3 chunkPosition = GetChunkWorldCenter(neighbor);

                    // check the chunk is in the render distance of the player
                    var distanceFromPlayer = (playerPosition - chunkPosition).magnitude;

                    if (distanceFromPlayer <= generalRenderDistance)
                    {
                        queue.Enqueue(neighbor);
                    }
                    else if (playerCamera.IsPointInFrustum(chunkPosition))
                    {
                        // check if the chunk is in the camera's view frustum

                        // check if the chunk is in the forward render distance
                        if (distanceFromPlayer <= forwardRenderDistance)
                        {
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            explored.Add(p);

            // check if the current chunk is in the chunk list
            if (chunkList.ContainsKey(p))
            {
                // the chunk already exists, ensure it is enabled
                var chunk = chunkList[p];
                chunk.gameObject.SetActive(true);
            }
            else
            {
                // the chunk does not exist yet, create it
                var chunk = CreateChunk(p.x, p.z);
                chunkList.Add(p, chunk);

                // queue the chunk for loading
                chunkLoader.Enqueue(chunk);
            }
        }
    }

    private void UpdateVisibleChunks(Vector3 playerPosition)
    {
        // get the players chunk position
        Vector3Int playerChunkPosition = GetChunkPosition(playerPosition);

        // loop for the chunks surrounding the player
        for (int x = playerChunkPosition.x - numChunks; x < playerChunkPosition.x + numChunks; ++x)
        {
            for (int z = playerChunkPosition.z - numChunks; z < playerChunkPosition.z + numChunks; ++z)
            {
                Vector3Int chunkPosition = new Vector3Int(x, 0, z);

                // check if the chunk already exists in the dictionary
                if (chunkList.ContainsKey(chunkPosition))
                {
                    // if it does, ensure that it is enabled
                    var chunk = chunkList[chunkPosition];
                    chunk.gameObject.SetActive(true);
                }
                else
                {
                    // if the chunk does not exist yet, create it.
                    var chunk = CreateChunk(x, z);

                    chunkList.Add(chunkPosition, chunk);

                    chunkLoader.Enqueue(chunk);
                }
            }
        }
    }

    private void FillViewFrustum()
    {
        CameraFrustum frustum = playerCamera.GetFrustum();

        Vector3 corner1 = frustum.GetNearCorner(CameraFrustum.Corner.LB);
        Vector3 corner2 = frustum.GetNearCorner(CameraFrustum.Corner.RB);

        Vector3 corner1XZ = new Vector3(corner1.x, 0, corner1.z);
        Vector3 corner2XZ = new Vector3(corner2.x, 0, corner2.z);

        Vector3Int chunkPosition1 = GetChunkPosition(corner1XZ);
        Vector3Int chunkPosition2 = GetChunkPosition(corner2XZ);



        // search chunk neighbors and load the ones that lie within the camera frustum and are within a certain distance
    }

    private void RemoveFarChunks(Vector3 playerPosition)
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();

        // iterate over chunks in the dictionary
        foreach (var pair in chunkList)
        {
            var chunk = pair.Value;
            var chunkPosition = chunk.transform.position;

            // calculate distance between player and chunk
            var distanceToChunk = (playerPosition - chunkPosition).magnitude;

            // if the distance is greater than the distance to which the chunk should be inactive, but not removed
            if (distanceToChunk >= distanceToInactive)
            {
                // set the chunk to inactive
                chunk.gameObject.SetActive(false);
            }

            // if the distance is grater tan the distance to which the chunk shoould be destroyed
            if (distanceToChunk >= distanceToDestroy)
            {
                // set the chunk for removal from the list
                toRemove.Add(pair.Key);
                // and destroy the gamebobject
                Destroy(chunk);
            }
        }

        // remove destroyed chunks from the list
        foreach (var key in toRemove)
        {
            chunkList.Remove(key);
        }
    }

    private Chunk CreateChunk(int x, int z)
    {
        // if the chunk does not exist yet, create it.
        var chunk = Instantiate(chunkPrefab);
        chunk.transform.position = new Vector3(x * chunkPrefab.chunkSizeX, 0, z * chunkPrefab.chunkSizeZ);
        chunk.transform.SetParent(transform, false);

        // TODO: Set chunk neighbors

        return chunk;
    }

    private Vector3Int[] GetChunkNeighbors(Vector3Int p)
    {
        return new Vector3Int[]
        {
            new Vector3Int(p.x + 1, 0, p.z),
            new Vector3Int(p.x - 1, 0, p.z),
            new Vector3Int(p.x, 0, p.z + 1),
            new Vector3Int(p.x, 0, p.z - 1)
        };
    }

    private Vector3Int GetChunkPosition(Vector3 position)
    {
        Vector3Int chunkPosition = new Vector3Int();

        chunkPosition.x = (int)position.x / chunkPrefab.chunkSizeX;
        chunkPosition.y = (int)position.y / chunkPrefab.chunkSizeY;
        chunkPosition.z = (int)position.z / chunkPrefab.chunkSizeZ;

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

    private void OnDrawGizmosSelected()
    {
        if (drawDebug)
        {
        
        }
    }

    private void OnValidate()
    {
        if (numChunks <= 0) numChunks = 1;
        if (moveThreshold <= 0) moveThreshold = 1f;
        if (distanceToInactive <= 0) distanceToInactive = 100f;
        if (distanceToDestroy <= 0) distanceToDestroy = 1f;
    }
}
