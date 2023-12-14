using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    public NewPlatform platform;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            platform.isPlayerOnPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            platform.isPlayerOnPlatform = false;
        }
    }
}
