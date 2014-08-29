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

	private bool isColliding = false;

	public void OnCollisionEnter2D(Collision2D coll)
	{
		isColliding = true;
	}

	public void OnCollisionExit2D(Collision2D coll)
	{
		isColliding = false;
	}

	private Vector2 _lastVelocity;
	public Vector2 lastVelocity
	{
		get {return _lastVelocity;}
	}

	public void Update()
	{
		if (!isColliding)
			_lastVelocity = _physics.velocity;
	}
}
