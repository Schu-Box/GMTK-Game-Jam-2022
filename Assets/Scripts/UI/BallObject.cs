using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallObject : MonoBehaviour
{
	public Ball ball;
	public Image ballImage;

	private GameController gameController;

	public void Setup(Ball newBall)
	{
		gameController = FindObjectOfType<GameController>();
		ball = newBall;
	}

	public void QueueDisplayMovement(Tile tile)
	{
		gameController.AddToAnimationQueue(() => DisplayMovement(tile, GameController.animationSpeed_BallMove));
	}
	private void DisplayMovement(Tile tile, float speed)
	{
		LeanTween.move(gameObject, tile.tileGameObject.transform.position, speed);

		gameController.CompleteQueueActionAfterDelay(speed);
	}

	public void QueueDisplayPossession(Athlete athlete)
	{
		gameController.AddToAnimationQueue(() => DisplayPossession(athlete, GameController.animationSpeed_BallPossession));
	}
	private void DisplayPossession(Athlete athlete, float duration)
	{
		Vector3 newPosition;
		if (athlete != null)
		{
			gameController.PlayAudio(gameController.possessionGained);

			newPosition = athlete.athleteGameObject.displayBallHolder.position;
			transform.SetParent(athlete.athleteGameObject.displayBallHolder);

			LeanTween.move(gameObject, newPosition, duration).setEaseOutElastic();
			//LeanTween.value(gameObject, UpdateColor, ballImage.color, athlete.team.teamColor, duration);
		}
		else
		{
			//newPosition = ball.currentTile_NoPossession.tileGameObject.transform.position;
			transform.SetParent(gameController.ballGameObjectParent);
		}
		

		gameController.CompleteQueueActionAfterDelay(duration);
	}

	private void UpdateColor(Color value)
	{
		ballImage.color = value;
	}

	public void QueueDisplayReset(Vector3 spawnPosition)
	{
		gameController.AddToAnimationQueue(() => DisplayReset(spawnPosition, GameController.animationSpeed_BallReset));
	}
	private void DisplayReset(Vector3 spawnPosition, float duration)
	{
		ballImage.color = Color.white;

		transform.SetParent(gameController.ballGameObjectParent);
		transform.position = spawnPosition;
		transform.localScale = Vector3.zero;

		LeanTween.scale(gameObject, Vector3.one, duration).setEaseInCubic();

		gameController.CompleteQueueActionAfterDelay(duration);
	}

	public void QueueDisplayScored()
	{
		gameController.AddToAnimationQueue(() => DisplayScored(GameController.animationSpeed_GoalBall));
	}
	private void DisplayScored(float duration)
	{
		LeanTween.scale(gameObject, Vector3.zero, duration).setEaseInBack();

		gameController.CompleteQueueActionAfterDelay(duration);
	}
}
