using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorleyField : FieldGenerator
{
    [System.Serializable]
    public class Biome
    {
        public string name;

        [Range(0,1)]
        public double minTemperature;
        [Range(0,1)]
        public double maxTemperature;

        [Range(0,1)]
        public double minMoisture;
        [Range(0,1)]
        public double maxMoisture;

        public Simplex heightMap;
        public double minHeight;
        public double maxHeight;
        public Blocks.Type type;

        public double Sample(double x, double z)
        {
            return heightMap.Sample(x, z).Remap(-1, 1, minHeight, maxHeight);
        }
    }

    public Biome[] biomes;

    public Worley worley;
    public SimplexVector2 distort;

    public Simplex temperatureMap;
    public Simplex moistureMap;

    private SafeDictionary<Vector3, Biome> biomeCache = new SafeDictionary<Vector3, Biome>();

    private System.Random rnd = new System.Random();

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var offset = distort.Sample(x, z);
            var samplePoint = ws + new Vector3(offset.x, 0, offset.y);
            var seedPoint = worley.GetClosestPoint(ws);

            Biome biome = null;

            if (biomeCache.ContainsKey(seedPoint))
            {
                biome = biomeCache.Get(seedPoint);
            }
            else
            {
                biome = GetBiome(seedPoint);
                biomeCache.Add(seedPoint, biome);
            }
            
            if (biome != null)
            {
                var height = biome.Sample(ws.x, ws.z);

                var maxY = Mathf.RoundToInt((float)height);

                for (int y = 0; y < maxY; ++y)
                {
                    field.Set(x, y, z, biome.type.ToByte());
                }
            }
            else
            {
                field.Set(x, 0, z, Blocks.Type.Land.ToByte());
            }
        });
    }

    private Biome GetBiome(Vector3 seedPoint)
    {
        var temperature = temperatureMap.Sample(seedPoint.x, seedPoint.z).Remap(-1, 1, 0, 1);
        var moisture = moistureMap.Sample(seedPoint.x, seedPoint.z).Remap(-1, 1, 0, 1);

        Biome selected = null;

        int j = rnd.Next(0, int.MaxValue) % biomes.Length;
        
        for (int i = 0; i < biomes.Length; ++i)
        {
            var biome = biomes[(j + i) % biomes.Length];

            if (temperature >= biome.minTemperature && temperature <= biome.maxTemperature)
            {
                if (moisture >= biome.minMoisture && moisture <= biome.maxMoisture)
                {
                    selected = biome;
                    break;
                }
            }
        }

        return selected;
    } 
}
