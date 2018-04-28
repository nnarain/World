using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class NoiseViz : MonoBehaviour
{
    private Action updateTexture = null;

    private bool updated = true;
    public bool Updated { get { return updated; } }

    public virtual Field CreateNoiseMap(int width, int height)
    {
        updated = true;
        return null;
    }

    public void SetOnValidateCallback(Action f)
    {
        updateTexture = f;
    }

    protected void OnValidate()
    {
        //updated = true;

        
        if (updateTexture != null)
        {
            updateTexture();
        }
        else
        {
            Debug.Log("Not initialized");
        }
        
    }
}
