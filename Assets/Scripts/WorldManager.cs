using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public enum GeneratorType
    {
        SimplexTerrain,
        MultipleCombined
    }

    public int seed;

    public GeneratorType generator;
    public FieldGenerator[] generators = new FieldGenerator[Enum.GetNames(typeof(GeneratorType)).Length];

    public Transform player;

    public TextureAtlas atlas;

    public float rotationSpeed;
    public bool enableRotation = false;

    public Rect fpsOverlay;
    public Rect playerChunk;
    public Rect playerChunkCached;

    private ChunkManager chunkManager;
    private FrameRate frameRate;

    private void Awake()
    {
        chunkManager = GetComponent<ChunkManager>();
        frameRate = GetComponent<FrameRate>();
        
        if (generators.Length > 0)
        {
            chunkManager.chunkPrefab.fieldGenerator = generators[(int)generator];
            chunkManager.chunkPrefab.fieldGenerator.Initialize();
        }
    }

    private void OnGUI()
    {
        GUI.Label(fpsOverlay, string.Format("FPS: {0}", (int)frameRate.CurrentFrameRate));
        GUI.Label(playerChunk, string.Format("Player Chunk: {0} -> {1}", player.position, chunkManager.GetPlayerChunk()));
        GUI.Label(playerChunkCached, string.Format("Chunk Cached: {0}", chunkManager.IsPlayerChunkCached()));
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (enableRotation)
        {
            chunkManager.transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
    }
}
