using UnityEngine;
using System.Collections;

/**
 * Paddle controller.
 * The AI and user controls are in this script
 */
public class PaddleScript : MonoBehaviour 
{
	private Rigidbody2D physics;
	private BoxCollider2D collider;
	private const float speed = 4f; //the maximum horizontal speed of the paddle
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
		collider = GetComponent<BoxCollider2D> ();
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
}
