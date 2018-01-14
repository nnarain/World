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
        Land
    }

    public float islandRadius = 32f;
    public int seaLevel = 0;

    public Perlin continentNoise;
    public AnimationCurve continentCurve;
    public Perlin mountainNoise;
    public AnimationCurve mountainCurve;
    public Perlin hillNoise;
    public AnimationCurve hillCurve;

    public AnimationCurve islandMaskCurve;

    [Range(0,1)]
    public float continentWeight;
    [Range(0, 1)]
    public float mountainWeight;
    [Range(0, 1)]
    public float hillWeight;


    public Color[] voxelColor = new Color[Enum.GetNames(typeof(VoxelType)).Length];

    public override void Generate(VoxelField field, Vector3 position)
    {
        Vector2 chunkPosition = position.ToXZ();
        AnimationCurve continentEval = continentCurve.Copy();
        AnimationCurve mountainEval = mountainCurve.Copy();
        AnimationCurve hillEval = hillCurve.Copy();
        AnimationCurve islandMaskEval = islandMaskCurve.Copy();

        float[,] continentMap = continentNoise.Generate(field.X, field.Z, chunkPosition);
        float[,] mountainMap = mountainNoise.Generate(field.X, field.Z, chunkPosition);
        float[,] hillMap = hillNoise.Generate(field.X, field.Z, chunkPosition);

        field.ForEachXZ((x, z) => {
            /*
                         float islandMask = IslandMask(position.x + x, position.z + z, islandMaskEval);
                        float continent = continentEval.Evaluate(continentMap[x, z]) * continentWeight;
                        float mountain = mountainEval.Evaluate(mountainMap[x, z]) * mountainWeight;
                        float hill = hillEval.Evaluate(hillMap[x, z]) * hillWeight;

                        float height = (continent + hill + mountain) * islandMask * (field.Y - 1);

                        int maxY = Mathf.RoundToInt(Mathf.Clamp(height, 0, field.Y-1));
                         */

            float islandMask = IslandMask(position.x + x, position.z + z, islandMaskEval);

            float continent = continentEval.Evaluate(continentMap[x, z]) * (field.Y - 1);
            float mountains = mountainEval.Evaluate(mountainMap[x, z]) * (field.Y - 1);

            float height = (continent + mountains) * islandMask;
            height = Mathf.Clamp(height, 0, field.Y - 1);

            int maxY = Mathf.RoundToInt(height);

            for (int y = 0; y < field.Y; ++y)
            {
                if (y <= seaLevel)
                {
                    field.Set(x, y, z, (byte)VoxelType.Water);
                }
                else if (y <= maxY)
                {
                    field.Set(x, y, z, (byte)VoxelType.Land);
                }
            }

        });
    }

    private float IslandMask(float x, float y, AnimationCurve curve)
    {
        float d = Mathf.Sqrt((x * x) + (y * y));
        float g = d / islandRadius;

        return curve.Evaluate(Mathf.Max(0f, 1.0f - g));
    }

    public override Color GetVoxelColor(byte type)
    {
        return voxelColor[type];
    }

    private void OnValidate()
    {
        
    }
}
