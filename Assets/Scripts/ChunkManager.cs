﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkLoader))]
public class ChunkManager : MonoBehaviour
{
    public GameObject player;

    public Chunk chunkPrefab;
    public int numChunks;

    [Range(0, 1)]
    public float renderDistance;
    public float moveThreshold;
    public float distanceToInactive;
    public float distanceToDestroy;

    private Vector3 lastPlayerPosition;

    private Dictionary<Vector3Int, Chunk> chunkList;
    private Camera camera;
    private ChunkLoader chunkLoader;

    // Use this for initialization
    private void Start()
    {
        chunkLoader = GetComponent<ChunkLoader>();
        chunkList = new Dictionary<Vector3Int, Chunk>();
        camera = player.GetComponentInChildren<Camera>();
    }

    public void UpdatePlayerPosition(Vector3 playerPosition)
    {
        // check if the player has moved the threshold distance.
        if ((playerPosition - lastPlayerPosition).magnitude > moveThreshold)
        {
            lastPlayerPosition = playerPosition;

            UpdateVisibleChunks(playerPosition);
        }
    }

    // Update is called once per frame
    private void Update()
    {
        UpdatePlayerPosition(player.transform.position);
        RemoveFarChunks(player.transform.position);
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
                    var chunk = Instantiate(chunkPrefab);
                    chunk.transform.position = new Vector3(x * chunkPrefab.chunkSizeX, 0, z * chunkPrefab.chunkSizeZ);
                    chunk.transform.SetParent(transform, false);

                    chunkList.Add(chunkPosition, chunk);

                    chunkLoader.Enqueue(chunk);
                }
            }
        }
    }

    private void FillViewFrustum()
    {

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
                Destroy(chunk.gameObject);
            }
        }

        // remove destroyed chunks from the list
        foreach (var key in toRemove)
        {
            chunkList.Remove(key);
        }
    }

    private Vector3Int GetChunkPosition(Vector3 position)
    {
        Vector3Int chunkPosition = new Vector3Int();

        chunkPosition.x = (int)position.x / chunkPrefab.chunkSizeX;
        chunkPosition.y = (int)position.y / chunkPrefab.chunkSizeY;
        chunkPosition.z = (int)position.z / chunkPrefab.chunkSizeZ;

        return chunkPosition;
    }

    private Vector3[] GetCameraFrustumCorners(Camera camera)
    {
        Vector3[] corners = new Vector3[4];


        return corners;
    }

    private void OnValidate()
    {
        if (numChunks <= 0) numChunks = 1;
        if (moveThreshold <= 0) moveThreshold = 1f;
        if (distanceToInactive <= 0) distanceToInactive = 100f;
        if (distanceToDestroy <= 0) distanceToDestroy = 1f;
    }
}
