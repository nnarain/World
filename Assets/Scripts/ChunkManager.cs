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

    private Camera playerCamera;
    private ChunkLoader chunkLoader;

    private Vector3Int lastPlayerPosition;

    private Chunk[,] graph;
    public float viewDisance;

    private Chunk head, tail;

    private SafeQueue<Chunk> activateQueue;
    private Queue<Chunk> reserveChunksQueue;

    // Use this for initialization
    private void Start()
    {
        chunkLoader = GetComponent<ChunkLoader>();
        playerCamera = player.GetComponentInChildren<Camera>();

        chunkLoader.SetLoadMode(ChunkLoader.LoadMode.Reserved);

        lastPlayerPosition = GetChunkPosition(player.transform.position);

        activateQueue = new SafeQueue<Chunk>();
        reserveChunksQueue = new Queue<Chunk>();

        LoadInitialChunks();
    }

    // Update is called once per frame
    private void Update()
    {
        HandlePlayerMovement();
        HandleChunkActivation();
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

    private void HandleLoadNewChunks(Vector3Int dc)
    {
        var dx = dc.x;
        var dz = dc.z;

        if (dx == 1)
        {
            MoveChunks(Direction.Right);
        }
        else if (dx == -1)
        {
            MoveChunks(Direction.Left);
        }

        if (dz == 1)
        {
            MoveChunks(Direction.Far);
        }
        else if (dz == -1)
        {
            MoveChunks(Direction.Near);
        }
    }

    private void MoveChunks(Direction d)
    {
        if (d == Direction.Right)
        {
            MoveChunksSub(d, head, Direction.Near, tail, Direction.Far);
        }
        else if (d == Direction.Left)
        {
            MoveChunksSub(d, tail, Direction.Far, head, Direction.Near);
        }
        else if (d == Direction.Far)
        {
            MoveChunksSub(d, tail, Direction.Left, head, Direction.Right);
        }
        else if (d == Direction.Near)
        {
            MoveChunksSub(d, head, Direction.Right, tail, Direction.Left);
        }
    }

    private void MoveChunksSub(Direction d, Chunk removeNode, Direction removeDirection, Chunk linkNode, Direction linkDirection)
    {
        Chunk newHead = null;
        Chunk newTail = null;

        if (d == Direction.Right || d == Direction.Near)
        {
            newHead = removeNode.GetNeighbor(d);
        }

        if (d == Direction.Far || d == Direction.Left)
        {
            newTail = removeNode.GetNeighbor(d);
        }

        var removeSet = GetLinkedChunks(removeNode, removeDirection);
        EnqueueReserveChunks(removeSet);

        var linkSet = GetLinkedChunks(linkNode, linkDirection);
        var newChunks = new Chunk[linkSet.Length];

        for (int i = 0; i < linkSet.Length; ++i)
        {
            var adjacentNode = linkSet[i];
            //Debug.Log(string.Format("dequeue reserved chunks: {0}", reserveChunksQueue.Count));
            var chunk = reserveChunksQueue.Dequeue();

            // position the chunk in the grid
            //if (adjacentNode == null) Debug.Log("adjacent node is null");
            var linkNodePosition = adjacentNode.transform.position;
            var moveVector = GetMoveDirection(d) * chunkPrefab.chunkSizeX;

            chunk.SetPosition(linkNodePosition + moveVector);

            // link chunk neighbors
            chunk.SetNeighbor(adjacentNode, d.Opposite());

            if (i > 0)
            {
                chunk.SetNeighbor(newChunks[i - 1], linkDirection.Opposite());
            }

            // THE TAIL IS WRONG... AHHHHH

            newChunks[i] = chunk;
            chunkLoader.Load(chunk);
        }

        if (d == Direction.Far || d == Direction.Left)
        {
            newHead = newChunks[0];
        }

        if (d == Direction.Right || d == Direction.Near)
        {
            newTail = newChunks[0];
        }

        head = newHead;
        tail = newTail;
    }

    private void EnqueueReserveChunks(Chunk[] nodes)
    {
        foreach (var chunk in nodes)
        {
            if (chunk == null) continue;

            chunk.ClearNeighbors();
            chunk.gameObject.SetActive(false);

            reserveChunksQueue.Enqueue(chunk);
        }
    }

    private Vector3 GetMoveDirection(Direction d)
    {
        switch (d)
        {
            case Direction.Left:
                return new Vector3(-1, 0, 0);
            case Direction.Right:
                return new Vector3(1, 0, 0);
            case Direction.Near:
                return new Vector3(0, 0, -1);
            case Direction.Far:
                return new Vector3(0, 0, 1);
            default:
                return new Vector3(0, 0, 0);
        }
    }

    private Chunk[] GetLinkedChunks(Chunk node, Direction d)
    {
        Chunk[] linkedChunks = new Chunk[graph.GetLength(0)];

        var addedCount = 0;
        for (int i = 0; i < graph.GetLength(0) && node != null; ++i)
        {
            linkedChunks[i] = node;
            node = node.GetNeighbor(d);
            addedCount += 1;
        }

        return linkedChunks;
    }

    private void LoadInitialChunks()
    {
        int d = ((int)viewDisance * 2) / chunkPrefab.chunkSizeX;

        // allocate space for all visible chunks surrounding the player
        graph = new Chunk[d, d];

        // get the player chunk position
        var chunkPosition = GetChunkPosition(lastPlayerPosition);

        var offsetX = chunkPosition.x - (d / 2);
        var offsetZ = chunkPosition.z - (d / 2);

        // create chunks
        for (int x = 0; x < d; ++x)
        {
            for (int z = 0; z < d; ++z)
            {
                var chunk = CreateChunk(x + offsetX, z + offsetZ);
                graph[x, z] = chunk;

                // set chunk neighbors
                if (x > 0)
                {
                    chunk.SetNeighbor(graph[x - 1, z], Direction.Left);
                }
                if (z > 0)
                {
                    chunk.SetNeighbor(graph[x, z - 1], Direction.Near);
                }

                // queue the chunk for loading
                chunkLoader.Load(chunk);
            }
        }

        head = graph[0, d - 1];
        tail = graph[d - 1, 0];

        // Load reserve chunks
        var numReserveChunks = d * 4;
        for (int i = 0; i < numReserveChunks; ++i)
        {
            var chunk = CreateChunk(0, 0);
            reserveChunksQueue.Enqueue(chunk);
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
