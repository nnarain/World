using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class NoiseVizManager : MonoBehaviour
{
    public NoiseViz viz;

    public int width;
    public int height;

    private Texture2D texture;
    private bool initialized = false;

    private void OnEnable()
    {
        Debug.Log("On Enable");

        //Debug.Log("Setting callback");
        viz.SetOnValidateCallback(UpdateTexture);

        var renderer = GetComponent<MeshRenderer>();

        texture = new Texture2D(width, height);
        texture.name = "Noise Viz";
        texture.filterMode = FilterMode.Bilinear;

        renderer.sharedMaterial.mainTexture = texture;

        initialized = true;

    }


    public void UpdateTexture()
    {
        GenerateTexture(width, height);
    }

    private void GenerateTexture(int width, int height)
    {
        Debug.Log("Generating Texture");


        var noiseMap = CreateNoiseMap(width, height);
        texture.SetPixels(noiseMap);

        texture.Apply();
    }

    private Color[] CreateNoiseMap(int width, int height)
    {
        var noiseMap = viz.CreateNoiseMap(width, height);

        Color[] colors = new Color[width * height];

        for (int x = 0; x < width; ++x)
        {
            for (int z = 0; z < height; ++z)
            {
                var sample = (float)noiseMap.Get(x, 0, z);
                colors[z * width + x] = new Color(sample, sample, sample);
            }
        }

        return colors;
    }


    private void OnValidate()
    {
        if (width <= 0) width = 1;
        if (height <= 0) height = 1;

        Debug.Log("On Validate");

        if (initialized)
        {
            UpdateTexture();
        }
    }
}
