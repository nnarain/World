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
        Sand,
        Grass1,
        Grass2,
        Dirt,
        Stone,
        Snow
    }

    public static byte ToByte(this Type t)
    {
        return (byte)t;
    }

    //...
    // Various property getters
    // ...
}
