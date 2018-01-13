using System;
using UnityEngine;

public class IslandFieldGenerator : FieldGenerator
{
    private enum VoxelType
    {
        Air,
        Water
    }

    public Perlin heightNoise;
    public AnimationCurve heightCurve;

    public Color[] voxelColor = new Color[Enum.GetNames(typeof(VoxelType)).Length];


    public override void Generate(VoxelField field, Vector3 position)
    {
        Vector2 samplePosition = position.ToXZ();
        AnimationCurve heightEval = heightCurve.Copy();

        float[,] heightMap = heightNoise.Generate(field.X, field.Z, samplePosition);

        field.ForEachXZ((x, z) => {
            float height = heightEval.Evaluate(heightMap[x, z]) * (field.Y - 1);

            int y = Mathf.RoundToInt(height);
            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, 1);
            }
        });
    }

    public override Color GetVoxelColor(byte type)
    {
        return voxelColor[type];
    }
}
