﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexTerrain : FieldGenerator
{
    public FastNoiseUnity noise;

    public float maxHeight;

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var sample = noise.Sample(ws.x, ws.z).Remap(-1.0f, 1.0f, 0.0f, maxHeight);

            int maxY = (int)sample;

            for (int y = 0; y < maxY; ++y)
            {
                field.Set(x, y, z, 2);
            }

        });
    }

    private void OnValidate()
    {
        noise.Update();
    }
}
