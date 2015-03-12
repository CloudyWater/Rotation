using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.IO;

public class RingController : MonoBehaviour {

	private const string LEVEL_FILE = "Levels/levels";
	private const string LEVEL = "level:";
	private const string LEFT = "LEFT";
	private const string BLOCKERS = "BLOCKERS";
	private const string GRAVITY = "GRAVITY";
	private const string BOUNCER = "BOUNCER";
	private const string FREEZER = "FREEZER";
	private const string PORTALS = "PORTAL";
	private const string LEVELTEXT = "Level: ";
	
	private const int DIRECTION = 0;
	private const int ROTATION = 1;
	private const int SPEED = 2;
	
	private const float MOVE_SPEED = 40f;

	public GameObject[] mRings;
	public GameObject[] mObjectLocations;
	public GameObject mBlock;
	public GameObject mGravityReverser;
	public GameObject mBouncer;
	public GameObject mFreezer;
	public GameObject mPortal;
	public int mSelectedRing;
	public BallController mBall;
	public Text mParTime;
	public Text mUserTime;
	public Text mParMade;
	public Text mParNotMade;
	public Text mContinueText;
	public Text mLevel;
	
	private float mTime;
	private bool mLevelFinished;
	
	private int mLevelNumber;

	
	public class Ring 
	{
		public enum Direction
		{
			LEFT, RIGHT
		};
	
		private float mRotation;
		private float mRotSpeed;
		private Color mColor;
		private GameObject mRing;
		private Direction mRotDirection;
		
		public Ring (float rotation, float rotSpeed, Color color, GameObject ring, Direction direction)
		{
			mRotation = rotation;
			mRotSpeed = rotSpeed;
			mColor = color;
			mRing = ring;
			mRotDirection = direction;
			mRing.transform.Rotate(0,0,rotation);
			SpriteRenderer spriteRender = mRing.GetComponent <SpriteRenderer> ();
			spriteRender.color = mColor;
			spriteRender.enabled = true;
			mRing.collider2D.enabled = true;
		}
		
		public void Update (float delta)
		{
			if (mRotDirection.Equals (Direction.LEFT))
			{
				mRing.transform.Rotate(0, 0, mRotSpeed * delta);
			}
			else
			{
				mRing.transform.Rotate (0, 0, -mRotSpeed * delta);
			}
			mRotation = mRing.transform.localRotation.z;
		}
		
		public float getRotation ()
		{
			return mRotation;
		}
		
		public void setRotation (float rotation)
		{
			mRotation = rotation;
		}
		
		public float getRotSpeed ()
		{
			return mRotSpeed;
		}
		
		public void setRotSpeed (float rotSpeed)
		{
			mRotSpeed = rotSpeed;
		}
		
		public Color getColor ()
		{
			return mColor;
		}
		
		public void setColor (Color color)
		{
			mColor = color;
			SpriteRenderer spriteRender = mRing.GetComponent <SpriteRenderer> ();
			spriteRender.color = mColor;
		}
		
		public GameObject getRing ()
		{
			return mRing;
		}
		
		public void setRing (GameObject ring)
		{
			mRing = ring;
		}
	}
	
	public abstract class Obstacle
	{
		protected Ring.Direction mDirection;
		protected float mRotation;
		protected float mRotSpeed;
		protected GameObject mObstacle;
		
		public Obstacle (GameObject targetLocation, GameObject blocker, Ring.Direction rotationDirection, float rotation, float rotationSpeed)
		{
			mObstacle = (GameObject) Instantiate (blocker, targetLocation.transform.position, targetLocation.transform.localRotation);
			mRotation = rotation;
			mObstacle.transform.RotateAround (Vector3.zero, Vector3.forward, mRotation);
			mRotSpeed = rotationSpeed;
			mDirection = rotationDirection;
		}
		
		public abstract void OnCollision (BallController ball);
		
