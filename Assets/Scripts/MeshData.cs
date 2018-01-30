using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data required for constructing a mesh
/// </summary>
public class MeshData
{
    public readonly List<Vector3> vertices;
    public readonly List<int> triangles;
    public readonly List<Color> colors;
    public readonly List<Vector2> uvs;

    public MeshData(List<Vector3> v, List<int> t, List<Color> c, List<Vector2> uvs = null)
    {
        vertices = v;
        triangles = t;
        colors = c;
        this.uvs = uvs;
    }

    public void Clear()
    {
        if (vertices != null)
            vertices.Clear();
        if (triangles != null)
            triangles.Clear();
        if (colors != null)
            colors.Clear();
        if (uvs != null)
            uvs.Clear();
    }
}
