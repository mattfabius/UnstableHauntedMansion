using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider2D))]
public class LenaInteractable : MonoBehaviour
{
    public UnityEvent onInteract;

    public float interactTime = 1f;

    private bool playerIsInTrigger = false;

    private void Start()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void Update()
    {
        if (playerIsInTrigger && Input.GetButtonDown("Interact")) // OnTriggerStay2D drops inputs
        {
            Interact();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerIsInTrigger = false;
        }
    }

    public void Interact()
    {
        StartCoroutine(Interacting());
    }

    private IEnumerator Interacting()
    {
        PlayerMovementPlatformer.instance.isInteracting = true;
        onInteract.Invoke();
        yield return new WaitForSeconds(interactTime);
        PlayerMovementPlatformer.instance.isInteracting = false;
    }
}
