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

    private float[] field = null;

    public Field(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        field = new float[x * y * z];
    }

    public void Set(int x, int y, int z, float v)
    {
        field[GetIndex(x, y, z)] = v;
    }

    public float Get(int x, int y, int z)
    {
        if (x >= this.x || x < 0 || y >= this.y || y < 0 || z >= this.z || z < 0)
        {
            return -1.0f;
        }

        return field[GetIndex(x, y, z)];
    }

    public void ForEach(Action<int, int, int, float> action)
    {
        for (int x = 0; x < this.x; ++x)
        {
            for (int y = 0; y < this.y; ++y)
            {
                for (int z = 0; z < this.z; ++z)
                {
                    float v = Get(x, y, z);
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

    private int GetIndex(int x, int y, int z)
    {
        return (y * this.x * this.z) + (z * this.x) + x;
    }
}