		public virtual void Update (float time)
		{
			if (mDirection.Equals (Ring.Direction.LEFT))
			{ 
				mObstacle.transform.RotateAround (Vector3.zero, Vector3.forward, -mRotSpeed * time);
			}
			else
			{
				mObstacle.transform.RotateAround (Vector3.zero, Vector3.forward, mRotSpeed * time);
			}
		}
		
		public Ring.Direction getDirection ()
		{
			return mDirection;
		}
		
		public void setDirection (Ring.Direction direction)
		{
			mDirection = direction;
		}
		
		public float getRotation ()
		{
			return mRotation;
		}
		
		public void setRotation (float rotation)
		{
			mRotation = rotation;
		}
		
		public float getRotationSpeed ()
		{
			return mRotSpeed;
		}
		
		public void setRotationSpeed (float speed)
		{
			mRotSpeed = speed;
		}
		
		public GameObject getObject ()
		{
			return mObstacle;
		}
	}
	
	public class Blocker : Obstacle
	{
		public Blocker (GameObject targetLocation, GameObject blocker, Ring.Direction rotationDirection, float rotation, float rotationSpeed) 
		: base (targetLocation, blocker, rotationDirection, rotation, rotationSpeed)
		{
			
		}
		
		public override void OnCollision (BallController ball)
		{
			
		}
	}
	
	public class Portal : Obstacle
	{
		protected Portal mLinkedPortal;
		protected bool mbPorted;
		protected float mActiveTimer;
		
		protected const float COOLDOWN = 1.0f;
		
		public Portal (GameObject targetLocation, GameObject portal, Ring.Direction rotationDirection, float rotation, float rotationSpeed, Color mColor)
		: base (targetLocation, portal, rotationDirection, rotation, rotationSpeed)
		{
			SpriteRenderer renderer = mObstacle.GetComponent <SpriteRenderer> ();
			renderer.color = mColor;
		}
		
		public override void Update (float deltaTime)
		{
			if (mbPorted)
			{
				mActiveTimer -= deltaTime;
				if (mActiveTimer <= 0)
				{
					SetPorted (false);
				}
			}
		}
		
		public void LinkPortals (Portal portal)
		{
			mLinkedPortal = portal;
		}
		
		public override void OnCollision (BallController ball)
		{
			if (!mbPorted)
			{
				ball.SetPosition (mLinkedPortal.getObject ().transform.position);
				mLinkedPortal.SetPorted (true);
				SetPorted (true);
			}
		}
		
		public void SetPorted (bool ported)
		{
			mbPorted = ported;
			mActiveTimer = COOLDOWN;
		}
	}
	
	public class GravityReversal : Obstacle
	{
		protected const int SPAWN_TIME = 3;
	
		protected float mRespawnTimer;
		
		public GravityReversal (GameObject targetLocation, GameObject blocker, Ring.Direction rotationDirection, float rotation, float rotationSpeed) 
		: base (targetLocation, blocker, rotationDirection, rotation, rotationSpeed)
		{
		
		}
		
		public override void Update (float deltaTime)
		{
			base.Update (deltaTime);
			if (mRespawnTimer > 0)
			{
				mRespawnTimer -= deltaTime;
				if (mRespawnTimer <= 0)
				{
					Activate ();
				}
			}
		}
		
		public override void OnCollision (BallController ball)
		{
			ball.SetGravityModifier (-1, 1);
			Deactivate (SPAWN_TIME);
		}
		
		protected void Deactivate (float time)
		{
			mObstacle.SetActive (false);
			mRespawnTimer = time;	
		}
		
		protected void Activate ()
		{
			mObstacle.SetActive (true);
		}
	}
	
	public class Freezer : GravityReversal
	{
		protected float FREEZE_TIME = 1.0f;
		
		public Freezer (GameObject targetLocation, GameObject blocker, Ring.Direction rotationDirection, float rotation, float rotationSpeed) :
		base (targetLocation, blocker, rotationDirection, rotation, rotationSpeed)
		{
			
		}
		
