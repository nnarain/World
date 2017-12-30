using System;
using System.Collections.Generic;
using UnityEngine;

class SineFieldGenerator : FieldGenerator
{
    void FieldGenerator.Generate(Chunk chunk)
    {
        var transform = chunk.transform;
        var field = chunk.Field;

        // chunk world position
        Vector3 p = transform.position;

        float a = (float)field.Y / 3f;
        const float f = 0.25f;

        field.ForEachXZ((x, z) => {
            Vector3 bp = p + new Vector3(x * CubeMetrics.CUBE_SIZE, 0, z * CubeMetrics.CUBE_SIZE);

            float m = Mathf.Sqrt(bp.x * bp.x + bp.z * bp.z);
            float y = a * Mathf.Sin(m * f) + a;
            y = Mathf.Clamp(y, 0, field.Y);

            field.Set(x, (int)y, z, 1);
        });

    }
}
