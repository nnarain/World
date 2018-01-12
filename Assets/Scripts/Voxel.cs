using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Voxel
{
    public byte Type
    {
        get; set;
    }

    public static readonly Voxel none = new Voxel();
}
