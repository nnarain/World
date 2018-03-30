using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleySimplex : NoiseViz
{
    public Worley noise;
    public SimplexVector2 distort;

    public override Field CreateNoiseMap(int width, int height)
    {
        var field = new Field(width, 1, height);

        field.ForEachXYZ((x, y, z) =>
        {
            var offset = distort.Sample(x, z);
            var sample = noise.Sample(x + offset.x, z + offset.y);

            field.Set(x, y, z, sample);
        });

        return field;
    }
}
