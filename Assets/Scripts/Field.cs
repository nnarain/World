using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represent a 3D field.
/// This grid will represent the blocks that should be created.
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

    private int GetIndex(int x, int y, int z)
    {
        return (y * this.x * this.z) + (z * this.x) + x;
    }
}
