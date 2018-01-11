using System;
using System.Collections.Generic;
using UnityEngine;

public class FillFieldGenerator : IFieldGenerator
{
    void IFieldGenerator.Generate(VoxelField field, Vector3 position)
    {
        field.ForEach((x, y, z, v) =>
        {
            field.Set(x, y, z, VoxelType.Grass);
        });
    }
}
