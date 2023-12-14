/**********************************************
* Source File: NewPlatform.cs
* Student Name: Giuliano Venturo Gonzales
* Student ID: 101319819
* Program Description: trigger checking for platform
*
**********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckTrigger : MonoBehaviour
{
    public AudioSource src;
    public AudioClip Shirking, Resized;

    //TODO: Triggers for the child gameobject in the platform and sfx
    public NewPlatform platform;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            src.clip = Shirking;
            src.Play();
            platform.isPlayerOnPlatform = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            src.clip = Resized;
            src.Play();
            platform.isPlayerOnPlatform = false;
        }
    }
}
