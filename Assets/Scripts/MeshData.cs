using System;
using System.Collections.Generic;
using UnityEngine;

public class MeshData
{
    public readonly List<Vector3> vertices;
    public readonly List<int> triangles;
    public readonly List<Color> colors;

    public MeshData(List<Vector3> v, List<int> t, List<Color> c)
    {
        vertices = v;
        triangles = t;
        colors = c;
    }
}
