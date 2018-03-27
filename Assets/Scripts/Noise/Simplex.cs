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

    public int octaves = 1;

    public double scale = 1;

    [Range(0,2)]
    public double e = 1;

    private OpenSimplexNoise n;

    public Simplex()
    {
        n = new OpenSimplexNoise(seed);
    }

    public double Sample(double x, double y)
    {
        return Math.Pow(Sample(x, y, 1, 1, octaves), e);
    }

    public double Sample(double x, double y, double z)
    {
        return Math.Pow(Sample(x, y, z, 1, 1, octaves), e);
    }

    public double Sample(Vector3 p, double f, double a, int o = 0)
    {
        return Sample(p.x, p.y, p.z, f, a, o);
    }

    public double Sample(double x, double y, double z, double f, double a, int octaves = 0)
    {
        /*
        if (o > 0)
        {
            f *= lacunarity * o;
            a *= persistance * o;
        }

        var fh = f / scaleH;
        var fv = f / scaleV;

        return n.Evaluate(x * fh, y * fv, z * fh) * a * scale;
        */

        var maxValue = 0.0;
        var sample = 0.0;

        for (int o = 0; o < octaves; ++o)
        {
            var sampleX = x / scaleH * f;
            var sampleY = y / scaleV * f;
            var sampleZ = z / scaleH * f;

            sample += n.Evaluate(sampleX, sampleY, sampleZ) * a;

            maxValue += a;

            f *= lacunarity;
            a *= persistance;
        }

        return (sample / maxValue) * scale;
    }

    public double Sample(double x, double z, double f, double a, int octacves = 0)
    {
        var maxValue = 0.0;
        var sample = 0.0;

        for (int o = 0; o < octaves; ++o)
        {
            var sampleX = x / scaleH * f;
            var sampleZ = z / scaleH * f;

            sample += n.Evaluate(sampleX, sampleZ) * a;

            maxValue += a;

            f *= lacunarity;
            a *= persistance;
        }

        return (sample / maxValue) * scale;
    }
}
