using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Chunk chunkPrefab;

    public int numChunks;

    // Use this for initialization
    void Start()
    {
        Generate();
    }

    public void Generate()
    {
        DestroyChunks();

        int chunkSizeX = chunkPrefab.chunkSizeX;
        int chunkSizeZ = chunkPrefab.chunkSizeZ;

        for (int x = -numChunks; x < numChunks; ++x)
        {
            for (int z = -numChunks; z < numChunks; ++z)
            {
                Chunk chunk = Instantiate(chunkPrefab);
                chunk.transform.SetParent(transform, false);
                chunk.transform.position = new Vector3(x * chunkSizeX, 0, z * chunkSizeZ);

                chunk.Build();
            }
        }
    }

    public void DestroyChunks()
    {
        while (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
