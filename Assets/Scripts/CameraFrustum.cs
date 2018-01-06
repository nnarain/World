using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraFrustum
{
    public enum Corner
    {
        LB, LT, RT, RB
    }

    public readonly Vector3[] nearPlaneCorners;
    public readonly Vector3[] farPlaneCorners;

    public CameraFrustum(Vector3[] n, Vector3[] f)
    {
        this.nearPlaneCorners = n;
        this.farPlaneCorners = f;
    }

    public Vector3 GetNearCorner(Corner c)
    {
        return nearPlaneCorners[c.ToInt()];
    }

    public Vector3 GetFarCorner(Corner c)
    {
        return farPlaneCorners[c.ToInt()];
    }
}

public static class CameraCornerExtension
{
    public static int ToInt(this CameraFrustum.Corner corner)
    {
        return (int)corner;
    }
}