using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombinedNoise : INoiseSampler
{
    public INoiseSampler n1;
    public INoiseSampler n2;
    public INoiseSampler interpolation;

    public CombinedNoise(INoiseSampler n1, INoiseSampler n2, INoiseSampler i)
    {
        this.n1 = n1;
        this.n2 = n2;
        this.interpolation = i;
    }

    public double Sample(double x, double y, double z)
    {
        var s1 = (float)n1.Sample(x, z);
        var s2 = (float)n2.Sample(x, z);

        var c = Mathf.Lerp(s1, s2, (float)interpolation.Sample(x, z).Remap(-1, 1, 0, 1));

        return c;
    }

    public double Sample(double x, double z)
    {
        return Sample(x, 0, z);
    }
}
