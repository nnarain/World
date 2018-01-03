using System.Collections;
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
    [Range(0, 360)]
    public float rotationThreshold;
    public float distanceToInactive;
    public float distanceToDestroy;

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
            //    FillViewFrustum();
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

        //    FillViewFrustum();
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

        // the opposite near and far plane corners
        Vector3 corner1 = frustum.GetNearCorner(CameraFrustum.Corner.RB);
        Vector3 corner2 = frustum.GetFarCorner(CameraFrustum.Corner.LB);

        // calculate a point somewhere between the corners
        Vector3 mid = (corner2 - corner1) * renderDistance;

        // XZ plane vectors
        Vector3 corner1XZ = new Vector3(corner1.x, 0, corner1.z);
        Vector3 corner2XZ = new Vector3(mid.x, 0, mid.z);

        Vector3Int chunkPosition1 = GetChunkPosition(corner1XZ);
        Vector3Int chunkPosition2 = GetChunkPosition(corner2XZ);

        int minX = System.Math.Min(chunkPosition1.x, chunkPosition2.x);
        int maxX = System.Math.Max(chunkPosition1.x, chunkPosition2.x);
        int minZ = System.Math.Min(chunkPosition1.z, chunkPosition2.z);
        int maxZ = System.Math.Max(chunkPosition1.z, chunkPosition2.z);

        Debug.Log(string.Format("X: ({0}, {1}), Z: ({2}, {3})", minX, maxX, minZ, maxZ));

        for (int x = minX; x <= maxX; ++x)
        {
            for (int z = minZ; z <= maxZ; ++z)
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

        return chunk;
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
