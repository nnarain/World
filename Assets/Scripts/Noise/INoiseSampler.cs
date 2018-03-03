using System;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public interface INoiseSampler
{
    double Sample(double x, double y);
    double Sample(double x, double y, double z);
}
