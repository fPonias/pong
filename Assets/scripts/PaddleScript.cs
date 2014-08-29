using UnityEngine;
using System.Collections;

/**
 * Paddle controller.
 * The AI and user controls are in this script
 */
public class PaddleScript : MonoBehaviour 
{
	private Rigidbody2D physics;
	private const float speed = 4.5f; //the maximum horizontal speed of the paddle
	private float starty; //the y coordinate of the paddle in case it gets knocked out of place
	private int _ailevel; //the level of diffuculty for the ai.  0 for human

	public int ailevel
	{
		get { return _ailevel;}
		set 
		{
			if (value >= 0 && value < 10)
				_ailevel = value; 
		}
	}

	// Use this for initialization
	void Start () 
	{
		physics = GetComponent<Rigidbody2D> ();
		starty = physics.position.y;
		ailevel = 0;
	}

	public void setAI (int level)
	{
		ailevel = level;
	}

	/**
	 * Get the direction we should move the paddle in.
	 * -1 to the left. 1 to the right. 0 to stay put.
	 */
	private float getInput()
	{
		GameObject ball = GameObject.FindGameObjectWithTag ("ball");
		Vector2 pos = ball.transform.position;

		//get the player input
		if (_ailevel <= 0)
		{
			Vector3 accel = Input.acceleration;

			//handle keyboard input
			if (accel.magnitude == 0)
				return Input.GetAxis("Horizontal");
			//handle accelerometer output for handheld devices
			else
			{
				if (accel.x <= -0.1f)
					return -1f;
				else if (accel.x >= 0.1f)
					return 1f;
				else
					return 0;
			}
		}
		//basic AI, stay centered on the ball
		else if (_ailevel == 1)
		{
			if (physics.position.x < pos.x - 0.15f)
				return 1;
			else if (physics.position.x > pos.x + 0.15f)
				return -1;
			else
				return 0;
		}
		//easier AI, forget about the ball if it's too far away
		else if (_ailevel == 2)
		{
			if (pos.y < -3f)
			{
				return 0;
			}
			else
			{
				if (physics.position.x < pos.x - 0.15f)
					return 1;
				else if (physics.position.x > pos.x + 0.15f)
					return -1;
				else
					return 0;
			}
		}

		return 0;
	}

	// Update is called once per frame
	void Update () 
	{
		//reset the paddle if it got knocked out of place by the physics
		physics.isKinematic = true;
		Vector2 oldPos = physics.position;
		oldPos.y = starty;
		physics.transform.position = oldPos;
		physics.transform.rotation = Quaternion.identity;
		physics.isKinematic = false;


		//get the direction we should move the paddle
		float input = getInput ();

		//move the paddle
		if (input != 0)
		{
			float multiplier = 1.0f;

			if (input < 0)
				multiplier = -1.0f;

			Vector2 newvel = new Vector2(multiplier * speed, 0);
			physics.velocity = newvel;
		}
		else
		{
			Vector2 newvel = Vector2.zero;
			physics.velocity = newvel;
		}
	}
	
	public void OnCollisionEnter2D(Collision2D coll) 
	{
		//modify the ball path if it hits the paddle
		if (coll.gameObject.tag == "ball")
		{
			//compare the old ball velocity and the new one
			BallScript ballCtrl = coll.gameObject.GetComponent<BallScript>();
			Vector2 oldVel = ballCtrl.lastVelocity;
			Rigidbody2D ballPhysc = coll.gameObject.rigidbody2D;
			Vector2 newVel = ballPhysc.velocity;

			//if the ball bounced off of the paddle in the opposite direction ...
			if (oldVel.y * newVel.y < 0)
			{
				float angle;

				//find the angle of reflection
				if (newVel.x != 0.0f)
					angle = (Mathf.Asin (newVel.x / newVel.magnitude) * (180.0f / Mathf.PI));
				else
					angle = 0.0f;

				//find where the ball hit the paddle
				CircleCollider2D ballColl = (CircleCollider2D) coll.collider;
				float ballx = ballPhysc.position.x;
				float paddlex = physics.position.x;
				float paddlewidth = ((BoxCollider2D) collider2D).size.x;

				//calculate the angle modifier
				float diff = (ballx - paddlex) / (paddlewidth / 1.6f);
				float weight = Mathf.Asin (Mathf.Clamp(diff, -1.0f, 1.0f));
				float addAngle = weight * (180.0f / Mathf.PI);

				//make it easier to straighten out the ball
				if (Mathf.Abs(angle) > 45 && addAngle * angle < 0)
					addAngle *= 2.0f;

				//make sure the new angle isn't too flat
				float newAngle = Mathf.Clamp(angle + addAngle, -60.0f, 60.0f);

				//the new angle is different if it struck the bottom of the paddle
				if (newVel.y < 0)
					newAngle = -newAngle + 180.0f;

				//rotate the ball path to the new angle
				Quaternion q = Quaternion.AngleAxis(newAngle, new Vector3(0.0f, 0.0f, -1.0f));
				Vector2 nextVel = q * (Vector2.up * oldVel.magnitude);
				ballPhysc.velocity = nextVel;
			}
		}
	}
}
