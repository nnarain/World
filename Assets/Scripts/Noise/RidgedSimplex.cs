using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[System.Serializable]
public class RidgedSimplex : Simplex
{
    public new double Sample(double x, double y)
    {
        return Math.Abs(Sample(x, y, 1, 1, 3));
    }

    public new double Sample(double x, double y, double z)
    {
        return Math.Abs(Sample(x, y, z, 1, 1, 3));
    }
}
