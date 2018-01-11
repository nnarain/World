using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for generating a 3d scalar field
/// </summary>
public interface IFieldGenerator
{
    void Generate(VoxelField field, Vector3 position);
}