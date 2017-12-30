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

    private Chunk[] neighbors;

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

    /// <summary>
    /// Build a mesh from the 3D grid.
    /// </summary>
    public void Build()
    {
        fieldGenerator.Generate(this);
        mesher.Extract(mesh, field);
    }

    public void SetNeighbor(Chunk chunk, Direction direction)
    {
        neighbors[direction.ToInt()] = chunk;
    }

    public Chunk GetNeighbor(Direction direction)
    {
        return neighbors[direction.ToInt()];
    }

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
