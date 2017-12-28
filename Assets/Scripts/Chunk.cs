﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public enum MeshExtractorType
    {
        Block,
        MarchingCubes
    }

    public int chunkSizeX;
    public int chunkSizeY;
    public int chunkSizeZ;

    public MeshExtractorType extractorType = MeshExtractorType.Block;

    private Mesh mesh;
    private MeshExtractor mesher;

    private Field field;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesher = CreateMeshExtractor(extractorType);

        field = new Field(chunkSizeX, chunkSizeY, chunkSizeZ);

        for (int x = 0; x < field.X; ++x)
        {
            for (int y = 0; y < field.Y; ++y)
            {
                for (int z = 0; z < field.Z; ++z)
                {
                    field.Set(x, y, z, 1);
                }
            }
        }
    }

    private void Start()
    {

    }

    /// <summary>
    /// Build a mesh from the 3D grid.
    /// </summary>
    public void Build()
    {
        mesher.Extract(mesh, field);
    }

    private MeshExtractor CreateMeshExtractor(MeshExtractorType type)
    {
        switch (type)
        {
            case MeshExtractorType.Block:
                return new BlockMeshExtractor();
            case MeshExtractorType.MarchingCubes:
                return null;
            default:
                return null;
        }
    }

    private void OnValidate()
    {
        if (chunkSizeX == 0) chunkSizeX = 1;
        if (chunkSizeY == 0) chunkSizeY = 1;
        if (chunkSizeZ == 0) chunkSizeZ = 1;
    }
}
