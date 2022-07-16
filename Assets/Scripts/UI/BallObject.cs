using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallObject : MonoBehaviour
{
	public Image ballImage;

	private GameController gameController;

	private void Awake()
	{
		gameController = FindObjectOfType<GameController>();
	}

	public void QueueDisplayMovement(Tile tile)
	{
		gameController.AddToAnimationQueue(() => DisplayMovement(tile, GameController.animationSpeed_Move));
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
		transform.SetParent(athlete.athleteGameObject.displayBallHolder);
		LeanTween.moveLocal(gameObject, Vector3.zero, duration);
		LeanTween.value(gameObject, UpdateColor, ballImage.color, athlete.team.teamColor, duration);

		gameController.CompleteQueueActionAfterDelay(0f);
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

		LeanTween.scale(gameObject, Vector3.one, duration);

		gameController.CompleteQueueActionAfterDelay(duration);
	}
}