		public override void OnCollision (BallController ball)
		{
			Deactivate (SPAWN_TIME);
			ball.SealMovements (FREEZE_TIME);
		}
	}

	public class Bouncer : GravityReversal
	{
		private const float SEAL_TIME = .25f;
		private const float BLOCKER_FORCE = 50f;
		private Ring.Direction mFacing;
		
		public Bouncer (GameObject targetLocation, GameObject bouncer, Ring.Direction rotationDirection, float rotation, float rotationSpeed, Ring.Direction facing)
			: base (targetLocation, bouncer, rotationDirection, rotation, rotationSpeed)
		{
			mFacing = facing;
			
			if (mFacing.Equals (Ring.Direction.LEFT))
			{
				mObstacle.transform.localRotation = new Quaternion (mObstacle.transform.localRotation.x, 
				                                                    mObstacle.transform.localRotation.y, mObstacle.transform.localRotation.z + 180, mObstacle.transform.localRotation.w);
			}
		}
		
		public override void OnCollision (BallController ball)
		{
			ball.AddForce (BLOCKER_FORCE, mFacing);
			ball.SealMovements (SEAL_TIME);
			Deactivate (SPAWN_TIME);
		}
	}

	public class Level 
	{
		private float mParTime;
		private int mNumRings;
		private Ring[] mRings;
		private Obstacle[] mObstacles = null;
		private bool mbLevelLoaded = true;
		
