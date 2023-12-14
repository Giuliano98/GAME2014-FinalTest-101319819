/**********************************************
* Source File: NewPlatform.cs
* Student Name: Giuliano Venturo Gonzales
* Student ID: 101319819
* Program Description: overlap event for diamond
*
**********************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}
