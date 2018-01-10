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
        public AnimationCurve heightCurve;
    }

    private int seed;
    private float scale;
    private int octaves;
    private float persistance;
    private float lacunarity;
    public AnimationCurve heightCurve;

    public PerlinHeightMapGenerator(Config config)
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

        field.ForEachXZ((x, z) => {
            float height = heightCurve.Evaluate(heightMap[x, z]) * (float)(field.Y - 1);
            
            int y = Mathf.RoundToInt(height);

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, ElevationToVoxelType(i, field.Y));
            }
        });
    }

    VoxelType ElevationToVoxelType (int elevation, int max)
    {
        if (elevation < 3)
        {
            return VoxelType.Water;
        }
        else if (elevation < 5)
        {
            return VoxelType.Sand;
        }
        else if (elevation < 7)
        {
            return VoxelType.Dirt;
        }
        else if (elevation < 10)
        {
            return VoxelType.Grass;
        }

        return VoxelType.Stone;
    }
}
