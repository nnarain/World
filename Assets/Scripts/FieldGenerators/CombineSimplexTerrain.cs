using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombineSimplexTerrain : FieldGenerator
{
    [System.Serializable]
    public class HeightMap
    {
        public Simplex continent;
        public Simplex hill;

        public Worley variation;
    }

    public enum HeightSelector
    {
        Continent,
        Hill,
        Variation,
        Combined
    }

    public HeightSelector selector;


    public HeightMap heightMap;

    public double maxHeight;

    public override void Generate(VoxelField field, Vector3 position)
    {
        CombinedNoise terrainHeight = new CombinedNoise(heightMap.continent, heightMap.hill, heightMap.variation);

        INoiseSampler selection = null;
        if (selector == HeightSelector.Continent)
            selection = heightMap.continent;
        else if (selector == HeightSelector.Hill)
            selection = heightMap.hill;
        else if (selector == HeightSelector.Variation)
            selection = heightMap.variation;
        else
            selection = terrainHeight;

        field.ForEachXZ((x, z) =>
        {
            Vector3 ws = position + new Vector3(x, 0, z);

            var height = (selection.Sample(ws.x, ws.z)).Remap(-1, 1, 0, 1) * maxHeight;

            int maxY = (int)Mathf.Clamp((float)height, 0, field.Y - 1);

            for (int y = 0; y < maxY; ++y)
            {
                field.Set(x, y, z, Blocks.Type.Stone.ToByte());
            }
        });
    }

    double AddOctaves(Simplex n, Vector3 ws, double f, double a, int octaves)
    {
        double total = 0;
        for (int i = 0; i < octaves; ++i)
        {
            total += n.Sample(ws, f, a, i);
        }

        return total;
    }

    double AddOctaves(Simplex n, double x, double z, double f, double a, int octaves)
    {
        double total = 0;
        for (int i = 0; i < octaves; ++i)
        {
            total += n.Sample(x, z, f, a, i);
        }

        return total;
    }

    private static double Saturate(double a)
    {
        return Mathf.Max(0, (float)a);
    }
}
