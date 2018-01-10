using System;
using System.Collections.Generic;
using UnityEngine;

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

[System.Serializable]
public struct Voxel
{
    byte type;
    public VoxelType Type
    {
        get { return (VoxelType)type; }
        set { type = (byte)value; }
    }

    public static readonly Voxel air = new Voxel();
}
