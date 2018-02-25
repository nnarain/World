using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineSimplexTerrain : FieldGenerator
{
    public CombinedNoise noise;

    public double hardFloorY;
    public double hardFloorScale;
    public int seaLevel;

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            Vector3 ws = position + new Vector3(x, 0, z);

            var height = ((float)noise.Sample(ws.x, ws.z)).Remap(-1, 1, 0, 1) * (field.Y - 1);

            int y = (int)Mathf.Clamp(height, 0, field.Y - 1);

            for (int j = 0; j < y; ++j)
            {
                field.Set(x, j, z, Blocks.Type.Stone.ToByte());
            }

            /*
            for (int y = 0; y < field.Y - 1; ++y)
            {
                if (y <= seaLevel)
                {
                    field.Set(x, y, z, Blocks.Type.Water.ToByte());
                }
                else if (y <= maxY)
                {
                    if (y < seaLevel + 4)
                    {
                        field.Set(x, y, z, Blocks.Type.Sand.ToByte());
                    }
                    else
                    {
                        field.Set(x, y, z, Blocks.Type.Land.ToByte());
                    }
                }
            }
            */

        });
    }

    double AddOctaves(Simplex n, Vector3 ws, double f, double a, int octaves)
    {
        double total = 0;
        for (int i = 0; i < octaves; ++i)
        {
            total += n.Sample(ws, f, a, i);
        }

        return total;
    }

    double AddOctaves(Simplex n, double x, double z, double f, double a, int octaves)
    {
        double total = 0;
        for (int i = 0; i < octaves; ++i)
        {
            total += n.Sample(x, z, f, a, i);
        }

        return total;
    }

    private static double Saturate(double a)
    {
        return Mathf.Max(0, (float)a);
    }
}
