using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate a map based on the Robert Whittaker 'moisture vs temperature'
/// </summary>
public class ElevationMoistureFieldGenerator : FieldGenerator
{
    [System.Serializable]
    public struct Config
    {
        public int seed;
        public float scale;
        public int octaves;
        [Range(0, 1)]
        public float persistance;
        [Range(1, 2)]
        public float lacunarity;
        public AnimationCurve heightCurve;
    }

    private int seed;
    private float scale;
    private int octaves;
    private float persistance;
    private float lacunarity;
    public AnimationCurve heightCurve;

    public ElevationMoistureFieldGenerator(Config config)
    {
        seed = config.seed;
        scale = config.scale;
        octaves = config.octaves;
        persistance = config.persistance;
        lacunarity = config.lacunarity;
        heightCurve = config.heightCurve;
    }

    void FieldGenerator.Generate(VoxelField field, Vector3 position)
    {
        Vector2 samplePosition = new Vector2(position.x, position.z);
        float[,] heightMap = PerlinNoise.Generate(field.X, field.Z, seed, scale, octaves, persistance, lacunarity, samplePosition);
    }
}
