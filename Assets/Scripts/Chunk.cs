using System;
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

    public enum FieldGeneratorType
    {
        Fill,
        Sine,
        PerlinHeight
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

    public MeshExtractorType extractorType = MeshExtractorType.SimpleBlocks;
    public FieldGeneratorType fieldType = FieldGeneratorType.Sine;
    public PerlinHeightMapGenerator.Config perlinConfig;

    private Mesh mesh;
    public Mesh Mesh { get { return mesh; } }

    private MeshExtractor mesher;
    private FieldGenerator fieldGenerator;

    private Field field;
    public Field Field { get { return field; } }

    private ChunkState state = ChunkState.Unloaded;
    public ChunkState State { get { return state; } }

    public bool IsLoaded { get { return state == ChunkState.Loaded; } }

    private Chunk[] neighbors;

    // mesh data recieves from a builder thread
    private MeshData meshData;
    // flag indicatin that a chunk mesh needs to be updated
    private bool hasMeshData;
    // temporary object to store the chunks position since the transform cannot be used in a thread
    private Vector3 chunkPosition;

    private Action<Chunk> onLoadCallback = null;
    private Action<Chunk> onBuildCallback = null;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesher = CreateMeshExtractor(extractorType);

        field = new Field(chunkSizeX, chunkSizeY, chunkSizeZ);
        fieldGenerator = CreateFieldGenerator(fieldType);

        neighbors = new Chunk[6];
    }

    private void Start()
    {
        chunkPosition = transform.position;
    }

    private void Update()
    {
        if (hasMeshData)
        {
            UpdateMesh(meshData);
        }
    }

    /// <summary>
    /// Load field data for this chunk
    /// </summary>
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

    private void OnMeshDataRecieve(MeshData data)
    {
        meshData = data;
        hasMeshData = true;
    }

    /// <summary>
    /// Recieve the calculated mesh data and update the mesh
    /// </summary>
    /// <param name="data"></param>
    private void UpdateMesh(MeshData data)
    {
        mesh.vertices = data.vertices.ToArray();
        mesh.triangles = data.triangles.ToArray();
        mesh.RecalculateNormals();

        state = ChunkState.Built;

        hasMeshData = false;
    }

    public void SetOnLoadCallback(Action<Chunk> callback)
    {
        onLoadCallback = callback;
    }

    public void SetOnBuildCallback(Action<Chunk> callback)
    {
        onBuildCallback = callback;
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
        // TODO set opposite neighbor
    }

    /// <summary>
    /// Get field data fro mthe chunk, accounting for overflow into adjacent chunks
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    public float GetField(int x, int y, int z)
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
            return GetNeighborField(Direction.Bottom, x, y + chunkSizeY, z);
        }
        else if (y >= chunkSizeY)
        {
            return GetNeighborField(Direction.Top, x, y - chunkSizeY, z);
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

    private float GetNeighborField(Direction d, int x, int y, int z)
    {
        var chunk = GetNeighbor(d);

        if (chunk != null)
        {
            return chunk.GetField(x, y, z);
        }
        else
        {
            return -1;
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

    /// <summary>
    /// Get the mesh extractor
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private MeshExtractor CreateMeshExtractor(MeshExtractorType type)
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

    /// <summary>
    ///  Get the firld generator
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private FieldGenerator CreateFieldGenerator(FieldGeneratorType type)
    {
        switch(type)
        {
            case FieldGeneratorType.Fill:
                return new FillFieldGenerator();
            case FieldGeneratorType.Sine:
                return new SineFieldGenerator();
            case FieldGeneratorType.PerlinHeight:
                return new PerlinHeightMapGenerator(perlinConfig);
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

    private void OnValidate()
    {
        if (chunkSizeX == 0) chunkSizeX = 1;
        if (chunkSizeY == 0) chunkSizeY = 1;
        if (chunkSizeZ == 0) chunkSizeZ = 1;
    }
}
