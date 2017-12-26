using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeMesh : MonoBehaviour
{
    private Mesh mesh;

    private const float CUBE_SIZE = 1f;
    private const float HALF_CUBE_SIZE = CUBE_SIZE / 2f;

    private static readonly Vector3[] cubeVertices =
    {
        // t b l r v f
        new Vector3(-HALF_CUBE_SIZE, -HALF_CUBE_SIZE, -HALF_CUBE_SIZE), // lbn 0
        new Vector3(-HALF_CUBE_SIZE, -HALF_CUBE_SIZE, HALF_CUBE_SIZE),  // lbf 1
        new Vector3(HALF_CUBE_SIZE, -HALF_CUBE_SIZE, HALF_CUBE_SIZE),   // rbf 2
        new Vector3(HALF_CUBE_SIZE, -HALF_CUBE_SIZE, -HALF_CUBE_SIZE),  // rbn 3
        new Vector3(-HALF_CUBE_SIZE, HALF_CUBE_SIZE, -HALF_CUBE_SIZE),  // ltn 4
        new Vector3(-HALF_CUBE_SIZE, HALF_CUBE_SIZE, HALF_CUBE_SIZE),   // ltf 5
        new Vector3(HALF_CUBE_SIZE, HALF_CUBE_SIZE, HALF_CUBE_SIZE),    // rtf 6
        new Vector3(HALF_CUBE_SIZE, HALF_CUBE_SIZE, -HALF_CUBE_SIZE)    // rtn 7
    };

    private const int lbn = 0;
    private const int lbf = 1;
    private const int rbf = 2;
    private const int rbn = 3;
    private const int ltn = 4;
    private const int ltf = 5;
    private const int rtf = 6;
    private const int rtn = 7;

    private static readonly int[] faces =
    {
        // near
        lbn, ltn, rtn,
        lbn, rtn, rbn,

        // far
        rbf, rtf, ltf,
        rbf, ltf, lbf,

        // left
        lbf, ltn, lbn,
        lbf, ltf, ltn,

        // right
        rbn, rtn, rtf,
        rbn, rtf, rbf,

        // top
        ltn, ltf, rtf,
        ltn, rtf, rtn,

        // bottom
        lbf, lbn, rbn,
        lbf, rbn, rbf
    };

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    private void Start()
    {
        List<Vector3> vertices = new List<Vector3>();

        foreach (var v in  cubeVertices)
        {
            vertices.Add(v);
        }

        List<int> indices = new List<int>();
        
        foreach(int i in faces)
        {
            indices.Add(i);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = indices.ToArray();

        mesh.RecalculateNormals();
    }
}
