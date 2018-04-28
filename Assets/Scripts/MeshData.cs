using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Data required for constructing a mesh
/// </summary>
public class MeshData
{
    public readonly List<Vector3> vertices;
    public readonly List<int> elements;
    public readonly List<Color> colors;
    public readonly List<Vector2> uvs;

    private static readonly Vector2 emptyUV = new Vector2();

    public MeshData() : this(new List<Vector3>(), new List<int>(), new List<Color>(), new List<Vector2>())
    {
        
    }

    public MeshData(List<Vector3> v, List<int> t, List<Color> c, List<Vector2> uvs)
    {
        vertices = v;
        elements = t;
        colors = c;
        this.uvs = uvs;
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector2 uvOffset)
    {
        AddQuad(v1, v2, v3, v4, uvOffset, Color.white);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Vector2 uvOffset, Color c)
    {
        int i = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);

        uvs.Add(uvOffset);
        uvs.Add(uvOffset);
        uvs.Add(uvOffset);
        uvs.Add(uvOffset);

        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);

        elements.Add(i + 0);
        elements.Add(i + 1);
        elements.Add(i + 2);
        elements.Add(i + 2);
        elements.Add(i + 3);
        elements.Add(i + 0);
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color c)
    {
        AddQuad(v1, v2, v3, v4, emptyUV, c);
    }

    public void AddVertex(Vector3 p)
    {
        vertices.Add(p);
    }

    public void AddUV(Vector2 uv)
    {
        uvs.Add(uv);
    }

    public void AddTriangle(int v1, int v2, int v3)
    {
        elements.Add(v1);
        elements.Add(v2);
        elements.Add(v3);
    }

    public void AddColor(Color c)
    {
        colors.Add(c);
    }

    public void Clear()
    {
        if (vertices != null)
            vertices.Clear();
        if (elements != null)
            elements.Clear();
        if (colors != null)
            colors.Clear();
        if (uvs != null)
            uvs.Clear();
    }
}
