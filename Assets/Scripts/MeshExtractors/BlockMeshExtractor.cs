using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate a cube mesh from a field
/// </summary>
public class BlockMeshExtractor : IMeshExtractor
{
    private MeshDataAllocator meshAllocator;

    public BlockMeshExtractor()
    {
        meshAllocator = new MeshDataAllocator();
    }

    void IMeshExtractor.Extract(Chunk chunk, Action<MeshData[]> callback)
    {
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

                        CreateCubeMesh(x, y, z, (int)block.Type, chunk.GetFaces(block.Type), l, r, t, b, n, f);
                    }
                }
            }
        }

        callback(meshAllocator.Get());
    }

    private void CreateCubeMesh(int x, int y, int z, int type, TextureAtlas.BlockFaces faces, bool l, bool r, bool t, bool b, bool n, bool f)
    {
        // local block position
        Vector3 blockPosition = new Vector3(x, y, z);

        int meshIndex = Blocks.BlockMeshProperty[type];

        if (l)
        {
            meshAllocator.Get(meshIndex).AddQuad(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LBN + blockPosition,
                faces.GetUVs(Direction.Left)[3]
            );
        }

        if (r)
        {
            meshAllocator.Get(meshIndex).AddQuad(
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RBF + blockPosition,
                faces.GetUVs(Direction.Right)[3]
            );
        }

        if (t)
        {
            meshAllocator.Get(meshIndex).AddQuad(
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RTN + blockPosition,
                faces.GetUVs(Direction.Top)[3]
            );
        }

        if (b)
        {
            meshAllocator.Get(meshIndex).AddQuad(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RBF + blockPosition,
                faces.GetUVs(Direction.Bottom)[3]
            );
        }

        if (n)
        {
            meshAllocator.Get(meshIndex).AddQuad(
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                faces.GetUVs(Direction.Near)[3]
            );
        }

        if (f)
        {
            meshAllocator.Get(meshIndex).AddQuad(
                CubeMetrics.RBF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LBF + blockPosition,
                faces.GetUVs(Direction.Far)[3]
            );
        }
    }
}
