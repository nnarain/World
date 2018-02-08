using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMetrics
{
    private Mesh mesh;

    public const float CUBE_SIZE = 1f;
    public const float HALF_CUBE_SIZE = CUBE_SIZE / 2f;

    // l - left
    // r - right
    // t - top
    // b - bottom
    // n - near
    // f - far
    public static readonly Vector3 LBN = new Vector3(0, 0, 0); // lbn 0
    public static readonly Vector3 LBF = new Vector3(0, 0, CUBE_SIZE);  // lbf 1
    public static readonly Vector3 RBF = new Vector3(CUBE_SIZE, 0, CUBE_SIZE);   // rbf 2
    public static readonly Vector3 RBN = new Vector3(CUBE_SIZE, 0, 0);  // rbn 3
    public static readonly Vector3 LTN = new Vector3(0, CUBE_SIZE, 0);  // ltn 4
    public static readonly Vector3 LTF = new Vector3(0, CUBE_SIZE, CUBE_SIZE);   // ltf 5
    public static readonly Vector3 RTF = new Vector3(CUBE_SIZE, CUBE_SIZE, CUBE_SIZE);    // rtf 6
    public static readonly Vector3 RTN = new Vector3(CUBE_SIZE, CUBE_SIZE, 0);    // rtn 7

    public static readonly Vector3[] cubeVertices =
    {
        LBN, LBF, RBF, RBN, LTN, LTF, RTF, RTN
    };
}
