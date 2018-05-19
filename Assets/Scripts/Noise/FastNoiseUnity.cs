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

    public float scale = 1;

    private FastNoise noise = new FastNoise();
    FastNoise Noise { get { return noise; } }

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

    public void SetCellularNoiseLookup(FastNoiseUnity noise)
    {
        this.noise.SetCellularNoiseLookup(noise.Noise);
    }

    public float Sample(float x, float y)
    {
        return noise.GetNoise(x / scale, y / scale);
    }

    public float Sample(float x, float y, float z)
    {
        return noise.GetNoise(x / scale, y / scale, z / scale);
    }
}
