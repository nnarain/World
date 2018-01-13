﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public enum GeneratorType
    {
        PerlinHeight,
        ElevationAndMoisture
    }

    public int seed;

    public GeneratorType generator;
    public FieldGenerator[] generators = new FieldGenerator[Enum.GetNames(typeof(GeneratorType)).Length];

    private ChunkManager chunkManager;

    private void Awake()
    {
        chunkManager = GetComponent<ChunkManager>();
        
        if (generators.Length > 0)
        {
            chunkManager.chunkPrefab.fieldGenerator = generators[(int)generator];
        }
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
