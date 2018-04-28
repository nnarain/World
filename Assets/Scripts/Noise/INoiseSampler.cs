using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public interface INoiseSampler
{
    float Sample(float x, float y);
    float Sample(float x, float y, float z);
}
