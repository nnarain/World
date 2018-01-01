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
        Block
    }

    public enum FieldGeneratorType
    {
        Sine,
        PerlinHeight
    }

    public int chunkSizeX;
    public int chunkSizeY;
    public int chunkSizeZ;

    public MeshExtractorType extractorType = MeshExtractorType.Block;
    public FieldGeneratorType fieldType = FieldGeneratorType.Sine;
    public PerlinHeightMapGenerator.Config perlinConfig;

    private Mesh mesh;
    public Mesh Mesh { get { return mesh; } }

    private MeshExtractor mesher;
    private FieldGenerator fieldGenerator;

    private Field field;
    public Field Field { get { return field; } }

    private bool isLoaded = false;
    public bool IsLoaded { get { return isLoaded; } }

    private Chunk[] neighbors;

    // mesh data recieves from a builder thread
    private MeshData meshData;
    // flag indicatin that a chunk mesh needs to be updated
    private bool hasMeshData;
    // temporary object to store the chunks position since the transform cannot be used in a thread
    private Vector3 chunkPosition;

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

    }

    private void Update()
    {
        if (hasMeshData)
        {
            UpdateMesh(meshData);
        }
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

        isLoaded = true;
    }

    /// <summary>
    /// Build a mesh from the 3D grid.
    /// </summary>
    public void Build()
    {
        isLoaded = false;

        chunkPosition = transform.position;

        Thread builderThread = new Thread(() => {
            BuildMeshThread(OnMeshDataRecieve);
        });
        builderThread.Start();
    }

    private void BuildMeshThread(Action<MeshData> callback)
    {
        fieldGenerator.Generate(field, chunkPosition);
        mesher.Extract(field, callback);
    }

    /// <summary>
    /// Set the chunk neighbor in the given direction
    /// </summary>
    /// <param name="chunk"></param>
    /// <param name="direction"></param>
    public void SetNeighbor(Chunk chunk, Direction direction)
    {
        neighbors[direction.ToInt()] = chunk;
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
            case MeshExtractorType.Block:
                return new BlockMeshExtractor();
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
            case FieldGeneratorType.Sine:
                return new SineFieldGenerator();
            case FieldGeneratorType.PerlinHeight:
                return new PerlinHeightMapGenerator(perlinConfig);
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
