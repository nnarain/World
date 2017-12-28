using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/// <summary>
/// Directions of chunk neighbors
/// </summary>
public enum Direction
{
    Left, Right, Top, Bottom, Near, Far
}

public static class DirectionExtension
{
    public static int ToInt(this Direction direction)
    {
        return (int)direction;
    }
}