using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexTerrain : FieldGenerator
{
    public Simplex heightNoise;
    public Simplex densityNoise;
    public Worley worley;

    public double maxHeight;
    public double midLevel;

    public double[] maxHeights;


    public override void Generate(VoxelField field, Vector3 position)
    {
        DensitySampler sampler = new DensitySampler(densityNoise, position, field.X, field.Y, field.Z, 4);

        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var mh = GetMaxHeight(worley.GetClosestPoint(ws));
            var height = (AddOctaves(heightNoise, ws.x, ws.z, 1, 1, 2) * mh) + midLevel;

            int maxY = Mathf.RoundToInt(Mathf.Clamp((float)height, 0, field.Y - 1));

            for (int y = 0; y < maxY; ++y)
            {
                ws.y = position.y + y;

                var density = (double)(-y + maxY);
                density += sampler.Sample(x, y, z);
                density += Saturate((midLevel - ws.y) * 3) * 40;

                if (density > 0)
                {
                    field.Set(x, y, z, Blocks.Type.Stone.ToByte());
                }
            }
        });
    }


    double AddOctaves(Simplex n, double x, double z, double f, double a, int octaves)
    {
        double maxValue = 0;
        double amp = a;

        double total = 0;
        for (int i = 0; i < octaves; ++i)
        {
            total += n.Sample(x, z, f, a, i).Remap(-1, 1, 0, 1);

            maxValue += amp;
            amp *= n.persistance;
        }

        return total / maxValue;
    }

    private double GetMaxHeight(Vector3 seed)
    {
        var s = seed.GetHashCode();
        s = Mathf.Abs(s) % maxHeights.Length;

        return maxHeights[s];
    }

    private static double Saturate(double a)
    {
        return Mathf.Max(0, (float)a);
    }
}
