﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Simplex 2D vector field
/// </summary>
[System.Serializable]
public class SimplexVector2
{
    public Simplex noise1;
    public Simplex noise2;

    public double scale;

    public Vector2 Sample(double x, double y)
    {
        var sx = (float)(noise1.Sample(x, y).Remap(-1, 1, 0, 1) * scale);
        var sy = (float)(noise2.Sample(x, y).Remap(-1, 1, 0, 1) * scale);

        return new Vector2(sx, sy);
    }
}
