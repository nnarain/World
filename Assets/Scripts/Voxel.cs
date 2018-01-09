using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Voxel
{
    public enum VoxelType
    {
        Air,
        Water,
        Sand,
        Dirt,
        Grass,
        Stone,
        Snow
    }

    private VoxelType type;
    public VoxelType Type
    {
        get { return type; }
        set { type = value; }
    }

    public Voxel(VoxelType type)
    {
        this.type = type;
    }

    public Voxel() : this(VoxelType.Air)
    {
    }

    // default types
    public static readonly Voxel air = new Voxel();
}
