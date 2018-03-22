using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Simplex : INoiseSampler
{
    public int seed;
    public double scaleH;
    public double scaleV;
    [Range(0, 1)]
    public float persistance;
    [Range(1, 2)]
    public float lacunarity;

    public double scale;

    private OpenSimplexNoise n;

    public Simplex()
    {
        n = new OpenSimplexNoise(seed);
    }

    public double Sample(double x, double y)
    {
        return Sample(x, y, 1, 1, 3);
    }

    public double Sample(double x, double y, double z)
    {
        return Sample(x, y, z, 1, 1, 3);
    }

    public double Sample(Vector3 p, double f, double a, int o = 0)
    {
        return Sample(p.x, p.y, p.z, f, a, o);
    }

    public double Sample(double x, double y, double z, double f, double a, int o = 0)
    {
        if (o > 0)
        {
            f *= lacunarity * o;
            a *= persistance * o;
        }

        var fh = f / scaleH;
        var fv = f / scaleV;

        return n.Evaluate(x * fh, y * fv, z * fh) * a * scale;
    }

    public double Sample(double x, double z, double f, double a, int o = 0)
    {
        if (o > 0)
        {
            f *= lacunarity * o;
            a *= persistance * o;
        }

        var fh = f / scaleH;
        var fv = f / scaleV;

        return n.Evaluate(x * fh, z * fh) * scale;
    }
}
