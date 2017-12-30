using System;
using System.Collections.Generic;
using UnityEngine;

public class PerlinHeightMapGenerator : FieldGenerator
{
    [System.Serializable]
    public struct Config
    {
        public int seed;
        public float scale;
        public int octaves;
        [Range(0,1)]
        public float persistance;
        [Range(1, 2)]
        public float lacunarity;
        public Perlin.NormalizeMode normalizeMode;
    }

    private int seed;
    private float scale;
    private int octaves;
    private float persistance;
    private float lacunarity;
    private Perlin.NormalizeMode normalizeMode;

    public PerlinHeightMapGenerator(Config config)
    {
        seed = config.seed;
        scale = config.scale;
        octaves = config.octaves;
        persistance = config.persistance;
        lacunarity = config.lacunarity;
        normalizeMode = config.normalizeMode;
    }

    void FieldGenerator.Generate(Field field, Transform transform)
    {
        Vector2 samplePosition = new Vector2(transform.position.x, transform.position.z);
        float[,] heightMap = Perlin.Generate(field.X, field.Z, seed, scale, octaves, persistance, lacunarity, samplePosition, normalizeMode);

        field.ForEachXZ((x, z) => {
            float height = heightMap[x, z] * (float)(field.Y - 1);
            
            int y = Mathf.RoundToInt(height);

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, 1);
            }
        });
    }
}
