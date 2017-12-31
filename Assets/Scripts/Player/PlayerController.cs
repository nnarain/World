using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float movementSpeed = 1f;
    public float rotationSpeed = 1f;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleElevation();
    }

    private void HandleMovement()
    {
        float xDelta = Input.GetAxis("Horizontal");
        float zDelta = Input.GetAxis("Vertical");

        if (xDelta != 0f || zDelta != 0f)
        {
            AdjustPosition(xDelta, zDelta);
        }
    }

    private void AdjustPosition(float xDelta, float zDelta)
    {
        float xSpeed = xDelta * movementSpeed * Time.deltaTime;
        float zSpeed = zDelta * movementSpeed * Time.deltaTime;

        transform.Translate(xDelta, 0, zDelta);
    }

    private void HandleRotation()
    {
        float xDelta = Input.GetAxis("Rotation");

        if (xDelta != 0f)
        {
            transform.Rotate(0, xDelta * rotationSpeed * Time.deltaTime, 0);
        }
    }

    private void HandleElevation()
    {
        float yDelta = Input.GetAxis("Elevation");

        if (yDelta != 0f)
        {
            transform.Translate(0, yDelta * movementSpeed * Time.deltaTime, 0);
        }
    }

    private void OnValidate()
    {
    }
}
