using System.Collections;
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

    private Mesh mesh;
    private MeshExtractor mesher;
    private FieldGenerator fieldGenerator;

    private Field field;
    public Field Field { get { return field; } }

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        mesher = CreateMeshExtractor(extractorType);

        field = new Field(chunkSizeX, chunkSizeY, chunkSizeZ);
        fieldGenerator = CreateFieldGenerator(fieldType);
    }

    private void Start()
    {

    }

    /// <summary>
    /// Build a mesh from the 3D grid.
    /// </summary>
    public void Build()
    {
        fieldGenerator.Generate(field, transform);
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

    private FieldGenerator CreateFieldGenerator(FieldGeneratorType type)
    {
        switch(type)
        {
            case FieldGeneratorType.Sine:
                return new SineFieldGenerator();
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
