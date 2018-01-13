using System;
using System.Collections.Generic;
using UnityEngine;

public static class Vector3Extensions
{
    public static Vector2 ToXZ(this Vector3 v)
    {
        return new Vector2(v.x, v.z);
    }
}
