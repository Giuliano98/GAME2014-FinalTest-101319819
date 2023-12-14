using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlatform : MonoBehaviour
{
    private Vector3 originalScale;
    public bool isPlayerOnPlatform = false;

    public float moveDistance = 1.0f;
    public float moveSpeed = 1.0f;

    public float ShrinkingTime = 2f;
    public float ResizedTime = 1.5f;

    private Collider2D platformCollider;
    private SpriteRenderer platformRenderer;
    private float initialY;

    private void Start()
    {
        originalScale = transform.localScale;
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();
        initialY = transform.position.y;
    }

    private void Update()
    {
        float newY = initialY + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        UpdateScale();
    }

    private void UpdateScale()
    {
        float scalePercentage = CalculateScalePercentage();
        //TODO: Illusion for platform disappearing 
        if (scalePercentage <= 0.02f)
        {
            platformCollider.enabled = false;
            platformRenderer.enabled = false;
        }
        else if (scalePercentage >= 0.06f)
        {
            platformCollider.enabled = true;
            platformRenderer.enabled = true;
        }

        transform.localScale = originalScale * scalePercentage; // Set current scale
    }

    private float CalculateScalePercentage()
    {
        float newScale;
        if (isPlayerOnPlatform)
            newScale = Mathf.Max(0.01f, transform.localScale.y - Time.deltaTime / ShrinkingTime);
        else
            newScale = Mathf.Min(1.0f, transform.localScale.y + Time.deltaTime / ResizedTime);

        return newScale;
    }

}
