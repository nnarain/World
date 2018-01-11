using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate a cube mesh from a field
/// </summary>
public class BlockMeshExtractor : IMeshExtractor
{
    private List<Vector3> vertices;
    private List<int> triangles;
    private List<Color> colors;

    public BlockMeshExtractor()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
    }

    void IMeshExtractor.Extract(Chunk chunk, Action<MeshData> callback)
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < chunk.Field.X; ++x)
        {
            for (int y = 0; y < chunk.Field.Y; ++y)
            {
                for (int z = 0; z < chunk.Field.Z; ++z)
                {
                    Voxel block = chunk.GetField(x, y, z);

                    if (block.Type != VoxelType.Air)
                    {
                        // get neighbor states and determine which faces should be constructed
                        bool l = chunk.GetField(x - 1, y, z).Type == VoxelType.Air;
                        bool r = chunk.GetField(x + 1, y, z).Type == VoxelType.Air;
                        bool t = chunk.GetField(x, y + 1, z).Type == VoxelType.Air;
                        bool b = chunk.GetField(x, y - 1, z).Type == VoxelType.Air;
                        bool n = chunk.GetField(x, y, z - 1).Type == VoxelType.Air;
                        bool f = chunk.GetField(x, y, z + 1).Type == VoxelType.Air;

                        CreateCubeMesh(x, y, z, Color.red, l, r, t, b, n, f);
                    }
                }
            }
        }

        MeshData data = new MeshData(vertices, triangles, colors);
        callback(data);
    }

    private void CreateCubeMesh(int x, int y, int z, Color c, bool l, bool r, bool t, bool b, bool n, bool f)
    {
        // local block position
        Vector3 blockPosition = new Vector3(x, y, z);

        if (l)
        {
            MakeFace(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LBN + blockPosition,
                c
            );
        }

        if (r)
        {
            MakeFace(
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RBF + blockPosition,
                c
            );
        }

        if (t)
        {
            MakeFace(
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RTN + blockPosition,
                c
            );
        }

        if (b)
        {
            MakeFace(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RBF + blockPosition,
                c
            );
        }

        if (n)
        {
            MakeFace(
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                c
            );
        }

        if (f)
        {
            MakeFace(
                CubeMetrics.RBF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LBF + blockPosition,
                c
            );
        }
    }

    /// <summary>
    /// Make a fae of the cube using 4 points
    /// </summary>
    /// <param name="lb">Left Bottom</param>
    /// <param name="lt">Left Top</param>
    /// <param name="rt">Right Top</param>
    /// <param name="rb">Right Bottom</param>
    private void MakeFace(Vector3 lb, Vector3 lt, Vector3 rt, Vector3 rb, Color c)
    {
        int index = vertices.Count;
        vertices.Add(lb);
        vertices.Add(lt);
        vertices.Add(rt);
        vertices.Add(rb);

        colors.Add(c);
        colors.Add(c);
        colors.Add(c);
        colors.Add(c);

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);
    }
}
