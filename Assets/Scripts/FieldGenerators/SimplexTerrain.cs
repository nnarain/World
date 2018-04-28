using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexTerrain : FieldGenerator
{
    public FastNoiseUnity noise;

    public float maxHeight;

    public enum BlockType
    {
        Air,
        Water,
        Stone
    }

    public Color[] colors;

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var sample = noise.Sample(ws.x, ws.z).Remap(-1.0f, 1.0f, 0.0f, maxHeight);

            int maxY = (int)sample;

            for (int y = 0; y < maxY; ++y)
            {
                if (y <= 20)
                {
                    field.Set(x, y, z, 1);
                }
                else
                {
                    field.Set(x, y, z, 2);
                }
            }

        });
    }

    public override Color GetVoxelColor(byte type)
    {
        if (type >= colors.Length)
        {
            return Color.black;
        }
        else
        {
            return colors[type];
        }
    }

    private void OnValidate()
    {
        noise.Update();
    }
}
