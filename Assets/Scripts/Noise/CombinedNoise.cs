using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombinedNoise
{
    public Simplex sampleNoise;
    public Simplex offsetNoise;

    public double Sample(double x, double y)
    {
        return sampleNoise.Sample(x + offsetNoise.Sample(x, y, 1, 1, 2), y, 1, 1, 3);
    }

    public double Sample(Vector3 ws)
    {
        return sampleNoise.Sample(ws.x + offsetNoise.Sample(ws.x, ws.y, ws.z, 1, 1, 2), ws.y, ws.z, 1, 1, 3);
    }
}
