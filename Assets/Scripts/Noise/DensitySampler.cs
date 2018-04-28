using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Compute a reduced density field
/// </summary>
public class DensitySampler
{
    private Field densityField;

    private int reduce;

    public DensitySampler(INoiseSampler sampler, Vector3 offset, int x, int y, int z, int reduce)
    {
        densityField = new Field(x / reduce + 1, y / reduce + 1, z / reduce + 1);

        this.reduce = reduce;

        Init(sampler, offset);
    }

    private void Init(INoiseSampler sampler, Vector3 offset)
    {
        densityField.ForEachXYZ((x, y, z) =>
        {
            densityField.Set(x, y, z, sampler.Sample(x * reduce + offset.x, y * reduce + offset.y, z * reduce + offset.z));
        });
    }

    public double Sample(int x, int y, int z)
    {
        int x2 = (x / reduce) + 0;
        int y2 = (y / reduce) + 0;
        int z2 = (z / reduce) + 0;

        double value = LinearInterpolate3d(
            densityField.Get(x2, y2, z2), 
            densityField.Get(x2 + 1, y2, z2), 
            densityField.Get(x2, y2 + 1, z2),
            densityField.Get(x2 + 1, y2 + 1, z2), 
            densityField.Get(x2, y2, z2 + 1),
            densityField.Get(x2 + 1, y2, z2 + 1),
            densityField.Get(x2, y2 + 1, z2 + 1), 
            densityField.Get(x2 + 1, y2 + 1, z2 + 1),
            (x % reduce) / (double)reduce,
            (y % reduce) / (double)reduce,
            (z % reduce) / (double)reduce);

        return value;
    }

    /// <summary>
    /// https://en.wikipedia.org/wiki/Bilinear_interpolation#Unit_Square
    /// </summary>
    /// <param name="xm_ym_zm"></param>
    /// <param name="xp_ym_zm"></param>
    /// <param name="xm_yp_zm"></param>
    /// <param name="xp_yp_zm"></param>
    /// <param name="xm_ym_zp"></param>
    /// <param name="xp_ym_zp"></param>
    /// <param name="xm_yp_zp"></param>
    /// <param name="xp_yp_zp"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    /// <returns></returns>
    private double LinearInterpolate3d(double xm_ym_zm, double xp_ym_zm, double xm_yp_zm, double xp_yp_zm,
                                    double xm_ym_zp, double xp_ym_zp, double xm_yp_zp, double xp_yp_zp,
                                    double x, double y, double z)
    {
        return (xm_ym_zm * (1 - x) * (1 - y) * (1 - z)) + 
               (xp_ym_zm * x * (1 - y) * (1 - z))       + 
               (xm_yp_zm * (1 - x) * y * (1 - z))       + 
               (xp_yp_zm * x * y * (1 - z))             +
               (xm_ym_zp * (1 - x) * (1 - y) * z)       + 
               (xp_ym_zp * x * (1 - y) * z)             + 
               (xm_yp_zp * (1 - x) * y * z)             + 
               (xp_yp_zp * x * y * z);
    }
}
