using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostCompanion : MonoBehaviour
{
    public bool isInDark = false;
    public bool isInFlashlightRange = false;

    private bool isRunningScared = false;

    private AIDestinationSetter destinationSetter;
    private AIPath path;
    private Animator animator;
    private GhostCompanionGraphics graphics;

    private Coroutine interactCoroutine;
    private List<Coroutine> activeInteractCoroutines = new List<Coroutine>();

    private float defaultPathSpeed, defaultPathEndReachedDistance, pathEndReachedDistance;
    private float exactPathEndReachedDistance = 0.001f;

    private Transform scareDestination;
    private Vector3 startPosition;

    void Start()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        path = GetComponent<AIPath>();
        animator = GetComponentInChildren<Animator>();
        graphics = GetComponentInChildren<GhostCompanionGraphics>();

        GameObject obj = new GameObject();
        obj.name = "ScareDestination";
        scareDestination = obj.transform;
        startPosition = transform.position;

        defaultPathSpeed = path.maxSpeed;
        defaultPathEndReachedDistance = path.endReachedDistance;
        pathEndReachedDistance = defaultPathEndReachedDistance;

        SetDestination(PlayerMovementPlatformer.instance.transform);
    }

    private void Update()
    {
        if (isInDark && !isInFlashlightRange && !isRunningScared)
        {
            if (!PlayerMovementPlatformer.instance.isInDark)
            {
                pathEndReachedDistance = defaultPathEndReachedDistance;
                scareDestination.position = PlayerMovementPlatformer.instance.transform.position;
                RunScared();
            }
            else
            {
                pathEndReachedDistance = exactPathEndReachedDistance;
                scareDestination.position = startPosition;
                RunScared();
            }
        }
    }

    public void GoInteract (GameObject interactableObject, Transform target)
    {
        if (isRunningScared)
        {
            Debug.Log("Tubs can't interacted because he's still running scared");
            return;
        }
        else if (interactCoroutine != null)
        {
            Debug.Log("Interaction still in progress");
            return;
        }

        GhostInteractable interactable = interactableObject.GetComponent<GhostInteractable>();
        if (interactable != null)
        {
            interactCoroutine = StartCoroutine(InteractOnArrival(interactable, target));
        }
        else
        {
            GoHere(interactableObject.transform, true);
            Debug.LogWarning("Ghost Interactable is missing its GhostInteractable script: " + interactableObject.name);
        }
    }

    private IEnumerator InteractOnArrival (GhostInteractable interactable, Transform target)
    {
        AdjustPathingSettings(defaultPathSpeed, .25f);
        yield return WaitForMoveComplete(target);

        float distance = interactable.transform.position.x - transform.position.x;
        if (distance > 0f)
        {
            graphics.FlipRight();
        }
        else if (distance < 0f)
        {
            graphics.FlipLeft();
        }

        animator.SetTrigger("interactOnce");
        yield return new WaitForSeconds(0.25f);
        interactable.Interact();

        AdjustPathingSettings(defaultPathSpeed, defaultPathEndReachedDistance);
        StopInteractCoroutine();
    }

    private void RunScared()
    {
        StopInteractCoroutine();

        SetDestination(transform);

        animator.SetTrigger("getScared"); // Animation calls OnScareAnimationComplete which triggers movement
        isRunningScared = true;
    }

    public void OnScareAnimationComplete()
    {
        StopInteractCoroutine();
        StartCoroutine(WaitForScareEnd());
    }

    private IEnumerator WaitForScareEnd()
    {
        AdjustPathingSettings(5f, pathEndReachedDistance);
        yield return WaitForMoveComplete(scareDestination);
        isRunningScared = false;
        AdjustPathingSettings(defaultPathSpeed, pathEndReachedDistance);
    }
    
    private IEnumerator WaitForMoveComplete(Transform target)
    {
        SetDestination(target);
        path.SearchPath();

        while (path.pathPending || !path.reachedEndOfPath)
        {
            yield return null;
        }
    }

    public void GoHere (Transform target, bool isExact)
    {
        if (isRunningScared)
        {
            Debug.Log("Tubs can't go there because he's still running scared");
            return;
        }

        if (interactCoroutine != null)
        {
            StopInteractCoroutine();
        }

        if (isExact)
        {
            pathEndReachedDistance = exactPathEndReachedDistance;
        }
        else
        {
            pathEndReachedDistance = defaultPathEndReachedDistance;
        }

        AdjustPathingSettings(defaultPathSpeed, pathEndReachedDistance);
        SetDestination(target);
    }

    private void SetDestination(Transform target)
    {
        destinationSetter.target = target;
    }

    private void StopInteractCoroutine()
    {
        if (interactCoroutine != null)
        {
            StopCoroutine(interactCoroutine);
            interactCoroutine = null;
        }
    }

    private void AdjustPathingSettings(float speed, float endReachedDistance)
    {
        path.maxSpeed = speed;
        path.endReachedDistance = endReachedDistance;
    }

    public void SetInFlashlightRange(bool state)
    {
        isInFlashlightRange = state;
    }

    public void SetIsInDark(bool state, GameObject darkZone)
    {
        isInDark = state;
    }
}
