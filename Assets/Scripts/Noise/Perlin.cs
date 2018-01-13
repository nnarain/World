using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Perlin
{
    public int seed;
    public float scale;
    [Range(1, 100)]
    public int octaves;
    [Range(0, 1)]
    public float persistance;
    [Range(1, 2)]
    public float lacunarity;

    public float[,] Generate(int width, int height, Vector2 offset)
    {
        return PerlinNoise.Generate(width, height, seed, scale, octaves, persistance, lacunarity, offset);
    }
}
