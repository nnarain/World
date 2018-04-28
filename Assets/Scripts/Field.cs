
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represents a 3D scalar field
/// </summary>
public class Field
{
    int x, y, z;

    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Z { get { return z; } }

    private double[] field = null;

    public Field(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        field = new double[x * y * z];
    }

    public void Set(int x, int y, int z, double v)
    {
        field[GetIndex(x, y, z)] = v;
    }

    public double Get(int x, int y, int z)
    {
        if (x >= this.x) x = this.x - 1;
        if (x < 0) x = 0;
        if (y >= this.y) y = this.y - 1;
        if (y < 0) y = 0;
        if (z >= this.z) z = this.z - 1;
        if (z < 0) z = 0;

        return field[GetIndex(x, y, z)];
    }

    public void ForEach(Action<int, int, int, double> action)
    {
        for (int x = 0; x < this.x; ++x)
        {
            for (int y = 0; y < this.y; ++y)
            {
                for (int z = 0; z < this.z; ++z)
                {
                    double v = Get(x, y, z);
                    action(x, y, z, v);
                }
            }
        }
    }

    public void ForEachXZ(Action<int, int> action)
    {
        for (int x = 0; x < this.x; ++x)
        {
            for (int z = 0; z < this.z; ++z)
            {
                action(x, z);
            }
        }
    }

    public void ForEachXYZ(Action<int, int, int> action)
    {
        for (int x = 0; x < this.x; ++x)
        {
            for (int y = 0; y < this.y; ++y)
            {
                for (int z = 0; z < this.z; ++z)
                {
                    action(x, y, z);
                }
            }
        }
    }

    private int GetIndex(int x, int y, int z)
    {
        return (y * this.x * this.z) + (z * this.x) + x;
    }
}
