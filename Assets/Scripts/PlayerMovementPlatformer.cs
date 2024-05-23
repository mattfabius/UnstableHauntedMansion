using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Rendering.Universal;
[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovementPlatformer : MonoBehaviour {

	public static PlayerMovementPlatformer instance;

	[Header("Movement Speeds")]
	public float moveSpeed = 0.6f;
	public float jumpStrength = 1f;
	public float gravity = -20f;
	public float terminalVelocity = -50f;
	//public float stillCountAsOnGroundTimer = 0.25f;

	//Private Movement Parameters
	Vector3 velocity;
	bool onGround = false;
	bool onSoftGround = false;
	bool ignoreSoftGround = false;
	float dropStartPoint = 0f;
	//float onGroundTimer = 0.25f;

	[Header("Collision Parameters")]
	public LayerMask hardCollisionMask;
	public LayerMask softCollisionMask;
	public int horizontalRayCount = 4;
	public int verticalRayCount = 4;

	// Private Collision Parameters
	const float skinWidth = 0.015f;
	float horizontalRaySpacing;
	float verticalRaySpacing;
	BoxCollider2D col;
	RaycastOrigins raycastOrigins;

	[Header("Flashlight Parameters")]
	public float flashlightFlipTime = 1f;
	public Transform flashlightLeftOffset;
	public Transform flashlightRightOffset;
	public Transform flashlightUpOffset;
	public Transform flashlightDownOffset;

	private float flashlightStartRadius;
	private Coroutine flipFlashlightCoroutine;

	[HideInInspector]
	public bool isInDark = false;
	[HideInInspector]
	public bool isInteracting = false;

	Animator anim;
	SpriteRenderer sprite;
	Light2D flashlight;

	enum Direction { Up, Down, Left, Right };
	Direction currentDirection = Direction.Right;

	private void Awake () {
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
		}

		col = GetComponent<BoxCollider2D> ();
		anim = GetComponentInChildren<Animator> ();
		sprite = transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
		flashlight = GetComponentInChildren<Light2D>();
		flashlightStartRadius = flashlight.pointLightOuterRadius;

		flashlight.transform.position = flashlightRightOffset.position;
		flashlight.transform.localRotation = flashlightRightOffset.localRotation;

		CalculateRaySpacing ();
	}

    private void Update()
    {
		if (isInteracting)
			return;

		GetInput();
		SetAnimations();
	}

    private void FixedUpdate () {
		Move ();
		
		if (ignoreSoftGround && dropStartPoint - transform.position.y > 0.5f)
        {
			ignoreSoftGround = false;
		}
	}

	private void GetInput () {
		float inputX = Input.GetAxisRaw("Horizontal");
		float inputY = Input.GetAxisRaw("Vertical");
		if (!Input.GetButton("Flashlight"))
        {
			velocity.x = inputX * moveSpeed;
		}

		if (inputX > 0 && currentDirection != Direction.Right) 
		{
			currentDirection = Direction.Right;
			StartFlipFlashlight(Direction.Right);
		} 
		else if (inputX < 0 && currentDirection != Direction.Left) 
		{
			currentDirection = Direction.Left;
			StartFlipFlashlight(Direction.Left);
		} 
		else if (inputY > 0 && currentDirection != Direction.Up)
        {
			currentDirection = Direction.Up;
			StartFlipFlashlight(Direction.Up);
		}
		else if (inputY < 0 && currentDirection != Direction.Down)
        {
			currentDirection = Direction.Down;
			StartFlipFlashlight(Direction.Down);
        }

		if (inputY < 0 && onSoftGround && !Input.GetButton("Flashlight")) { //Drop through soft platform
			ignoreSoftGround = true;
			dropStartPoint = transform.position.y;
			onSoftGround = false;
		} else if (Input.GetButtonDown ("Jump") && onGround == true) { //Jump if on ground
			velocity.y += jumpStrength / 10;
		}
	}

	private void FlipX (bool state)
    {
		Vector3 scale = transform.localScale;
		scale.x = state? -1f : 1f;
		transform.localScale = scale;
	}

	private void SetAnimations()
    {
		if (velocity.x != 0f)
		{
			anim.SetBool("isWalking", true);
		}
		else
		{
			anim.SetBool("isWalking", false);
		}

		if (onGround)
        {
			anim.SetBool("isOnGround", true);
        }
		else
        {
			anim.SetBool("isOnGround", false);
		}
	}
	private void StartFlipFlashlight(Direction direction)
    {
		if (flipFlashlightCoroutine != null)
        {
			StopCoroutine(flipFlashlightCoroutine);
		}
		
		flipFlashlightCoroutine = StartCoroutine(FlipFlashlight(direction));
	}

	private IEnumerator FlipFlashlight(Direction direction)
	{
		flashlight.pointLightOuterRadius = flashlightStartRadius;
		bool hasFlipped = false;
		for (float time = 0f; time < flashlightFlipTime; time += Time.deltaTime)
		{
			if (time < flashlightFlipTime / 2f)
			{
				flashlight.pointLightOuterRadius = flashlightStartRadius * (1 - (time / (flashlightFlipTime / 2f)));
			}
			else if (time == flashlightFlipTime / 2f)
			{
				flashlight.pointLightOuterRadius = 0f;
			}
			else
			{
				if (!hasFlipped)
				{
					Transform directionTransform;
					switch (direction)
                    {
						case Direction.Up:
							directionTransform = flashlightUpOffset;
							break;
						case Direction.Down:
							directionTransform = flashlightDownOffset;
							break;
						case Direction.Left:
							directionTransform = flashlightRightOffset;
							break;
						case Direction.Right:
							directionTransform = flashlightRightOffset;
							break;
						default:
							directionTransform = flashlightRightOffset;
							break;
                    }
					flashlight.transform.position = directionTransform.position;
					flashlight.transform.localRotation = directionTransform.localRotation;
					FlipX(direction == Direction.Left);
					hasFlipped = true;
				}
				flashlight.pointLightOuterRadius = flashlightStartRadius * ((time - (flashlightFlipTime / 2f)) / (flashlightFlipTime / 2f));
			}
			yield return null;
		}

		flashlight.pointLightOuterRadius = flashlightStartRadius;
	}

	private void Move () {
		UpdateRaycastOrigins ();

		velocity.y += gravity * Time.fixedDeltaTime;
		if (velocity.y < terminalVelocity)
			velocity.y = terminalVelocity;

		//Check for collisions and change input velocity accordingly
		if (velocity.x != 0) {
			CollideHorizontal ();
		}
		CollideVertical (); //deals with gravity so always on
		
		transform.Translate (velocity); //Move the player
	}

	#region Raycasting Physics
	private void CollideVertical () { //Change velocity based on collisions & set on-ground state
		float directionY = (velocity.y == 0) ? -1f : Mathf.Sign (velocity.y); //Default to down as raycast direction
		float rayLength = Mathf.Abs (velocity.y) + skinWidth; //the ray length is how long the player will move if it doesn't collide with anything this frame
		int rayGroundHits = 0; //used to verify if a collision occurred

		for (int i = 0; i < verticalRayCount; i++) {
			Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x); //determine ray placement relative to character

			RaycastHit2D hit = new RaycastHit2D ();
			RaycastHit2D hardHit = new RaycastHit2D ();
			RaycastHit2D softHit = new RaycastHit2D ();

			if (directionY >= 0) {
				hit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, hardCollisionMask); //don't check for soft collisions when moving upward
			} else {
				hardHit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, hardCollisionMask); //cast a ray on hard platform layer

				if (!ignoreSoftGround)
					softHit = Physics2D.Raycast (rayOrigin, Vector2.up * directionY, rayLength, softCollisionMask); //cast a ray on soft platform layer

				if (softHit && hardHit && softHit.distance < hardHit.distance) { //determine which hit object should be the one to collide with
					hit = softHit;
				} else if (softHit && hardHit && hardHit.distance <= softHit.distance) {
					hit = hardHit;
				} else if (softHit) {
					hit = softHit;
				} else if (hardHit) {
					hit = hardHit;
				}
			}

			Debug.DrawRay (rayOrigin, Vector2.up * directionY * rayLength, Color.red);

			if (hit) { // if there was a collision / if anything was hit by the ray
				if (hit.distance - (skinWidth * 2) <= 0) {
					float angle = 90f - Vector2.Angle(Vector2.down, hit.normal);
					if (directionY < 0f && (angle < 45f && angle > -45f))
					{
						Debug.Log("Slopey ground! Angle: " + angle);
						float slideDirection = Mathf.Sign(angle);
						velocity.x -= Mathf.Tan(angle * Mathf.Deg2Rad) * velocity.y * slideDirection;
					}
                    else
                    {
						velocity.y -= gravity * Time.fixedDeltaTime; //remove acceleration due to gravity if it's not safe to move down
						rayGroundHits++; //indicate that at least one ray is in an on-ground state

						onSoftGround = (hit == softHit); //store ground type for next frame's player input

						velocity.y = (hit.distance - skinWidth) * directionY; //adjust velocity so character doesn't move through the object it collided with
					}
				}
                else
                {
					onSoftGround = false;
					velocity.y = (hit.distance - skinWidth) * directionY; //adjust velocity so character doesn't move through the object it collided with
				}
					
				rayLength = hit.distance; //adjust the ray length to only account for collisions closer than this one for the rest of the rays
			}
		}

		onGround = (directionY <= 0 && rayGroundHits > 0); //store the on-ground state of the player for next frame's input
	}

	private void CollideHorizontal () { //Change velocity based on collisions & set on-ground state
		float directionX = Mathf.Sign (velocity.x); //direction to check collision in
		float rayLength = Mathf.Abs (velocity.x) + skinWidth; //the ray length is how long the player will move if it doesn't collide with anything this frame

		for (int i = 0; i < horizontalRayCount; i++) {
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight; // moving left = cast from left; moving right = cast from right
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, hardCollisionMask);

			Debug.DrawRay (rayOrigin, Vector2.right * directionX * rayLength, Color.red);

			if (hit) { //if something was hit
				velocity.x = (hit.distance - (skinWidth * 2)) * directionX; //adjust velocity so character doesn't move through the object it collided with
				rayLength = hit.distance; // only check for closer collisions
			}
		}
	}

	private void UpdateRaycastOrigins () {
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth * -2);

		raycastOrigins.bottomLeft = new Vector2 (bounds.min.x, bounds.min.y);
		raycastOrigins.bottomRight = new Vector2 (bounds.max.x, bounds.min.y);
		raycastOrigins.topLeft = new Vector2 (bounds.min.x, bounds.max.y);
		raycastOrigins.topRight = new Vector2 (bounds.max.x, bounds.max.y);
	}

	private void CalculateRaySpacing () {
		Bounds bounds = col.bounds;
		bounds.Expand (skinWidth * -2);

		horizontalRayCount = Mathf.Clamp (horizontalRayCount, 2, int.MaxValue); //must at least two per side (one at each corner)
		verticalRayCount = Mathf.Clamp (verticalRayCount, 2, int.MaxValue);

		horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1); //if min, space between two rays is entire width
		verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
	}

	private struct RaycastOrigins {
		public Vector2 topLeft, topRight;
		public Vector2 bottomLeft, bottomRight;
	}
	#endregion
}
