using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GhostHelpRequester : MonoBehaviour
{
    public GhostCompanion companion;

    public LayerMask ghostInteractableLayer;

    private Transform clickedPosition;

    void Start()
    {
        GameObject obj = new GameObject();
        obj.name = "ClickedPosition";
        clickedPosition = obj.transform;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f,ghostInteractableLayer))
            {
                Transform target = CreateTransformAtNearestSide(hit.collider.transform);
                companion.GoInteract(hit.collider.gameObject, target);
            }
            else
            {
                clickedPosition.position = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
                companion.GoHere(clickedPosition, true);
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            companion.GoHere(transform, false);
        }
    }

    private Transform CreateTransformAtNearestSide(Transform parent)
    {
        Transform targetTransform = parent.Find("PathingTarget");
        GameObject target;

        if (targetTransform == null)
        {
            target = new GameObject();
            target.name = "PathingTarget";
            target.transform.parent = parent;
            targetTransform = target.transform;
        }
        else
        {
            target = targetTransform.gameObject;
        }

        target.transform.localPosition = Vector3.left;
        return target.transform;
    }
}
