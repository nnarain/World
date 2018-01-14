using System;
using UnityEngine;

/// <summary>
/// Generate an island using a 2D height map
/// </summary>
public class IslandFieldGenerator : FieldGenerator
{
    private enum VoxelType
    {
        Air,
        Water,
        Land,
        Stone,
        Snow,
        Sand,
        GrassLand,
        RainForest,
        Scorched
    }

    public float islandRadius = 32f;
    public int seaLevel = 0;

    public Perlin continentNoise;
    public AnimationCurve continentCurve;
    public Perlin mountainNoise;
    public AnimationCurve mountainCurve;
    public float mountainScale;
    public AnimationCurve islandMaskCurve;
    public float terrainScale;

    public Perlin moistureNoise;
    [Range(0,1)]
    public float moistureMapWeight;
    public AnimationCurve temperatureCurve;

    [Range(0, 1)]
    public float continentWeight;
    [Range(0, 1)]
    public float mountainWeight;


    public Color[] voxelColor = new Color[Enum.GetNames(typeof(VoxelType)).Length];

    public override void Generate(VoxelField field, Vector3 position)
    {
        Vector2 chunkPosition = position.ToXZ();
        AnimationCurve continentEval = continentCurve.Copy();
        AnimationCurve mountainEval = mountainCurve.Copy();
        AnimationCurve islandMaskEval = islandMaskCurve.Copy();
        AnimationCurve temperatureEval = temperatureCurve.Copy();

        float[,] continentMap = continentNoise.Generate(field.X, field.Z, chunkPosition);
        float[,] mountainMap = mountainNoise.Generate(field.X, field.Z, chunkPosition);
        float[,] moistureMap = moistureNoise.Generate(field.X, field.Z, chunkPosition);

        field.ForEachXZ((x, z) =>
        {
            float blockX = position.x + x;
            float blockZ = position.z + z;
            
            float islandMask = IslandMask(position.x + x, position.z + z, islandMaskEval);
            float continent = continentEval.Evaluate(continentMap[x, z]) * continentWeight;
            float mountain = mountainEval.Evaluate(mountainMap[x, z]) * mountainWeight;

            float height = (continent + mountain) * terrainScale * islandMask * (field.Y - 1);
            height = Mathf.Clamp(height, 0, field.Y - 1);

            int maxY = Mathf.RoundToInt(height);

            float temperature = temperatureEval.Evaluate(height / (float)(field.Y - 1));
            float moisture = (moistureMap[x, z] * moistureMapWeight) + (PercentOfIslandCenter(blockX, blockZ) * (1.0f - moistureMapWeight));

            for (int y = 0; y < field.Y; ++y)
            {
                if (y <= seaLevel)
                {
                    field.Set(x, y, z, (byte)VoxelType.Water);
                }
                else if (y <= maxY)
                {
                    field.Set(x, y, z, (byte)Biome(height, temperature, moisture, field.Y - 1));
                }
            }

        });
    }

    private VoxelType Biome(float e, float t, float m, float maxE)
    {
        // mountains and snow caps
        if (e >= 0.50 * maxE)
        {
            if (e >= 0.75 * maxE)
            {
                return VoxelType.Snow;
            }
            else
            {
                return VoxelType.Stone;
            }
        }
        else
        {
            // sand/beaches
            if (e <= seaLevel + 3)
            {
                return VoxelType.Sand;
            }
            else
            {
                // in land biomes
                if (t >= 0.7)
                {
                    if (m >= 0.25)
                    {
                        if (m <= 0.55)
                        {
                            return VoxelType.GrassLand;
                        }
                        else
                        {
                            return VoxelType.RainForest;
                        }
                    }
                    else
                    {
                        return VoxelType.Scorched;
                    }
                }
            }
        }

        return VoxelType.Land;
    }

    private float IslandMask(float x, float y, AnimationCurve curve)
    {
        return curve.Evaluate(Mathf.Max(0f, InverseIslandCenter(x, y)));
    }

    private float InverseIslandCenter(float x, float y)
    {
        return 1.0f - PercentOfIslandCenter(x, y);
    }

    private float PercentOfIslandCenter(float x, float y)
    {
        float d = DistanceFromCenter(x, y);
        return d / islandRadius;
    }

    private float DistanceFromCenter(float x, float y)
    {
        return Mathf.Sqrt((x * x) + (y * y));
    }

    public override Color GetVoxelColor(byte type)
    {
        return voxelColor[type];
    }

    private void OnValidate()
    {

    }
}