		public void LoadLevel (int levelNumber, GameObject[] rings, GameObject[] blockers, GameObject blocker, GameObject gravReverser, GameObject bouncer, 
		GameObject freezer, GameObject portal, Text parTime)
		{
			
			//need code to read level setup from file
			bool bLevelFound = false, bObstaclesRead = false;
			string line;
			char[] delims = {','};
			int obstacleLevel;
			int numBlockers, numObstacles, numGravReversers, numBouncers, numFreezers, numPortals, obstacleCount = 0;
			
			float rotation, rotspeed;
			Color color;
			Ring.Direction direction;
			StringReader reader;
			
			TextAsset levelFile = Resources.Load (LEVEL_FILE) as TextAsset;
			
			if (mObstacles != null)
			{
				foreach (Obstacle obj in mObstacles)
				{
					GameObject.Destroy(obj.getObject ());
				}
			}
			
			try
			{
				reader = new StringReader (levelFile.text);

				do
				{
					line = reader.ReadLine ();
					
					if (line != null && line.StartsWith (LEVEL))
					{
						int levelNum = int.Parse (line.Substring (LEVEL.Length));
						if (levelNum == levelNumber)
						{
							bLevelFound = true;
						}
					}
				}
				while (line != null && !bLevelFound);
				
				line = reader.ReadLine ();
				if (bLevelFound)
				{
					mNumRings = int.Parse (line);
					mRings = new Ring[mNumRings];
					
					for (int i = 0; i < mNumRings; i++)
					{
						line = reader.ReadLine ();
						string[] lineInfo = line.Split (delims);
						
						if (lineInfo[DIRECTION].Equals (LEFT))
						{
							direction = Ring.Direction.LEFT;
						}
						else
						{
							direction = Ring.Direction.RIGHT;
						}
						rotation = float.Parse (lineInfo[ROTATION]);
						rotspeed = float.Parse (lineInfo[SPEED]);
						color = new Color (Random.value, Random.value, Random.value);
						mRings[i] = new Ring(rotation, rotspeed, color, rings[i], direction);
					}
					
					for (int i = mNumRings; i < rings.Length; i++)
					{
						SpriteRenderer renderer = rings[i].GetComponent<SpriteRenderer> ();
						renderer.enabled = false;
						rings[i].collider2D.enabled = false;
					}
					
					line = reader.ReadLine ();
					numObstacles = int.Parse (line);
					mObstacles = new Obstacle[numObstacles];
					line = reader.ReadLine ();
					do
					{						
						if (line.StartsWith (BLOCKERS))
						{
							numBlockers = int.Parse (reader.ReadLine ());
														
							for (int i = obstacleCount; i < numBlockers + obstacleCount; i++)
							{
								line = reader.ReadLine ();
								string[] lineInfo = line.Split (delims);
								if (lineInfo[DIRECTION].Equals(LEFT))
								{
									direction = Ring.Direction.LEFT;
								}
								else
								{
									direction = Ring.Direction.RIGHT;
								}
								rotation = float.Parse (lineInfo[ROTATION]);
								rotspeed = float.Parse (lineInfo[SPEED]);
								obstacleLevel = int.Parse (lineInfo[SPEED + 1]);
								mObstacles[i] = new Blocker(blockers[obstacleLevel], blocker, direction, rotation, rotspeed);
							}
							
							obstacleCount += numBlockers;
							line = reader.ReadLine ();
						}
						else if (line.StartsWith (FREEZER))
						{
							numFreezers = int.Parse (reader.ReadLine ());
							
							for (int i = obstacleCount; i < numFreezers + obstacleCount; i++)
							{
								line = reader.ReadLine ();
								string[] lineInfo = line.Split (delims);
								if (lineInfo[DIRECTION].Equals(LEFT))
								{
									direction = Ring.Direction.LEFT;
								}
								else
								{
									direction = Ring.Direction.RIGHT;
								}
								rotation = float.Parse (lineInfo[ROTATION]);
								rotspeed = float.Parse (lineInfo[SPEED]);
								obstacleLevel = int.Parse (lineInfo[SPEED + 1]);
								mObstacles[i] = new Freezer(blockers[obstacleLevel], freezer, direction, rotation, rotspeed);
							}
							
							obstacleCount += numFreezers;
							line = reader.ReadLine ();
						}
						else if (line.StartsWith (GRAVITY))
						{
							numGravReversers = int.Parse (reader.ReadLine ());
							
							for (int i = obstacleCount; i < numGravReversers + obstacleCount; i++)
							{
								line = reader.ReadLine ();
								string[] lineInfo = line.Split (delims);
								if (lineInfo[DIRECTION].Equals(LEFT))
								{
									direction = Ring.Direction.LEFT;
								}
								else
								{
									direction = Ring.Direction.RIGHT;
								}
								rotation = float.Parse (lineInfo[ROTATION]);
								rotspeed = float.Parse (lineInfo[SPEED]);
								obstacleLevel = int.Parse (lineInfo[SPEED + 1]);
								mObstacles[i] = new GravityReversal(blockers[obstacleLevel], gravReverser, direction, rotation, rotspeed);
							}
							obstacleCount += numGravReversers;
							line = reader.ReadLine ();
						}
						else if (line.StartsWith (BOUNCER))
						{
							numBouncers = int.Parse (reader.ReadLine ());
							Ring.Direction facing;
							for (int i = obstacleCount; i < numBouncers + obstacleCount; i++)
							{
								line = reader.ReadLine ();
								string[] lineInfo = line.Split (delims);
								if (lineInfo[DIRECTION].Equals(LEFT))
								{
									direction = Ring.Direction.LEFT;
								}
								else
								{
									direction = Ring.Direction.RIGHT;
								}
								rotation = float.Parse (lineInfo[ROTATION]);
								rotspeed = float.Parse (lineInfo[SPEED]);
								obstacleLevel = int.Parse (lineInfo[SPEED + 1]);
								if (lineInfo [SPEED + 2].Equals (LEFT))
								{
									facing = Ring.Direction.LEFT;
								}
								else
								{
									facing = Ring.Direction.RIGHT;
								}
								mObstacles[i] = new Bouncer(blockers[obstacleLevel], bouncer, direction, rotation, rotspeed, facing);
							}
							obstacleCount += numBouncers;
							line = reader.ReadLine ();
						}
						else if (line.StartsWith (PORTALS))
						{
							Portal one, two;
							numPortals = int.Parse (reader.ReadLine ());
							for (int i = obstacleCount; i < numPortals + obstacleCount; i+=2)
							{
								color = new Color (Random.value, Random.value, Random.value);
								line = reader.ReadLine ();
								string[] lineInfo = line.Split (delims);
								if (lineInfo[DIRECTION].Equals(LEFT))
								{
									direction = Ring.Direction.LEFT;
								}
								else
								{
									direction = Ring.Direction.RIGHT;
								}
								rotation = float.Parse (lineInfo[ROTATION]);
								rotspeed = float.Parse (lineInfo[SPEED]);
								obstacleLevel = int.Parse (lineInfo[SPEED + 1]);
								one = new Portal (blockers[obstacleLevel], portal, direction, rotation, rotspeed, color);
								line = reader.ReadLine ();
								lineInfo = line.Split (delims);
								if (lineInfo[DIRECTION].Equals(LEFT))
								{
									direction = Ring.Direction.LEFT;
								}
								else
								{
									direction = Ring.Direction.RIGHT;
								}
								rotation = float.Parse (lineInfo[ROTATION]);
								rotspeed = float.Parse (lineInfo[SPEED]);
								obstacleLevel = int.Parse (lineInfo[SPEED + 1]);
								two = new Portal (blockers[obstacleLevel], portal, direction, rotation, rotspeed, color);
								one.LinkPortals (two);
								two.LinkPortals (one);
								mObstacles[i] = one;
								mObstacles[i+1] = two;
							}
							obstacleCount += numPortals;
							line = reader.ReadLine ();
						}
						else
						{
							bObstaclesRead = true;
						}
					} while (!bObstaclesRead);
					
					parTime.text = line;
					mParTime = float.Parse (line);
					line = reader.ReadLine ();
					
					mbLevelLoaded = true;
				}
				else
				{
					mbLevelLoaded = false;
				}
			}
			catch (IOException e)
			{
				Debug.Log (e.Message);
			}
		}
		
