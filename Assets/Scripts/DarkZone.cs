using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DarkZone : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tubs"))
        {
            Debug.Log("In Dark");
            other.gameObject.GetComponent<GhostCompanion>().isInDark = true;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lena is in the dark");
            other.gameObject.GetComponent<PlayerMovementPlatformer>().isInDark = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tubs"))
        {
            Debug.Log("Out of Dark");
            other.gameObject.GetComponent<GhostCompanion>().isInDark = false;
        }
        else if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Lena is out of the dark");
            other.gameObject.GetComponent<PlayerMovementPlatformer>().isInDark = false;
        }
    }
}
