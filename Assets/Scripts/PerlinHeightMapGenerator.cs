using System;
using System.Collections.Generic;
using UnityEngine;

public class PerlinHeightMapGenerator : FieldGenerator
{
    void FieldGenerator.Generate(Field field, Transform transform)
    {
        float[,] heightMap = Noise.Generate(field.X, field.Z, 0.001f, 4, 0.5f, 2f);

        field.ForEachXZ((x, z) => {
            float height = heightMap[x, z] * (field.Y - 1);
            int y = Mathf.RoundToInt(height);

            Debug.Log(string.Format("Height: {0}, Y: {1}", height, y));

            for (int i = y; i >= 0; --i)
            {
                field.Set(x, i, z, 1);
            }
        });
    }
}