		public void ObstacleHit (GameObject obstacle, BallController ball)
		{
			foreach (Obstacle possible in mObstacles)
			{
				if (obstacle.Equals (possible.getObject ()))
				{
					possible.OnCollision (ball);
				}
			}
		}
		
		public void Update (float delta)
		{
			foreach (Ring ring in mRings)
			{
				ring.Update (delta);
			}
			if (mObstacles != null)
			{
				foreach (Obstacle obstacle in mObstacles)
				{
					obstacle.Update (delta);
				}
			}
		}
		
		public Ring[] getRings ()
		{
			return mRings;
		}
		
		public int getNumRings ()
		{
			return mNumRings;
		}
		
		public float GetParTime ()
		{
			return mParTime;
		}
		
		public bool isLevelLoaded ()
		{
			return mbLevelLoaded;
		}
		
	}
	
	private Level mCurrentLevel;

	// Use this for initialization
	void Start () 
	{
		mLevelNumber = LevelSelector.mLevelSelected;
		mCurrentLevel = new Level ();
		mCurrentLevel.LoadLevel (mLevelNumber, mRings, mObjectLocations, mBlock, mGravityReverser, mBouncer, mFreezer, mPortal, mParTime);
		mSelectedRing = mCurrentLevel.getNumRings () - 1;
		SpriteRenderer renderer = mRings [mSelectedRing].GetComponent <SpriteRenderer> ();
		renderer.color = Color.black;
		mTime = 0;
		mLevelFinished = false;
		mLevel.text = LEVELTEXT + mLevelNumber.ToString ();
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (!mLevelFinished)
		{
			mTime += Time.deltaTime;
			mCurrentLevel.Update (Time.deltaTime);
			
			//Handling key inputs for switching which ring is controlled
			
			if (Input.GetKeyDown (KeyCode.W))
			{
				mSelectedRing += 1;
				if (mSelectedRing >= mCurrentLevel.getNumRings())
				{
					mSelectedRing = mCurrentLevel.getNumRings() - 1;
				}
				else
				{
					/*SpriteRenderer renderer = mRings [mSelectedRing].GetComponent <SpriteRenderer> ();
					renderer.color = Color.black;
					renderer = mRings[mSelectedRing-1].GetComponent <SpriteRenderer> ();
					renderer.color = mCurrentLevel.getRings()[mSelectedRing-1].getColor ();*/
					mRings[mSelectedRing].GetComponent <Animator> ().SetTrigger("Selection");
				}
				
			}
			if (Input.GetKeyDown (KeyCode.S))
			{
				mSelectedRing -= 1;
				if (mSelectedRing < 0)
				{
					mSelectedRing = 0;
				}
				else
				{
				/*
					SpriteRenderer renderer = mRings [mSelectedRing].GetComponent <SpriteRenderer> ();
					renderer.color = Color.black;
					renderer = mRings[mSelectedRing+1].GetComponent <SpriteRenderer> ();
					renderer.color = mCurrentLevel.getRings()[mSelectedRing+1].getColor ();*/
					mRings[mSelectedRing].GetComponent <Animator> ().SetTrigger("Selection");
				}
			}
			
			
			//Handling key inputs for moving controlled ring
			
			if (Input.GetKey (KeyCode.A))
			{
				mRings[mSelectedRing].transform.Rotate (0,0,MOVE_SPEED * Time.deltaTime);
			}
			if (Input.GetKey (KeyCode.D))
			{
				mRings[mSelectedRing].transform.Rotate (0,0,-MOVE_SPEED * Time.deltaTime);
			}
			
			//changing the value in the user time text field
			string currentTime = mTime.ToString ();
			currentTime = currentTime.Substring (0, currentTime.IndexOf (".") + 2);
			
			mUserTime.text = currentTime;
		}
		else
		{
			if (Input.anyKey)
			{
				LoadNextLevel ();
				mLevelFinished = false;
				
				if (!mCurrentLevel.isLevelLoaded ())
				{
					Application.LoadLevel ("gameover");
				}
			}
		}
	}
	
