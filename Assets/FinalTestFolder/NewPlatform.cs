using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlatform : MonoBehaviour
{
    private Vector3 originalScale;
    private float scalePercentage = 1.0f;
    public bool isPlayerOnPlatform = false;

    public float moveDistance = 1.0f;
    public float moveSpeed = 1.0f;
    public Rigidbody2D playerRigidbody; // Reference to the player's Rigidbody2D

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (isPlayerOnPlatform)
        {
            scalePercentage = Mathf.Clamp01(scalePercentage - Time.deltaTime * 0.5f);

            if (scalePercentage <= 0.01f)
            {
                HandleShrinking();
            }
        }
        else
        {
            scalePercentage = Mathf.Clamp01(scalePercentage + Time.deltaTime * 2.0f);

            if (scalePercentage >= 0.99f)
            {
                HandleRestoring();
            }
        }

        transform.localScale = originalScale * scalePercentage; // Set the scale based on percentage
    }

    private void HandleShrinking()
    {
        GetComponent<Collider2D>().enabled = false; // Disable collision

        // Adjust player's gravity scale to counteract the jump
        playerRigidbody.gravityScale *= 0.5f; // Adjust the multiplier as needed
    }

    private void HandleRestoring()
    {
        GetComponent<Collider2D>().enabled = true; // Enable collision

        // Revert player's gravity scale to normal
        playerRigidbody.gravityScale = 1.0f;
    }
}
