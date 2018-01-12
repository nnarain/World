using System;
using System.Collections.Generic;
using UnityEngine;

public class FillFieldGenerator : IFieldGenerator
{
    public enum VoxelType
    {
        Air
    }

    void IFieldGenerator.Generate(VoxelField field, Vector3 position)
    {
        field.ForEach((x, y, z, v) =>
        {
            field.Set(x, y, z, (byte)VoxelType.Air);
        });
    }
}
