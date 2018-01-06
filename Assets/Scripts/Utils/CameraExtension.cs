using System;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtension
{
    /// <summary>
    /// Get the camera view frustum corners translated to the cameras world position
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static CameraFrustum GetFrustum(this Camera camera)
    {
        Vector3[] nearCorners = new Vector3[4];
        Vector3[] farCorners = new Vector3[4];

        Rect rect = new Rect(0, 0, 1, 1);
        camera.CalculateFrustumCorners(rect, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
        camera.CalculateFrustumCorners(rect, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farCorners);

        for (int i = 0; i < 4; ++i)
        {
            nearCorners[i] = camera.transform.localToWorldMatrix.MultiplyVector(nearCorners[i]);
            farCorners[i] = camera.transform.localToWorldMatrix.MultiplyVector(farCorners[i]);
        }

        return new CameraFrustum(nearCorners, farCorners);
    }

    public static bool IsPointInFrustum(this Camera camera, Vector3 point, float margin = 0.0f)
    {
        float min = 0 - margin;
        float max = 1 + margin;

        Vector3 viewportPoint = camera.WorldToViewportPoint(point);

        return viewportPoint.x >= min && viewportPoint.x <= max && viewportPoint.y >= min && viewportPoint.y <= max && viewportPoint.z > 0;
    }
}
