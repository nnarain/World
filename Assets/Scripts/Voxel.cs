using System;
using System.Collections.Generic;
using UnityEngine;

struct Voxel
{
    enum Type
    {
        Air,
        Water,
        Sand,
        Dirt,
        Grass,
        Stone,
        Snow
    }

    Type type;
}