	public void LevelFinished ()
	{
		mLevelFinished = true;
		
		if (mTime <= mCurrentLevel.GetParTime ())
		{
			mParMade.gameObject.SetActive (true);
			PlayerPrefs.SetInt (LEVEL + mLevelNumber.ToString (), LevelSelector.PASSED);
			PlayerPrefs.Save ();
		}
		else
		{
			mParNotMade.gameObject.SetActive (true);
			PlayerPrefs.SetInt (LEVEL + mLevelNumber.ToString (), LevelSelector.PLAYED);
			PlayerPrefs.Save ();
		}
		
		mContinueText.gameObject.SetActive (true);
	}
	
	public void LoadNextLevel ()
	{
		mBall.ResetPosition ();
		mCurrentLevel.LoadLevel (++mLevelNumber, mRings, mObjectLocations, mBlock, mGravityReverser, mBouncer, mFreezer, mPortal, mParTime);
		mSelectedRing = mCurrentLevel.getNumRings () - 1;
		SpriteRenderer renderer = mRings [mSelectedRing].GetComponent <SpriteRenderer> ();
		renderer.color = Color.black;
		mTime = 0;
		
		mParMade.gameObject.SetActive (false);
		mParNotMade.gameObject.SetActive (false);
		mContinueText.gameObject.SetActive (false);
		mLevel.text = LEVELTEXT + mLevelNumber.ToString ();
	}
	
	public void HitObstacle (GameObject obstacle)
	{
		mCurrentLevel.ObstacleHit (obstacle, mBall);
	}
}
