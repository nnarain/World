﻿using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class Chunk : MonoBehaviour
{
    public enum MeshExtractorType
    {
        SimpleBlocks,
        GreedyBlocks
    }

    public enum ChunkState
    {
        Unloaded,
        LoadPending,
        Loading,
        Loaded,
        Building,
        Built
    }

    public int chunkSizeX;
    public int chunkSizeY;
    public int chunkSizeZ;

    public FieldGenerator fieldGenerator;
    public TextureAtlas blockAtlas;

    public MeshExtractorType extractorType = MeshExtractorType.SimpleBlocks;
    private IMeshExtractor mesher;


    private VoxelField field;
    public VoxelField Field { get { return field; } }

    private ChunkState state = ChunkState.Unloaded;
    public ChunkState State { get { return state; } }

    public bool IsLoaded { get { return state == ChunkState.Loaded; } }

    public Vector3Int Key
    {
        get
        {
            int kx = ((int)transform.position.x / chunkSizeX);
            int ky = ((int)transform.position.y / chunkSizeY);
            int kz = ((int)transform.position.z / chunkSizeZ);

            return new Vector3Int(kx, ky, kz);
        }
    }

    private Mesh mesh;
    public Mesh Mesh { get { return mesh; } }

    private MeshFilter meshFilter;
    private MeshCollider meshCollider;
    public bool ColliderEnabled { get { return meshCollider.enabled; } set { meshCollider.enabled = value; } }

    public Chunk[] neighbors = new Chunk[6];

    // mesh data recieves from a builder thread
    private MeshData[] meshData;
    // flag indicatin that a chunk mesh needs to be updated
    private bool hasMeshData;
    // temporary object to store the chunks position since the transform cannot be used in a thread
    private Vector3 chunkPosition;

    private Action<Chunk> onLoadCallback = null;
    private Action<Chunk> onBuildCallback = null;

    private void Awake()
    {
        mesh = new Mesh();
        mesh.subMeshCount = 2;

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;

        meshCollider = gameObject.AddComponent<MeshCollider>();
        meshCollider.enabled = true;

        mesher = CreateMeshExtractor(extractorType);

        field = new VoxelField(chunkSizeX, chunkSizeY, chunkSizeZ);
    }

    private void Start()
    {
    }

    private void Update()
    {
        if (hasMeshData)
        {
            UpdateMesh(meshData);
        }

        chunkPosition = transform.position;
    }

    /// <summary>
    /// Load field data for this chunk
    /// </summary>
    [ContextMenu("Reload")]
    public void Load()
    {
        state = ChunkState.Loading;
        fieldGenerator.Generate(field, chunkPosition);
        state = ChunkState.Loaded;

        if (onLoadCallback != null)
            onLoadCallback(this);
    }

    /// <summary>
    /// Build a mesh from the 3D grid.
    /// </summary>
    public void Build()
    {
        state = ChunkState.Building;
        mesher.Extract(this, OnMeshDataRecieve);

        if (onBuildCallback != null)
            onBuildCallback(this);
    }

    private void OnMeshDataRecieve(MeshData[] data)
    {
        meshData = data;
        hasMeshData = true;
    }

    /// <summary>
    /// Recieve the calculated mesh data and update the mesh
    /// </summary>
    /// <param name="data"></param>
    private void UpdateMesh(MeshData[] data)
    {
        //Debug.Log(string.Format("{0} updating mesh", GetHashCode()));
        List<Vector3> meshVertices = new List<Vector3>();
        List<Vector2> meshUVs = new List<Vector2>();
        List<Color> meshColors = new List<Color>();
        List<int> subMeshOffsets = new List<int>();

        ClearStaleData();

        foreach (var d in data)
        {
            subMeshOffsets.Add(meshVertices.Count);
            meshVertices.AddRange(d.vertices);
            meshUVs.AddRange(d.uvs);
            meshColors.AddRange(d.colors);
        }

        // add vertex data
        mesh.vertices = meshVertices.ToArray();
        mesh.uv = meshUVs.ToArray();
        mesh.colors = meshColors.ToArray();

        for (int i = 0; i < data.Length; ++i)
        {
            mesh.SetTriangles(data[i].elements, i, false, subMeshOffsets[i]);
        }

        // recalculate vertex normals
        mesh.RecalculateNormals();
        // recalculate bounds after meshes are added
        mesh.RecalculateBounds();

        // add the collider
        meshCollider.sharedMesh = mesh;

        state = ChunkState.Built;

        hasMeshData = false;
    }

    private void ClearStaleData()
    {
        mesh.Clear();
        mesh = new Mesh();
        mesh.subMeshCount = 2;
        meshFilter.mesh = mesh;

        field.Clear();
    }

    public void SetOnLoadCallback(Action<Chunk> callback)
    {
        onLoadCallback = callback;
    }

    public void SetOnBuildCallback(Action<Chunk> callback)
    {
        onBuildCallback = callback;
    }

    public void Translate(Vector3 t)
    {
        chunkPosition += t;
        transform.position += t;
    }

    public void SetPosition(Vector3 position)
    {
        chunkPosition = position;
        transform.position = position;
    }

    /// <summary>
    /// Set the chunk neighbor in the given direction
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="direction"></param>
    public void SetNeighbor(Chunk chunk, Direction direction)
    {
        neighbors[direction.ToInt()] = chunk;
        
        if (chunk != null)
        {
            chunk.neighbors[direction.Opposite().ToInt()] = this;
        }
    }

    public void ClearNeighbors()
    {
        for (int i = 0; i < neighbors.Length; ++i)
        {
            neighbors[i] = null;
        }
    }

    /// <summary>
    /// Get field data fro mthe chunk, accounting for overflow into adjacent chunks
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public Voxel GetField(int x, int y, int z)
    {
        if (x < 0)
        {
            return GetNeighborField(Direction.Left, x + chunkSizeX, y, z);
        }
        else if (x >= chunkSizeX)
        {
            return GetNeighborField(Direction.Right, x - chunkSizeX, y, z);
        }

        if (y < 0)
        {
        //    return GetNeighborField(Direction.Bottom, x, y + chunkSizeY, z);
        }
        else if (y >= chunkSizeY)
        {
        //    return GetNeighborField(Direction.Top, x, y - chunkSizeY, z);
        }

        if (z < 0)
        {
            return GetNeighborField(Direction.Near, x, y, z + chunkSizeZ);
        }
        else if (z >= chunkSizeZ)
        {
            return GetNeighborField(Direction.Far, x, y, z - chunkSizeZ);
        }

        return field.Get(x, y, z);
    }

    private Voxel GetNeighborField(Direction d, int x, int y, int z)
    {
        var chunk = GetNeighbor(d);

        if (chunk != null)
        {
            return chunk.GetField(x, y, z);
        }
        else
        {
            return Voxel.none;
        }
    }

    /// <summary>
    /// Get the chunk's neighbor for the given direction
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public Chunk GetNeighbor(Direction direction)
    {
        return neighbors[direction.ToInt()];
    }

    public Color GetVoxelColor(byte type)
    {
        return fieldGenerator.GetVoxelColor(type);
    }

    public TextureAtlas.BlockFaces GetFaces(byte t)
    {
        return blockAtlas.GetBlockFaces((Blocks.Type)t);
    }

    public Vector2[] GetFaceUVs(byte t, Direction d)
    {
        return GetFaces(t).GetUVs(d);
    }

    /// <summary>
    /// Get the mesh extractor
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private IMeshExtractor CreateMeshExtractor(MeshExtractorType type)
    {
        switch (type)
        {
            case MeshExtractorType.SimpleBlocks:
                return new BlockMeshExtractor();
            case MeshExtractorType.GreedyBlocks:
                return new GreedyMeshExtractor();
            default:
                return null;
        }
    }

    public void MarkLoadPending()
    {
        // Can only go from unloaded to loading pending
        if (state == ChunkState.Unloaded)
        {
            state = ChunkState.LoadPending;
        }
        else
        {
            // throw exception?
        }
    }

    [ContextMenu("Grid Value")]
    public void DisplayGridPosition()
    {
        Debug.Log(string.Format("{0} -> {1}", transform.position, Key));
    }

    private void OnValidate()
    {
        if (chunkSizeX == 0) chunkSizeX = 1;
        if (chunkSizeY == 0) chunkSizeY = 1;
        if (chunkSizeZ == 0) chunkSizeZ = 1;
    }

    public override string ToString()
    {
        return base.ToString();
    }
}
