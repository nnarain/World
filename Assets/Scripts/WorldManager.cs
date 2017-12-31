using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public Transform player;

    private ChunkManager chunkManager;

    private void Awake()
    {
        chunkManager = GetComponent<ChunkManager>();
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    //    chunkManager.UpdatePlayerPosition(player.position);
    }
}
