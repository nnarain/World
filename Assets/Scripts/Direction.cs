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

    public static Direction Opposite(this Direction direction)
    {
        switch(direction)
        {
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
            case Direction.Near:
                return Direction.Far;
            case Direction.Far:
                return Direction.Near;
            default:
                return default(Direction);
        }
    }
}