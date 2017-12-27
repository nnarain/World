using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Represent a 3D grid.
/// This grid will represent the blocks that should be created.
/// </summary>
public class Grid
{
    int x, y, z;

    public int X { get { return x; } }
    public int Y { get { return y; } }
    public int Z { get { return z; } }

    private int[] grid = null;

    public Grid(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;

        grid = new int[x * y * z];
    }

    public void Set(int x, int y, int z, int v)
    {
        grid[GetIndex(x, y, z)] = v;
    }

    public int Get(int x, int y, int z)
    {
        if (x >= this.x || x < 0 || y >= this.y || y < 0 || z >= this.z || z < 0)
        {
            return -1;
        }

        return grid[GetIndex(x, y, z)];
    }

    private int GetIndex(int x, int y, int z)
    {
        return (y * this.x * this.z) + (z * this.x) + x;
    }
}
