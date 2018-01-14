using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevationMoistureFieldGenerator : FieldGenerator
{
    public enum VoxelType
    {
        Air,
        Water,
        TemperateForest,
        DeciduousForset,
        Grassland,
        TemperateDesert,
        RainForset,
        Snow,
        Tundra
    }

    [System.Serializable]
    public struct NoiseConfig
    {
        public int seed;
        public float scale;
        [Range(1, 100)]
        public int octaves;
        [Range(0, 1)]
        public float persistance;
        [Range(1, 2)]
        public float lacunarity;

    }

    public NoiseConfig heightConfig;
    public NoiseConfig moistureConfig;

    public AnimationCurve heightCurve;
    public AnimationCurve heightToTemperature;

    public Color[] voxelColors = new Color[Enum.GetNames(typeof(VoxelType)).Length];

    private const float SEA_LEVEL = 4f;

    public override void Generate(VoxelField field, Vector3 position)
    {
        AnimationCurve heightEval = new AnimationCurve(heightCurve.keys);
        AnimationCurve heightToTemperatureEval = new AnimationCurve(heightToTemperature.keys);

        Vector2 samplePosition = new Vector2(position.x, position.z);
        float[,] heightMap = PerlinNoise.Generate(
            field.X, field.Z,
            heightConfig.seed,
            heightConfig.scale,
            heightConfig.octaves,
            heightConfig.persistance,
            heightConfig.lacunarity,
            samplePosition
        );

        float[,] moistureMap = PerlinNoise.Generate(
            field.X, field.Z,
            moistureConfig.seed,
            moistureConfig.scale,
            moistureConfig.octaves,
            heightConfig.persistance,
            moistureConfig.lacunarity,
            samplePosition
        );

        field.ForEachXZ((x, z) => {
            float height = heightEval.Evaluate(heightMap[x, z]) * (float)(field.Y - 1);
            int y = Mathf.RoundToInt(height);

            float temperature = heightToTemperatureEval.Evaluate(heightMap[x, z]).Remap(0, 1, 0, 4);
            float moisture = moistureMap[x, z].Remap(0, 1, 0, 5);

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, (byte)Simple(height));
            }
        });
    }

    private VoxelType Simple(float elevation)
    {
        if (elevation <= SEA_LEVEL)
        {
            return VoxelType.Water;
        }
        else
        {
            return VoxelType.TemperateForest;
        }
    }

    private VoxelType Whittaker(float temperature, float moisture)
    {
        if (temperature >= 3)
        {
            if (moisture >= 4)
            {
                return VoxelType.RainForset;
            }
            else if (moisture >= 2)
            {
                return VoxelType.Grassland;
            }
            else
            {
                return VoxelType.TemperateDesert;
            }
        }
        else if (temperature >= 2)
        {
            if (moisture >=4)
            {
                return VoxelType.TemperateForest;
            }
            else if (moisture >= 2)
            {
                return VoxelType.DeciduousForset;
            }
            else
            {
                return VoxelType.TemperateDesert;
            }
        }
        else
        {
            if (moisture >= 4)
            {
                return VoxelType.Snow;
            }
            else
            {
                return VoxelType.Tundra;
            }
        }
        
        return VoxelType.Air;
    }

    public override Color GetVoxelColor(byte type)
    {
        return voxelColors[type];
    }

}
