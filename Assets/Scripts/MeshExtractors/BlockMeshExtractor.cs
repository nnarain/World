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
    private List<Vector2> texCoords;

    public BlockMeshExtractor()
    {
        vertices = new List<Vector3>();
        triangles = new List<int>();
        colors = new List<Color>();
        texCoords = new List<Vector2>();
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

                    if (block.Type != 0)
                    {
                        // get neighbor states and determine which faces should be constructed
                        bool l = chunk.GetField(x - 1, y, z).Type == 0;
                        bool r = chunk.GetField(x + 1, y, z).Type == 0;
                        bool t = chunk.GetField(x, y + 1, z).Type == 0;
                        bool b = chunk.GetField(x, y - 1, z).Type == 0;
                        bool n = chunk.GetField(x, y, z - 1).Type == 0;
                        bool f = chunk.GetField(x, y, z + 1).Type == 0;

                        CreateCubeMesh(x, y, z, chunk.GetFaces(block.Type), l, r, t, b, n, f);
                    }
                }
            }
        }

        MeshData data = new MeshData(vertices, triangles, colors, texCoords);
        callback(data);
    }

    private void CreateCubeMesh(int x, int y, int z, TextureAtlas.BlockFaces faces, bool l, bool r, bool t, bool b, bool n, bool f)
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
                faces.GetUVs(Direction.Left)
            );
        }

        if (r)
        {
            MakeFace(
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RBF + blockPosition,
                faces.GetUVs(Direction.Right)
            );
        }

        if (t)
        {
            MakeFace(
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RTN + blockPosition,
                faces.GetUVs(Direction.Top)
            );
        }

        if (b)
        {
            MakeFace(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RBF + blockPosition,
                faces.GetUVs(Direction.Bottom)
            );
        }

        if (n)
        {
            MakeFace(
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                faces.GetUVs(Direction.Near)
            );
        }

        if (f)
        {
            MakeFace(
                CubeMetrics.RBF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LBF + blockPosition,
                faces.GetUVs(Direction.Far)
            );
        }
    }

    /// <summary>
    /// Make a face of the cube using 4 points
    /// </summary>
    /// <param name="lb">Left Bottom</param>
    /// <param name="lt">Left Top</param>
    /// <param name="rt">Right Top</param>
    /// <param name="rb">Right Bottom</param>
    private void MakeFace(Vector3 lb, Vector3 lt, Vector3 rt, Vector3 rb, Vector2[] uvs)
    {
        int index = vertices.Count;
        vertices.Add(lb);
        vertices.Add(lt);
        vertices.Add(rt);
        vertices.Add(rb);

        texCoords.Add(uvs[0]);
        texCoords.Add(uvs[0]);
        texCoords.Add(uvs[0]);
        texCoords.Add(uvs[0]);

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);
    }
}
