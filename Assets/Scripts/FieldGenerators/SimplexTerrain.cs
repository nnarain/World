using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexTerrain : FieldGenerator
{
    public Simplex contientNoise;
    public Simplex hillNoise;
    [Range(0, 1)]
    public float continentWeight;
    [Range(0, 1)]
    public float hillWeight;

    public CombinedNoise densityNoise;


    [Range(0, 1)]
    public double isolevel;
    public double terrainScale;

    public double hardFloorY;
    public double hardFloorScale;
    public double seaLevel;
    public double maxHeight;

    public float islandRadius;
    public AnimationCurve islandCurve;

    public override void Generate(VoxelField field, Vector3 position)
    {
        var islandEval = islandCurve.Copy();

        DensitySampler densitySampler = new DensitySampler(densityNoise, position, field.X, field.Y, field.Z, 4);

        field.ForEachXZ((x, z) =>
        {
            Vector3 ws = position + new Vector3(x, 0, z);

            var continentHeight = ((float)AddOctaves(contientNoise, ws.x, ws.z, 1, 1, 1)).Remap(-1, 1, 0, 1);
            var hillHeight = ((float)AddOctaves(hillNoise, ws.x, ws.z, 1, 1, 1)).Remap(-1, 1, 0, 1);
            var islandMask = IslandMask(ws.x, ws.z, islandEval);

            var height = (float)((continentHeight * continentWeight + hillHeight * hillWeight) * islandMask * maxHeight);
            height = Mathf.Clamp(height, 0, field.Y - 1);

            ///*
            for (int y = 0; y < field.Y; ++y)
            {
                ws.y = position.y + y;

                var density = -(position.y + height + ws.y);
                density += (float)densitySampler.Sample(x, y, z);
                density += (float)(Saturate((hardFloorY - ws.y) * 3) * hardFloorScale);

                if (density >= isolevel)
                {
                    if (ws.y <= seaLevel + 3)
                    {
                        field.Set(x, y, z, Blocks.Type.Sand.ToByte());
                    }
                    else
                    {
                        field.Set(x, y, z, Blocks.Type.Stone.ToByte());
                    }
                }
                else
                {
                    if (ws.y <= seaLevel)
                    {
                        field.Set(x, y, z, Blocks.Type.Water.ToByte());
                    }
                }
            }
            //*/
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


    private float IslandMask(float x, float y, AnimationCurve curve)
    {
        return curve.Evaluate(Mathf.Max(0f, InverseIslandCenter(x, y)));
    }

    private float InverseIslandCenter(float x, float y)
    {
        return 1.0f - PercentOfIslandCenter(x, y);
    }

    private float PercentOfIslandCenter(float x, float y)
    {
        float d = DistanceFromCenter(x, y);
        return d / islandRadius;
    }

    private float DistanceFromCenter(float x, float y)
    {
        return Mathf.Sqrt((x * x) + (y * y));
    }
}
