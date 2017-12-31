using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkManager : MonoBehaviour
{
    public Transform player;

    public Chunk chunkPrefab;
    public int numChunks;

    public float moveThreshold;
    public float distanceToInactive;
    public float distanceToDestroy;

    private Vector3 lastPlayerPosition;

    private Dictionary<Vector3Int, Chunk> chunkList;

    // Use this for initialization
    private void Start()
    {
        chunkList = new Dictionary<Vector3Int, Chunk>();
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
        UpdatePlayerPosition(player.position);
        RemoveFarChunks(player.position);
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

                    chunk.Build();
                }
            }
        }
    }

    private void RemoveFarChunks(Vector3 playerPosition)
    {
        foreach (var pair in chunkList)
        {
            var chunk = pair.Value;
            var chunkPosition = chunk.transform.position;

            var distanceToChunk = (playerPosition - chunkPosition).magnitude;

            if (distanceToChunk >= distanceToInactive)
            {
                chunk.gameObject.SetActive(false);
            }
            else if (distanceToChunk >= distanceToDestroy)
            {
                DestroyObject(chunk);
            }
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

    private void OnValidate()
    {
        if (numChunks <= 0) numChunks = 1;
        if (moveThreshold <= 0) moveThreshold = 1f;
        if (distanceToInactive <= 0) distanceToInactive = 100f;
        if (distanceToDestroy <= 0) distanceToDestroy = 1f;
    }
}
