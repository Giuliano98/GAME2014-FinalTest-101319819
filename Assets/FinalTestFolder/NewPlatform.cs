using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlatform : MonoBehaviour
{
    private Vector3 originalScale;
    public float moveDistance = 1.0f;
    public float moveSpeed = 1.0f;

    private bool isShrinking = false;
    private bool isRestoring = false;
    private float shrinkTimer = 0f;
    private const float shrinkDuration = 1.6f;

    private void Start()
    {
        originalScale = transform.localScale;
    }

    private void Update()
    {
        float newY = Mathf.Sin(Time.time * moveSpeed) * moveDistance;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        if (isShrinking)
        {
            ShrinkPlatform();
        }
        else if (isRestoring)
        {
            RestorePlatform();
        }
    }

    public void StartShrinking()
    {
        isShrinking = true;
        isRestoring = false;
    }

    public void StartRestoring()
    {
        isShrinking = false;
        isRestoring = true;
    }

    private void ShrinkPlatform()
    {
        shrinkTimer += Time.deltaTime;
        float t = Mathf.Clamp01(shrinkTimer / shrinkDuration);
        transform.localScale = Vector3.Lerp(originalScale, Vector3.zero, t);

        if (t >= 1f)
        {
            Invoke(nameof(StartRestoring), 2f);
        }
    }

    private void RestorePlatform()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * 2f);

        if (Vector3.Distance(transform.localScale, originalScale) < 0.01f)
        {
            transform.localScale = originalScale;
            isRestoring = false;
        }
    }
}
