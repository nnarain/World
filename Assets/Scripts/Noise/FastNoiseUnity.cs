using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FastNoiseUnity : INoiseSampler
{
    public FastNoise.NoiseType noiseType;
    public FastNoise.FractalType fractalType;
    public FastNoise.Interp interp;

    public FastNoise.CellularDistanceFunction cellularDistanceFunction;
    public FastNoise.CellularReturnType cellularReturnType;

    public int seed = 1337;
    [Range(0, 10)]
    public int octaves = 1;
    public float frequency = 0.01f;
    [Range(0, 1)]
    public float gain = 0.5f;
    [Range(1, 2)]
    public float lacunarity = 2.0f;
    public float cellularJitter = 0.45f;

    private FastNoise noise = new FastNoise();

    public FastNoiseUnity()
    {
        Update();
    }

    public void Update()
    {
        noise.SetNoiseType(noiseType);
        noise.SetFractalType(fractalType);
        noise.SetInterp(interp);
        noise.SetCellularDistanceFunction(cellularDistanceFunction);
        noise.SetCellularReturnType(cellularReturnType);
        noise.SetSeed(seed);
        noise.SetFractalOctaves(octaves);
        noise.SetFrequency(frequency);
        noise.SetFractalGain(gain);
        noise.SetFractalLacunarity(lacunarity);
        noise.SetCellularJitter(cellularJitter);
    }

    public float Sample(float x, float y)
    {
        return noise.GetNoise(x, y);
    }

    public float Sample(float x, float y, float z)
    {
        return noise.GetNoise(x, y, z);
    }
}
