using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Blocks
{
    public enum Type
    {
        Air,
        Water,
        Land,
        Stone,
        Snow,
        Sand,
        GrassLand,
        RainForest,
        Scorched
    }

    public static int[] BlockMeshProperty = new int[]
    {
        0,
        1,
        0,
        0, 
        0, 
        0,
        0,
        0,
        0
    };

    public static byte ToByte(this Type t)
    {
        return (byte)t;
    }

    //...
    // Various property getters
    // ...
}
