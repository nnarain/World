using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplexTerrain : FieldGenerator
{
    public FastNoiseUnity baseTerrain;
    public float baseMaxHeight;
    public float baseE;

    public FastNoiseUnity mountainTerrain;
    public float mountainMaxHeight;
    public float mountainLevel;
    public float mountainE;

    public FastNoiseUnity interp;

    public int seaLevel;

    [Range(0, 1)]
    public float baseThreshold;
    [Range(0, 1)]
    public float mountainThreshold;

    public enum BlockType
    {
        Air,
        Water,
        Grass,
        Stone
    }

    public enum Biome
    {
        Tundra,
        BorealForest,
        BarrenSubarctic,
        TemperateForest,
        Savanna,
        Grassland,
        Desert,
        Rainforest
    }

    public Color[] colors;

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var height = GetHeight(ws.x, ws.z);

            int maxY = (int)height;

            if (maxY >= (field.Y - 1)) maxY = field.Y - 1;

            if (maxY >= seaLevel)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    if (height >= mountainLevel)
                    {
                        field.Set(x, y, z, (byte)BlockType.Stone);
                    }
                    else
                    {
                        field.Set(x, y, z, (byte)BlockType.Grass);
                    }
                }
            }
            else
            {
                for (int y = 0; y < seaLevel; ++y)
                {
                    field.Set(x, y, z, (byte)BlockType.Water);
                }
            }
        });
    }

    private float GetHeight(float x, float y)
    {
        var baseTerrainSample = Mathf.Pow(baseTerrain.Sample(x, y).Remap(-1, 1, 0, 1), baseE);

        if (baseTerrainSample < baseThreshold)
        {
            return baseTerrainSample * baseMaxHeight;
        }
        else
        {
            var mountainTerrainSample = Mathf.Pow(mountainTerrain.Sample(x, y).Remap(-1, 1, 0, 1), mountainE);

            if (baseTerrainSample < mountainThreshold)
            {
                return BlendSamples(baseTerrainSample, baseMaxHeight, baseThreshold, mountainThreshold, mountainTerrainSample * mountainMaxHeight);
            }
            else
            {
                return mountainTerrainSample * mountainMaxHeight;
            }
        }
    }

    public float BlendSamples(float baseSample, float maxBaseHeight, float t1, float t2, float mountainHeight)
    {
        // The base sample is in the range [t1, t2]
        // So remap the sample to range [0,1]
        var t = baseSample.Remap(t1, t2, 0, 1);

        var height = Mathf.Lerp(baseSample * maxBaseHeight, mountainHeight, t);

        return height;
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
        baseTerrain.Update();

        mountainTerrain.Update();
    }
}
