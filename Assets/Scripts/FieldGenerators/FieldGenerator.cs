using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for generating a 3d scalar field
/// </summary>
public interface FieldGenerator
{
    void Generate(Field field, Transform transform);
}