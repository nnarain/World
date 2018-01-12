using System;
using System.Collections.Generic;
using UnityEngine;

public class PerlinHeightMapGenerator : FieldGenerator
{
    public enum VoxelType
    {
        Air,
        Water,
        Sand,
        Dirt,
        Grass,
        Stone,
        Snow
    }

    public int seed;
    public float scale;
    [Range(1, 100)]
    public int octaves;
    [Range(0,1)]
    public float persistance;
    [Range(1, 2)]
    public float lacunarity;
    public AnimationCurve heightCurve;

    public Color[] voxelColors = new Color[Enum.GetNames(typeof(VoxelType)).Length];

    public override void Generate(VoxelField field, Vector3 position)
    {
        Vector2 samplePosition = new Vector2(position.x, position.z);
        float[,] heightMap = PerlinNoise.Generate(field.X, field.Z, seed, scale, octaves, persistance, lacunarity, samplePosition);

        field.ForEachXZ((x, z) => {
            float height = heightCurve.Evaluate(heightMap[x, z]) * (float)(field.Y - 1);

            int y = Mathf.RoundToInt(height);

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, (byte)ElevationToVoxelType(i, field.Y));
            }
        });
    }

    public override Color GetVoxelColor(byte type)
    {
        return voxelColors[type];
    }

    private VoxelType ElevationToVoxelType (int elevation, int max)
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
        else if (elevation < 60)
        {
            return VoxelType.Stone;
        }

        return VoxelType.Snow;
    }
}
