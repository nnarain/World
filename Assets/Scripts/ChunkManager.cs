using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[RequireComponent(typeof(ChunkLoader))]
public class ChunkManager : MonoBehaviour
{
    public GameObject player;

    public Chunk chunkPrefab;

    public float viewDisance;
    public float removeChunkTime;
    public float removeDistanceThreshold;

    private Camera playerCamera;
    private ChunkLoader chunkLoader;

    private Vector3Int lastPlayerPosition;

    private Dictionary<Vector3Int, Chunk> chunkMap;
    

    private Vector3Int head, tail;
    private int viewChunkLength;

    private float removeChunksTimer = 0;

    private SafeQueue<Chunk> activateQueue;

    // Use this for initialization
    private void Start()
    {
        chunkLoader = GetComponent<ChunkLoader>();
        playerCamera = player.GetComponentInChildren<Camera>();

        chunkLoader.SetLoadMode(ChunkLoader.LoadMode.Reserved);

        lastPlayerPosition = GetChunkPosition(player.transform.position);

        activateQueue = new SafeQueue<Chunk>();

        chunkMap = new Dictionary<Vector3Int, Chunk>();
        LoadInitialChunks();
    }

    // Update is called once per frame
    private void Update()
    {
        HandlePlayerMovement();
        HandleChunkActivation();
        HandleChunkRemoval();
    }

    private void HandleChunkActivation()
    {
        while (!activateQueue.Empty)
        {
            var chunk = activateQueue.Dequeue();
            chunk.gameObject.SetActive(true);
        }
    }

    private void HandlePlayerMovement()
    {
        // check if the player has moved to a new chunk
        var currentPosition = GetChunkPosition(player.transform.position);

        if (currentPosition.x != lastPlayerPosition.x || currentPosition.z != lastPlayerPosition.z)
        {
            // get the difference in the chunk movement
            var dc = currentPosition - lastPlayerPosition;
            HandleLoadNewChunks(dc);

            // set the last chunk position to the current chunk position
            lastPlayerPosition = currentPosition;
        }
    }

    private void HandleChunkRemoval()
    {
        removeChunksTimer += Time.deltaTime;
        if (removeChunksTimer >= removeChunkTime)
        {
            RemoveChunks();
        }
    }

    private void RemoveChunks()
    {
        List<Vector3Int> toRemove = new List<Vector3Int>();

        foreach(var pair in chunkMap)
        {
            var chunk = pair.Value;
            var chunkPosition = chunk.transform.position;

            var playerPosition = player.transform.position;

            var distance = Mathf.Abs((playerPosition - chunkPosition).magnitude);

            if (distance > removeDistanceThreshold)
            {
                toRemove.Add(pair.Key);
                Destroy(chunk);
            }
        }

        foreach (var key in toRemove)
        {
            chunkMap.Remove(key);
        }
    }

    private void HandleLoadNewChunks(Vector3Int dc)
    {
        var dx = dc.x;
        var dz = dc.z;

        if (dx == 1)
        {
            AllocateNewChunks(new Vector3Int(tail.x + dx, 0, tail.z), new Vector3Int(0, 0, 1));
        }
        else if (dx == -1)
        {
            AllocateNewChunks(new Vector3Int(head.x + dx, 0, head.z), new Vector3Int(0, 0, -1));
        }

        if (dz == 1)
        {
            AllocateNewChunks(new Vector3Int(head.x, 0, head.z + dz), new Vector3Int(1, 0, 0));
        }
        else if (dz == -1)
        {
            AllocateNewChunks(new Vector3Int(tail.x, 0, tail.z + dz), new Vector3Int(-1, 0, 0));
        }

        head += dc;
        tail += dc;
    }

    private void AllocateNewChunks(Vector3Int start, Vector3Int d)
    {
        Vector3Int position = start;

        for (int i = 0; i < viewChunkLength; ++i)
        {
            position += d;
            Vector3Int key = new Vector3Int(position.x, 0, position.z);

            // only create a new chunk if it does not exist in the map
            if (!chunkMap.ContainsKey(key))
            {
                var chunk = CreateChunk(key.x, key.z);
                chunkMap[key] = chunk;

                chunkLoader.Load(chunk);
            }

            
        }
    }

    private void LoadInitialChunks()
    {
        int d = ((int)viewDisance * 2) / chunkPrefab.chunkSizeX;
        viewChunkLength = d;

        // get the player chunk position
        var chunkPosition = GetChunkPosition(lastPlayerPosition);

        var offsetX = chunkPosition.x - (d / 2);
        var offsetZ = chunkPosition.z - (d / 2);

        head = new Vector3Int(offsetX, 0, offsetZ + (d - 1));
        tail = new Vector3Int(offsetX + (d - 1), 0, offsetZ);

        // create chunks
        for (int x = 0; x < d; ++x)
        {
            for (int z = 0; z < d; ++z)
            {
                Vector3Int key = new Vector3Int(x + offsetX, 0, z + offsetZ);
                var chunk = CreateChunk(key.x, key.z);

                chunkMap.Add(key, chunk);
                
                // set chunk neighbors
                if (x > 0)
                {
                    key.x -= 1;
                    var n = chunkMap[key];
                    chunk.SetNeighbor(n, Direction.Left);
                }
                if (z > 0)
                {
                    key.z -= 1;
                    var n = chunkMap[key];
                    chunk.SetNeighbor(n, Direction.Near);
                }

                // queue the chunk for loading
                chunkLoader.Load(chunk);
            }
        }
    }

    private Chunk GetNeighbor(int x, int z)
    {
        Vector3Int key = new Vector3Int(x, 0, z);

        if (chunkMap.ContainsKey(key))
        {
            return chunkMap[key];
        }
        else
        {
            return null;
        }
    }

    private void OnChunkLoad(Chunk chunk)
    {
        chunk.Build();
    }

    private void OnChunkBuild(Chunk chunk)
    {
        activateQueue.Enqueue(chunk);
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
        chunk.gameObject.SetActive(false);

        return chunk;
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
        return null;
    }

    public void UpdateChunk(Chunk chunk)
    {
        chunkLoader.Build(chunk, 0);
    }

    public Vector3Int GetPlayerChunk()
    {
        return GetChunkPosition(player.transform.position);
    }

    public bool IsPlayerChunkCached()
    {
        return false;
    }

    private void OnValidate()
    {

    }
}
