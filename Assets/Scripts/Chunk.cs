using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> triangles;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    /// <summary>
    /// Build a mesh from the 3D grid.
    /// </summary>
    /// <param name="grid"></param>
    public void Build(Grid grid)
    {
        vertices.Clear();
        triangles.Clear();

        for (int x = 0; x < grid.X; ++x)
        {
            for (int y = 0; y < grid.Y; ++y)
            {
                for (int z = 0; z < grid.Z; ++z)
                {
                    int block = grid.Get(x, y, z);

                    if (block != 0)
                    {
                        // get neighbor states and determine which faces should be constructed
                        bool l = grid.Get(x - 1, y, z) <= 0;
                        bool r = grid.Get(x + 1, y, z) <= 0;
                        bool t = grid.Get(x, y + 1, z) <= 0;
                        bool b = grid.Get(x, y - 1, z) <= 0;
                        bool n = grid.Get(x, y, z - 1) <= 0;
                        bool f = grid.Get(x, y, z + 1) <= 0;

                        CreateCubeMesh(x, y, z, l, r, t, b, n, f);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();

        mesh.RecalculateNormals();
    }

    private void CreateCubeMesh(int x, int y, int z, bool l, bool r, bool t, bool b, bool n, bool f)
    {
        // the the world space offset of the chunk
        Vector3 chunkOffset = transform.TransformPoint(transform.localPosition);
        // local block position
        Vector3 blockPosition = new Vector3(x, y, z);

        if (l)
        {
            MakeFace(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LBN + blockPosition
            );
        }

        if (r)
        {
            MakeFace(
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RBF + blockPosition
            );
        }

        if (t)
        {
            MakeFace(
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.RTN + blockPosition
            );
        }

        if (b)
        {
            MakeFace(
                CubeMetrics.LBF + blockPosition,
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.RBN + blockPosition,
                CubeMetrics.RBF + blockPosition
            );
        }
        
        if (n)
        {
            MakeFace(
                CubeMetrics.LBN + blockPosition,
                CubeMetrics.LTN + blockPosition,
                CubeMetrics.RTN + blockPosition,
                CubeMetrics.RBN + blockPosition
            );
        }

        if (f)
        {
            MakeFace(
                CubeMetrics.RBF + blockPosition,
                CubeMetrics.RTF + blockPosition,
                CubeMetrics.LTF + blockPosition,
                CubeMetrics.LBF + blockPosition
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
    private void MakeFace(Vector3 lb, Vector3 lt, Vector3 rt, Vector3 rb)
    {
        int index = vertices.Count;
        vertices.Add(lb);
        vertices.Add(lt);
        vertices.Add(rt);
        vertices.Add(rb);

        triangles.Add(index + 0);
        triangles.Add(index + 1);
        triangles.Add(index + 2);

        triangles.Add(index + 0);
        triangles.Add(index + 2);
        triangles.Add(index + 3);
    }
}
