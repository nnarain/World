using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexTerrain : FieldGenerator
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
    public Simplex moisture;
    public Simplex blockDistort;

    public double maxHeight;
    public double islandRadius;
    public double seaLevel;
    public double sandLevel;
    public double landLevel;
    public double stoneLevel;
    [Range(0,1)]
    public double grassMoisture;
    [Range(0, 1)]
    public double rainforestMoisture;


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

            var islandMask = IslandMask(ws);
            var height = (selection.Sample(ws.x, ws.z)).Remap(-1, 1, 0, 1) * maxHeight * islandMask;

            int maxY = Mathf.RoundToInt(Mathf.Clamp((float)height, 0, field.Y - 1));

            var blockOffset = blockDistort.Sample(ws.x, ws.z);

            

            for (int y = 0; y < field.Y; ++y)
            {
                ws.y = position.y + y;

                var density = (double)(-y + maxY);
                //density += sampler.Sample(x, y, z);
                //density += Saturate((midLevel - ws.y) * 3) * 40;

                if (density > 0)
                {
                    var blockY = ws.y + blockOffset;

                    if (ws.y <= seaLevel + sandLevel)
                    {
                        field.Set(x, y, z, Blocks.Type.Sand.ToByte());
                    }
                    else if (blockY <= seaLevel + landLevel)
                    {
                        var wetness = GetMoisture(ws);

                        

                        if (wetness <= grassMoisture)
                        {
                            field.Set(x, y, z, Blocks.Type.Land.ToByte());
                        }
                        else if (wetness >= grassMoisture && wetness < rainforestMoisture)
                        {
                            field.Set(x, y, z, Blocks.Type.GrassLand.ToByte());
                        }
                        else
                        {
                            field.Set(x, y, z, Blocks.Type.RainForest.ToByte());
                        }
                    }
                    else
                    {
                        field.Set(x, y, z, Blocks.Type.Stone.ToByte());
                    }
                }
                else
                {
                    if (ws.y <= seaLevel)
                    {
                        field.Set(x, y, z, Blocks.Type.Water.ToByte());
                    }
                }
            }
        });
    }

    private double GetMoisture(Vector3 ws)
    {
        return moisture.Sample(ws.x, ws.z).Remap(-1, 1, 0, 1) * PercentOfIslandCenter(ws);
    }

    private double IslandMask(Vector3 ws)
    {
        return InverseIslandCenter(ws);
    }

    private double InverseIslandCenter(Vector3 ws)
    {
        return 1.0 - PercentOfIslandCenter(ws);
    }

    private double PercentOfIslandCenter(Vector3 ws)
    {
        double d = ws.magnitude;
        return d / islandRadius;
    }

    private static double Saturate(double a)
    {
        return Mathf.Max(0, (float)a);
    }
}
