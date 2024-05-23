using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class FlashlightRange : MonoBehaviour
{
    public LayerMask flashlightLayerMask;
    private PolygonCollider2D flashlightCollider;

    private void Start()
    {
        flashlightCollider = GetComponent<PolygonCollider2D>();
        flashlightCollider.isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tubs"))
        {
            Vector3 flashlightStartPosition = transform.position;
            Vector3 direction = (other.transform.position - flashlightStartPosition).normalized;
            float distance = Vector3.Distance(flashlightStartPosition, other.transform.position);
            
            RaycastHit2D hit = Physics2D.Raycast(flashlightStartPosition, direction, distance, flashlightLayerMask);
            if (hit.collider != null & hit.collider.gameObject.CompareTag("Tubs"))
            {
                Debug.DrawRay(flashlightStartPosition, direction * distance, Color.red);
                other.gameObject.GetComponent<GhostCompanion>().SetInFlashlightRange(true);
                return;
            }
            else if (hit.collider != null)
            {
                Debug.DrawRay(flashlightStartPosition, hit.point-(Vector2)flashlightStartPosition, Color.red);
                Debug.Log("Flashlight blocked by " + hit.collider.gameObject.name);
            }
            else
            {
                Debug.Log("Flashlight hit nothing");
            }

            other.gameObject.GetComponent<GhostCompanion>().SetInFlashlightRange(false);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Tubs"))
        {
            other.gameObject.GetComponent<GhostCompanion>().SetInFlashlightRange(false);
        }
    }
}
