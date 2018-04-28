using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represents a 3D scalar field
/// </summary>
public class VoxelField
{
    int x, y, z;

    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Z { get { return z; } }

    private Voxel[] field = null;

    public VoxelField(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        field = new Voxel[x * y * z];        
    }

    public void Set(int x, int y, int z, byte t)
    {
        field[GetIndex(x, y, z)].Type = t;
    }

    public Voxel Get(int x, int y, int z)
    {
        if (x >= this.x || x < 0 || y >= this.y || y < 0 || z >= this.z || z < 0)
        {
            return Voxel.none;
        }

        return field[GetIndex(x, y, z)];
    }

    public void ForEach(Action<int, int, int, Voxel> action)
    {
        for (int x = 0; x < this.x; ++x)
        {
            for (int y = 0; y < this.y; ++y)
            {
                for (int z = 0; z < this.z; ++z)
                {
                    Voxel v = Get(x, y, z);
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

    public void ForEachXYZ(Action<int,int,int> action)
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
