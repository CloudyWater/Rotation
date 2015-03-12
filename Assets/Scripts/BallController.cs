using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour {

	private const float MAX_MOVESPEED = 4f;
	private const float ACCELERATION = 40;
	private const float GRAVITY = -1.30f;
		
	public RingController controller;
	public Vector3 mStartingPosition;
	
	private bool mbInAction;
	private bool mbMovementsSealed;
	private bool mbTouchEnabled;
	private float mGravityMod = 1;
	private float mGravityModTime = 0;
	private float mMovementVelocity = 0f;
	private float mMoveSealTimer = 0;
	private float mMinimumSwipeDistance;
	private Vector3 mTouchLocation;
	

	// Use this for initialization
	void Start () 
	{
		mStartingPosition = transform.position;
		mbInAction = true;
		mbMovementsSealed = false;
		mbTouchEnabled = false;
		mMinimumSwipeDistance = Screen.width * .10f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (mbInAction)
		{
			float x, y, angle;
			Vector2 force;
			
			if (mbMovementsSealed)
			{
				mMoveSealTimer -= Time.deltaTime;
				if (mMoveSealTimer <= 0)
				{
					mbMovementsSealed = false;
				}
			}
			
			x = transform.position.x;
			y = transform.position.y;
			
			angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
			
			//add force towards the origin to simulate gravity towards the center
			if (mGravityModTime > 0)
			{
				force = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector2 (GRAVITY * mGravityMod, 0);
				this.rigidbody2D.AddForce (force);
				mGravityModTime -= Time.deltaTime;
			}
			else
			{
				force = Quaternion.AngleAxis(angle, Vector3.forward) * new Vector2 (GRAVITY, 0);
				this.rigidbody2D.velocity += force ;
			}
			
			angle += 90;
			
			Vector2 testForce = Quaternion.AngleAxis (angle, Vector3.forward) * new Vector2 (MAX_MOVESPEED, 0);
			testForce += force;
			
			force = Quaternion.AngleAxis (angle, Vector3.forward) * new Vector2 (mMovementVelocity, 0);
			this.rigidbody2D.AddForce (force);

			if (testForce.magnitude < this.rigidbody2D.velocity.magnitude && !mbMovementsSealed)
			{
				float finalMag = this.rigidbody2D.velocity.magnitude - (this.rigidbody2D.velocity.magnitude - testForce.magnitude) * .25f;
				this.rigidbody2D.velocity = Vector2.ClampMagnitude (this.rigidbody2D.velocity, finalMag);
			}
			
			//Move The ball with KB/Mouse
			
			if (Input.GetKey (KeyCode.LeftArrow) && !mbMovementsSealed)
			{
				mMovementVelocity = ACCELERATION;
			}
			else if (Input.GetKey (KeyCode.RightArrow) && !mbMovementsSealed)
			{
				mMovementVelocity = -ACCELERATION;
			}
			else if (!mbTouchEnabled)
			{
				mMovementVelocity = 0;
			}
			
			//Move The ball with Touch
			
			if (Input.touchCount > 0)
			{
				mbTouchEnabled = true;
				Touch touch  = Input.GetTouch (0);
				if (touch.phase == TouchPhase.Began)
				{
					mTouchLocation = touch.position;
				}
				if (touch.phase == TouchPhase.Ended)
				{
					if (Mathf.Abs (touch.position.x - mTouchLocation.x) > mMinimumSwipeDistance)
					{
						if (touch.position.x - mTouchLocation.x > 0)
						{
							mMovementVelocity = ACCELERATION;
						}
						else
						{
							mMovementVelocity = -ACCELERATION;
						}
					}
				}
			}
		}
	}
	
	void OnTriggerEnter2D (Collider2D other)
	{
		if (other.tag.Equals("Endpoint"))
		{
			Debug.Log ("Level end reached!");
			controller.LevelFinished ();
			mbInAction = false;
		}
		else if (other.tag.Equals ("Obstacle"))
		{
			controller.HitObstacle (other.gameObject);
		}
	}
	
	public void SetGravityModifier (float modifier, float timer)
	{
		mGravityMod = modifier;
		mGravityModTime = timer;
	}
	
	public void ResetPosition ()
	{
		transform.position = mStartingPosition;
		mbInAction = true;
	}
	
	public void SetPosition (Vector3 position)
	{
		transform.position = position;
	}
	
	public void AddForce (float magnitude, RingController.Ring.Direction direction)
	{
		float x, y, angle;
		Vector2 force;
		x = transform.position.x;
		y = transform.position.y;
		
		angle = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
		angle += 90;
		
		if (direction.Equals (RingController.Ring.Direction.LEFT))
		{
			magnitude *= -1;
		}
		
		force = Quaternion.AngleAxis (angle, Vector3.forward) * new Vector2 (magnitude, 0);
		this.rigidbody2D.velocity = force;
	}
	
	public void SealMovements (float time)
	{
		mbMovementsSealed = true;
		mMoveSealTimer = time;
	}
	
	public void SetMoveSpeed ()
	{
	
	}
}
