using System;
using System.Collections.Generic;
using UnityEngine;

public class ElevationMoistureFieldGenerator : FieldGenerator
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

            float temperature = heightToTemperatureEval.Evaluate(heightMap[x, z]).Remap(0, 1, -10, 30);
            float moisture = moistureMap[x, z] * 400;

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, (byte)WhittakerBiomes(temperature, moisture));
            }
        });
    }

    public override Color GetVoxelColor(byte type)
    {
        return voxelColors[type];
    }

    private VoxelType WhittakerBiomes(float temperature, float moisture)
    {
        if (moisture < 100)
        {
            if (temperature <= 30)
            {
                return VoxelType.Sand;
            }
            else if (temperature <= 15)
            {
                return VoxelType.Dirt;
            }
            else
            {
                return VoxelType.Snow;
            }
        }
        else if (moisture < 200)
        {
            return VoxelType.Grass;
        }
        else
        {
            return VoxelType.Grass;
        }
    }

    private VoxelType ElevationToVoxelType(int elevation, int max)
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
