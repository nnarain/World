using System;
using System.Collections.Generic;
using UnityEngine;

public class PerlinHeightMapGenerator : FieldGenerator
{
    [System.Serializable]
    public struct Config
    {
        public float scale;
        public int octaves;
        public float persistance;
        [Range(0, 2)]
        public float lacunarity;
    }

    private float scale;
    private int octaves;
    private float persistance;
    private float lacunarity;

    public PerlinHeightMapGenerator(Config config)
    {
        scale = config.scale;
        octaves = config.octaves;
        persistance = config.persistance;
        lacunarity = config.lacunarity;
    }

    void FieldGenerator.Generate(Field field, Transform transform)
    {
        float[,] heightMap = Noise.Generate(field.X, field.Z, scale, octaves, persistance, lacunarity);

        field.ForEachXZ((x, z) => {
            float height = heightMap[x, z] * (field.Y - 1);
            int y = Mathf.RoundToInt(height);

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, 1);
            }
        });
    }
}
