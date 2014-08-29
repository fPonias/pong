using UnityEngine;
using System.Collections;

/**
 * Ball controller Script.
 * handles ball behaviour on collision and provides a reset for the main controller.
 */
public class BallScript : MonoBehaviour 
{
	private Rigidbody2D _physics;
	public Rigidbody2D physics
	{
		get {return _physics;}
	}
	private CircleCollider2D collider;
	private float rotation;
	private const float MAX_ROTATION = 70.0f;
	private Vector2 defaultDir = new Vector2(0, 7f);

	// Use this for initialization
	void Start () 
	{
		_physics = GetComponent<Rigidbody2D> ();
		this.collider = (CircleCollider2D) base.collider2D;

		Reset (true);
	}

	private Vector2 newVel;

	/**
	 * this is mainly for the paddle since the built in physics work way better in unity.
	 */
	public void OnCollisionEnter2D(Collision2D coll) 
	{
		//if the ball hit one of the paddles ...
		if (coll.gameObject.tag == "paddle") 
		{
			//determine which direction the ball is traveling
			bool up = true;

			if (rotation < -90.0f)
			{
				up = false;
				rotation += 180.0f;
			}
			else if (rotation > 90.0f)
			{
				up = false;
				rotation -= 180.0f;
			}

			//determine how far off center the ball hit
			BoxCollider2D paddleCollider = (BoxCollider2D) coll.collider;
			CircleCollider2D ballCollider = (CircleCollider2D) physics.collider2D;

			float width = paddleCollider.size.x;
			float paddlex = coll.gameObject.transform.position.x;
			float ballx = _physics.position.x;

			float diffx = ballx - paddlex;

			//if the ball hit the edge or bottom/top of the paddle
			if ((up == true && ballCollider.transform.position.y >= coll.gameObject.transform.position.y) ||
			    (up == false && ballCollider.transform.position.y <= coll.gameObject.transform.position.y)
			    )
			{
				//weird stuff happens and this solution didn't help
				//physics.isKinematic = true;
			}
			//if the ball hit the side of the paddle act like the paddle is a wall
			else if (diffx > width - (ballCollider.radius / 2))
			{
				rotation = -rotation;
			}
			//if the ball hit the top/bottom of the paddle
			else
			{
				//use arcsin to model a curve to determine the angle of deflection
				float weight = diffx / (width / 2.0f);
				float asin = Mathf.Asin(Mathf.Clamp(weight, -1.0f, 1.0f));
				float rotateZ = 180.0f * asin / Mathf.PI;

				if (!up)
					rotateZ = -rotateZ;

				//don't exceed the maximum lateral movement
				rotation = Mathf.Clamp(rotation + rotateZ, -MAX_ROTATION, MAX_ROTATION);

				if (!up)
					rotation += 180.0f;
			
				//flip and reflect 
				rotation = -rotation + 180;
			}

			//play a sound on impact
			coll.gameObject.audio.Play();
		}
		//reflect the angle if we hit a wall
		else if (coll.gameObject.tag == "wall")
		{
			rotation = -rotation;
			coll.gameObject.audio.Play ();
		}



		//normalize the rotation
		if (rotation < -180.0f)
			rotation += 360.0f;
		
		while (rotation > 180.0f)
			rotation -= 360.0f;



		//rotate and set the new velocity.  The ball always maintains a constant speed
		Quaternion q = Quaternion.AngleAxis(rotation, new Vector3(0, 0, -1.0f));
		_physics.velocity = q * defaultDir;
	}

	//move the ball back to the center and pick a direction to fling it in
	public void Reset(bool up)
	{
		float offY = 0.0f;

		if (up)
		{
			rotation = 0f;
			offY = -2.0f;
		}
		else
		{
			rotation = 180.0f;
			offY = 2.0f;
		}

		_physics.velocity = Quaternion.AngleAxis(rotation, new Vector3(0, 0, -1.0f)) * defaultDir;
		_physics.MovePosition (new Vector2(0, offY));
		_physics.isKinematic = false;
	}

	void Update () 
	{
		//error checking
		if (physics.velocity.magnitude < 1.0f)
			Reset (true);
	}
}
