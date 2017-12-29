using System;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] Generate(int width, int height, float scale, int octaves, float persistance, float lacunarity)
    {
        float[,] noise = new float[width, height];

        if (scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                float amplitude = 1f;
                float frequency = 1f;
                float noiseHeight = 0;

                for (int i = 0; i < octaves; ++i)
                {
                    float sampleX = (float)x / scale * frequency;
                    float sampleY = (float)y / scale * frequency;

                    float perlin = Mathf.PerlinNoise(sampleX, sampleY);
                    noiseHeight += perlin * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

                noise[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < height; ++y)
        {
            for (int x = 0; x < width; ++x)
            {
                noise[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noise[x, y]);
            }
        }

        return noise;
    }
}
