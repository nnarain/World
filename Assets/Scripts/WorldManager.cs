using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Chunk chunkPrefab;

    // Use this for initialization
    void Start()
    {
        Chunk chunk = Instantiate<Chunk>(chunkPrefab);
        chunk.transform.SetParent(transform, false);

        chunk.Build();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
