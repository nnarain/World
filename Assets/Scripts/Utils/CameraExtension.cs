using System;
using System.Collections.Generic;
using UnityEngine;

public static class CameraExtension
{
    /// <summary>
    /// Get the camera view frustum corners
    /// </summary>
    /// <param name="camera"></param>
    /// <returns></returns>
    public static CameraFrustum GetFrustum(this Camera camera)
    {
        Vector3[] nearCorners = new Vector3[4];
        Vector3[] farCorners = new Vector3[4];

        if (camera == null)
        {
            Debug.Log("Camera is null somehow");
        }

        Rect rect = new Rect(0, 0, 1, 1);
        camera.CalculateFrustumCorners(rect, camera.nearClipPlane, Camera.MonoOrStereoscopicEye.Mono, nearCorners);
        camera.CalculateFrustumCorners(rect, camera.farClipPlane, Camera.MonoOrStereoscopicEye.Mono, farCorners);

        return new CameraFrustum(nearCorners, farCorners);
    }
}
