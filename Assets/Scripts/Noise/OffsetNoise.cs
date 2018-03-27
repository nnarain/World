using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class OffsetNoise : INoiseSampler
{
    public Simplex sampleNoise;
    public Simplex offsetNoise;

    public double Sample(double x, double y)
    {
        return sampleNoise.Sample(x + offsetNoise.Sample(x, y, 1, 1, 2), y, 1, 1, 3);
    }

    public double Sample(double x, double y, double z)
    {
        return sampleNoise.Sample(x + offsetNoise.Sample(x, z, 1, 1, 2), y, z, 1, 1, 3);
    }

    public double Sample(Vector3 ws)
    {
        return Sample(ws.x, ws.y, ws.z);
    }

    public double Sample2D(Vector3 ws)
    {
        return Sample(ws.x, ws.z);
    }
}
