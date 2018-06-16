using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGenerator : MonoBehaviour
{
    public virtual void Initialize()
    {
        // ...
    }

    public virtual void Generate(VoxelField field, Vector3 position)
    {
        // TODO: Throw exception
    }

    public virtual Color GetVoxelColor(byte type)
    {
        return Color.black;
    }
}
