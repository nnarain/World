using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SimplexViz : NoiseViz
{
    public Simplex noise;

    public override Field CreateNoiseMap(int width, int height)
    {
        base.CreateNoiseMap(width, height);

        var field = new Field(width, 1, height);

        field.ForEachXYZ((x, y, z) =>
        {
            var sample = noise.Sample(x, z).Remap(-1, 1, 0, 1);
            field.Set(x, y, z, sample);
        });

        return field;
    }
}
