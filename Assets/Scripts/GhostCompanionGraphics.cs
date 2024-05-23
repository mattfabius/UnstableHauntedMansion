using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class GhostCompanionGraphics : MonoBehaviour
{
    public AIPath aiPath;
    public float tolerance = 0.01f;

    private bool isFacingRight = false;

    private void Start()
    {
        aiPath = GetComponentInParent<AIPath>();
    }

    private void Update()
    {
        
        if (aiPath.velocity.x >= tolerance && !isFacingRight && !aiPath.reachedEndOfPath && !aiPath.pathPending && aiPath.hasPath)
        {
            FlipRight();
        }
        else if (aiPath.velocity.x <= -tolerance && isFacingRight && !aiPath.reachedEndOfPath && !aiPath.pathPending && aiPath.hasPath)
        {
            FlipLeft();
        }
    }

    public void OnScareAnimationComplete()
    {
        GetComponentInParent<GhostCompanion>().OnScareAnimationComplete();
    }

    public void FlipRight()
    {
        transform.localScale = new Vector3(-1f, 1f, 1f);
        isFacingRight = true;
    }

    public void FlipLeft()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
        isFacingRight = false;
    }
}
