using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleCombined : FieldGenerator
{
    [System.Serializable]
    public class HeightMap
    {
        public FastNoiseUnity height1;
        public FastNoiseUnity height2;

        public float height1Min;
        public float height1Max;
        public float height2Min;
        public float height2Max;

        [Range(-10, 10)]
        public float e1 = 1;
        [Range(-10, 10)]
        public float e2 = 1;

        public float Sample(float x, float y)
        {
            //float s1 = height1.Sample(x, y).Remap(-1, 1, 0, 1);
            var s1 = GetSample(height1, x, y, e1);

            if (s1 < t1)
            {
                return s1.Remap(0, 1, height1Min, height1Max);
            }
            else
            {
                //float s2 = height2.Sample(x, y).Remap(-1, 1, 0, 1);
                var s2 = GetSample(height2, x, y, e2);
                var h2 = s2.Remap(0, 1, height2Min, height2Max);

                if (s1 < t2)
                {
                    return blend(s1, h2);
                }
                else
                {
                    return h2;
                }
            }

            return 0;
        }

        private float blend(float s1, float h2)
        {
            var t = s1.Remap(t1, t2, 0, 1);

            var h1 = s1.Remap(0, 1, height1Min, height1Max);

            return Mathf.Lerp(h1, h2, t);
        }

        private float GetSample(FastNoiseUnity n, float x, float y, float e)
        {
            return Mathf.Pow(n.Sample(x, y).Remap(-1, 1, 0, 1), e);
        }

        public void Update()
        {
            height1.Update();
            height2.Update();
        }

        [Range(0, 1)]
        public float t1;
        [Range(0, 1)]
        public float t2;
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

    [System.Serializable]
    public class BiomeSelector
    {
        public FastNoiseUnity moistureMap;
        public FastNoiseUnity temperatureMap;

        [Range(0, 1)]
        public float m1;
        [Range(0, 1)]
        public float m2;
        [Range(0, 1)]
        public float m3;

        [Range(0, 1)]
        public float t1;
        [Range(0, 1)]
        public float t2;
        [Range(0, 1)]
        public float t3;

        public Biome Sample(float x, float y)
        {
            var moisture = moistureMap.Sample(x, y).Remap(-1, 1, 0, 1);
            var temperature = temperatureMap.Sample(x, y).Remap(-1, 1, 0, 1);

            if (temperature >= t1)
            {
                if (moisture >= m1)
                {
                    return Biome.Rainforest;
                }
                else if (moisture >= m2)
                {
                    return Biome.TemperateForest;
                }
                else if (moisture >= m3)
                {
                    return Biome.Grassland;
                }
                else
                {
                    return Biome.Desert;
                }
            }
            else if (temperature >= t2)
            {
                if (moisture >= m1)
                {
                    return Biome.TemperateForest;
                }
                else if (moisture >= m2)
                {
                    return Biome.Grassland;
                }
                else if (moisture >= m3)
                {
                    return Biome.Grassland;
                }
                else
                {
                    return Biome.Desert;
                }
            }
            else if (temperature >= t3)
            {
                if (moisture >= m2)
                {
                    return Biome.BorealForest;
                }
                else
                {
                    return Biome.BarrenSubarctic;
                }
            }
            else
            {
                return Biome.Tundra;
            }
        }

        public void Update()
        {
            moistureMap.Update();
            temperatureMap.Update();
        }
    }

    [System.Serializable]
    public class ThreasholdNoise
    {
        public float threshold;
        public FastNoiseUnity n;
        public float min;
        public float max;

        public float GetThreshold(float x, float y)
        {
            return threshold + n.Sample(x, y).Remap(-1, 1, min, max);
        }

        public void Update()
        {
            n.Update();
        }
    }

    [System.Serializable]
    public enum HeightSelection
    {
        Height1,
        Height2,
        Interp,
        Combinded
    }

    public HeightSelection selection;


    public HeightMap baseHeightMap;
    public HeightMap mountainHeightMap;
    public FastNoiseUnity interp;
    [Range(-10, 10)]
    public float interpE = 1;

    public float minHeight;
    public float maxHeight;

    public BiomeSelector biomeSelector;

    [Range(0, 1)]
    public float baseThreshold;
    [Range(0, 1)]
    public float mountainThreshold;

    //public float snowLevel;
    public ThreasholdNoise snowLevel;
    public ThreasholdNoise mountainLevel;
    public int seaLevel;

    public enum BlockType
    {
        Air,
        Water,
        TemperateGrass,
        RainforestGrass,
        BorealGrass,
        BarrenGrass,
        DryGrass,
        Sand,
        Dirt,
        Stone,
        Snow
    }

    public Color[] colors;

    public override void Generate(VoxelField field, Vector3 position)
    {
        field.ForEachXZ((x, z) =>
        {
            var ws = position + new Vector3(x, 0, z);

            var height = 0.0f;

            if (selection == HeightSelection.Height1)
            {
                height = baseHeightMap.Sample(ws.x, ws.z);
            }
            else if (selection == HeightSelection.Height2)
            {
                height = mountainHeightMap.Sample(ws.x, ws.z);
            }
            else if (selection == HeightSelection.Interp)
            {
                height = interp.Sample(ws.x, ws.z).Remap(-1, 1, 0, 1) * maxHeight;
            }
            else
            {
                height = GetHeight(ws.x, ws.z);
            }


            int maxY = (int)height;
            if (maxY >= (field.Y - 1)) maxY = field.Y - 1;

            var biome = biomeSelector.Sample(ws.x, ws.z);

            if (maxY >= seaLevel)
            {
                for (int y = 0; y < maxY; ++y)
                {
                    if (height >= snowLevel.GetThreshold(ws.x, ws.y))
                    {
                        field.Set(x, y, z, (byte)BlockType.Snow);
                    }
                    else if (height >= mountainLevel.GetThreshold(ws.x, ws.z))
                    {
                        field.Set(x, y, z, (byte)BlockType.Stone);
                    }
                    else
                    {
                        field.Set(x, y, z, (byte)GetBlockFromBiome(biome));
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
        var interpValue = interp.Sample(x, y).Remap(-1, 1, 0, 1);
        interpValue = Mathf.Pow(interpValue, interpE);

        if (interpValue < baseThreshold)
        {
            return baseHeightMap.Sample(x, y);
        }
        else
        {
            var h1 = baseHeightMap.Sample(x, y);
            var h2 = mountainHeightMap.Sample(x, y);

            if (interpValue < mountainThreshold)
            {
                var t = interpValue.Remap(baseThreshold, mountainThreshold, 0, 1);
                return Mathf.Lerp(h1, h2, t);
            }
            else
            {
                return h2;
            }
        }
    }

    BlockType GetBlockFromBiome(Biome biome)
    {
        switch(biome)
        {
            case Biome.Rainforest:
                return BlockType.RainforestGrass;
            case Biome.TemperateForest:
                return BlockType.TemperateGrass;
            case Biome.Grassland:
                return BlockType.DryGrass;
            case Biome.Savanna:
                return BlockType.DryGrass;
            case Biome.BorealForest:
                return BlockType.BorealGrass;
            case Biome.BarrenSubarctic:
                return BlockType.BarrenGrass;
            case Biome.Desert:
                return BlockType.Sand;
            case Biome.Tundra:
                return BlockType.Snow;
            default:
                return BlockType.TemperateGrass;
        }
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
        baseHeightMap.Update();
        mountainHeightMap.Update();
        interp.Update();
        biomeSelector.Update();
        snowLevel.Update();
        mountainLevel.Update();
    }
}
