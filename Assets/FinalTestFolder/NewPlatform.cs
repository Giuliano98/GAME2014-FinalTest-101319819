/**********************************************
* Source File: NewPlatform.cs
* Student Name: Giuliano Venturo Gonzales
* Student ID: 101319819
* Program Description: vertical movement and shirinking logic
*
**********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlatform : MonoBehaviour
{
    public bool isPlayerOnPlatform = false;

    [Header("Movement")]
    public float moveDistance = 1.0f;
    public float moveSpeed = 1.0f;
    [Header("Shrinking")]
    public float ShrinkingTime = 2f;
    public float ResizedTime = 1.5f;

    Vector3 originalScale;
    Collider2D platformCollider;
    SpriteRenderer platformRenderer;
    float initialY;

    private void Start()
    {
        originalScale = transform.localScale;
        platformCollider = GetComponent<Collider2D>();
        platformRenderer = GetComponent<SpriteRenderer>();
        initialY = transform.position.y;
    }

    private void Update()
    {
        // TODO: Vertical movement
        float newY = initialY + Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        //TODO: Logic for shrinking
        UpdateScale();
    }

    private void UpdateScale()
    {
        //TODO: Set Min and Max for the scale multiplayer 0.01f to 1f
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
