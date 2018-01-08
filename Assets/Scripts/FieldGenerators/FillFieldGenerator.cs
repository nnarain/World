using System;
using System.Collections.Generic;
using UnityEngine;

public class FillFieldGenerator : FieldGenerator
{
    void FieldGenerator.Generate(Field field, Vector3 position)
    {
        field.ForEach((x, y, z, v) =>
        {
            field.Set(x, y, z, 1);
        });
    }
}
