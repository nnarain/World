using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyViz : NoiseViz
{
    public Worley noise;

    public override Field CreateNoiseMap(int width, int height)
    {
        base.CreateNoiseMap(width, height);

        var field = new Field(width, 1, height);

        field.ForEachXYZ((x, y, z) =>
        {
            var sample = noise.Sample(x, z);
            field.Set(x, y, z, sample);
        });

        return field;
    }

    private new void OnValidate()
    {
        base.OnValidate();

    }
}
