using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyField : FieldGenerator
{
    public Worley worley;
    public double maxHeight;

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var height = worley.Sample(ws.x, ws.z) * maxHeight;
            height = Mathf.Clamp((float)height, 0, field.Y - 1);

            int maxY = Mathf.RoundToInt((float)height);

            for (int y  = 0; y < maxY; ++y)
            {
                field.Set(x, y, z, Blocks.Type.Stone.ToByte());
            }
        });
    }
}
