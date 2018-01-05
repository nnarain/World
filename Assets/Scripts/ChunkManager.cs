using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ChunkLoader))]
public class ChunkManager : MonoBehaviour
{
    public GameObject player;

    public Chunk chunkPrefab;

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

            UpdateVisibleChunks(playerPosition);
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
            UpdateVisibleChunks(player.transform.position);
        }
    }

    /// <summary>
    /// Use a breath-first search to queue surrounding chunks for loading
    /// </summary>
    /// <param name="playerPosition"></param>
    private void UpdateVisibleChunks(Vector3 playerPosition)
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
                // get the world position of the chunk
                Vector3 chunkPosition = GetChunkWorldCenter(neighbor);

                // check the chunk is in the render distance of the player
                var distanceFromPlayer = (playerPosition - chunkPosition).magnitude;

                if (distanceFromPlayer <= generalRenderDistance)
                {
                    queue.Enqueue(neighbor);
                }
                else if (IsChunkInFrustum(chunkPosition))
                {
                    // check if the chunk is in the camera's view frustum

                    // check if the chunk is in the forward render distance
                    if (distanceFromPlayer <= forwardRenderDistance)
                    {
                        queue.Enqueue(neighbor);
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
        chunk.SetPosition(new Vector3(x * chunkPrefab.chunkSizeX, 0, z * chunkPrefab.chunkSizeZ));
        chunk.transform.SetParent(transform, false);

        // TODO: Set chunk neighbors

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
            if (playerCamera.IsPointInFrustum(corner))
            {
                return true;
            }
        }

        return false;
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
        if (moveThreshold <= 0) moveThreshold = 1f;
        if (distanceToInactive <= 0) distanceToInactive = 100f;
        if (distanceToDestroy <= 0) distanceToDestroy = 1f;
    }
}
