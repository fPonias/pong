using UnityEngine;
using System.Collections;

/**
 * Main controller to rule them all
 */
public class MainScript : MonoBehaviour 
{
	private GameObject ball; //the ball
	private BallScript ballCtrl;
	private GUIText score; //global score label

	private GameObject playerPaddle; //player paddle
	private PaddleScript playerPaddleCtrl;
	private GameObject aiPaddle; //ai paddle
	private PaddleScript aiPaddleCtrl;

	private int playerScore;
	private int aiScore;

	// Use this for initialization
	void Start () 
	{
		playerScore = 0;
		aiScore = 0;

		ball = GameObject.FindGameObjectWithTag ("ball");
		ballCtrl = ball.GetComponent<BallScript> ();
		GameObject obj = GameObject.FindGameObjectWithTag ("score");
		score = obj.GetComponent<GUIText> ();

		//setup the ai and controls
		GameObject[] paddles = GameObject.FindGameObjectsWithTag ("paddle");
		playerPaddle = paddles [0];
		playerPaddleCtrl = playerPaddle.GetComponent<PaddleScript> ();
		playerPaddleCtrl.ailevel = 0;

		aiPaddle = paddles [1];
		aiPaddleCtrl = aiPaddle.GetComponent<PaddleScript> ();
		aiPaddleCtrl.ailevel = 2;

		//start by putting the ball in play
		Reset (true);
	}
	
	// Update is called once per frame
	void Update () 
	{
		//check if the ball went out of bounds, update the score and reset the ball position
		if (ballCtrl.physics.position.y > 5.3f)
		{
			playerScore++;
			Reset (true);
		}
		else if (ballCtrl.physics.position.y < -5.3f)
		{
			aiScore++;
			Reset (false);
		}

	}

	private void Reset(bool up)
	{
		ballCtrl.Reset (up);
		score.text = "AI - " + aiScore + " : Player - " + playerScore;
	}
}
