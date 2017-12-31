using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovement();
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
        Vector3 position = transform.position;
        position += new Vector3(xDelta, 0, zDelta) * Time.deltaTime * speed;

        transform.position = position;
    }
}
