using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class DoorExit : MonoBehaviour
{
    public UnityEvent exitEvents;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Exit();
        }
    }

    private void Exit()
    {
        exitEvents.Invoke();
    }
}
