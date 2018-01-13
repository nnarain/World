using System;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationCurveExtension
{
    public static AnimationCurve Copy(this AnimationCurve curve)
    {
        return new AnimationCurve(curve.keys);
    }
}
