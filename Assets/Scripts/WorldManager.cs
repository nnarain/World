﻿using System;
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

    public TextureAtlas atlas;

    public float rotationSpeed;
    public bool enableRotation = false;

    public Rect fpsOverlay;

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
