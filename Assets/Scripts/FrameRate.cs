using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameRate : MonoBehaviour
{
    public float refreshTime;

    private int frameCounter = 0;
    private float timeElapsed = 0.0f;
    private float currentFrameRate = 0;

    public float CurrentFrameRate { get { return currentFrameRate; } }

    // Update is called once per frame
    void Update()
    {
        if (timeElapsed < refreshTime)
        {
            // increment time and count frames
            timeElapsed += Time.smoothDeltaTime;
            frameCounter++;
        }
        else
        {
            // calculate frames in the refresh period
            currentFrameRate = (float)frameCounter / timeElapsed;

            // reset counters
            frameCounter = 0;
            timeElapsed = 0;
        }
    }

    private void OnValidate()
    {
        if (refreshTime <= 0)
        {
            refreshTime = 1.0f;
        }
    }
}
