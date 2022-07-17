using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AthleteObject : MonoBehaviour
{
	public Transform displayDiceHolder;
	public Transform displayBallHolder;

	private GameController gameController;
	private void Awake()
	{
		gameController = FindObjectOfType<GameController>();
	}

	//private  Image image;
	public void FlipImage()
	{
		transform.eulerAngles = new Vector3(0, 180, 0);
	}

	public void QueueDisplayMovement(Tile movedToTile)
	{
		gameController.AddToAnimationQueue(() => DisplayMovement(movedToTile.tileGameObject, GameController.animationSpeed_Move));
	}

	public void DisplayMovement(TileObject tileObject, float speed)
	{
		LeanTween.move(gameObject, tileObject.transform.position, speed).setEaseOutCubic();

		gameController.CompleteQueueActionAfterDelay(speed);
	}

	public void QueueDisplayRoll(Dice diceRoll)
	{
		gameController.AddToAnimationQueue(() => DisplayRoll(diceRoll, GameController.animationSpeed_DiceRollGrow, GameController.animationSpeed_DiceRollShow));
	}

	private void DisplayRoll(Dice diceRoll, float durationGrow, float durationShow)
	{
		DiceObject newDiceObject = Instantiate(gameController.diceGameObjectPrefab,displayDiceHolder).GetComponent<DiceObject>();
		newDiceObject.Setup(diceRoll);
		newDiceObject.transform.localScale = Vector3.zero;
		newDiceObject.transform.localPosition = Vector3.zero;

		LeanTween.scale(newDiceObject.gameObject, Vector3.one, durationGrow);

		gameController.CompleteQueueActionAfterDelay(durationGrow + durationShow);
	}

	public void QueueDisplayDestroyDice()
	{
		gameController.AddToAnimationQueue(() => DisplayDestroyDice(GameController.animationSpeed_DiceDestroy));
	}

	private void DisplayDestroyDice(float duration)
	{
		for(int i = 0; i < displayDiceHolder.childCount; i++)
		{
			GameObject diceObject = displayDiceHolder.GetChild(i).gameObject;

			LeanTween.scale(diceObject, Vector3.zero, duration).setOnComplete(() => Destroy(diceObject));
		}

		gameController.CompleteQueueActionAfterDelay(duration);
	}

	private float speed = 10f;
	private float pulseRange = 0.2f;
	private bool highlightActive;
	private Coroutine highlightCoroutine = null;

	public void Highlight(bool highlight)
	{
		highlightActive = highlight;

		if (highlight)
		{
			if (highlightCoroutine == null)
				StartCoroutine(HighlightCoroutine());
		}
		else
		{
			transform.localScale = Vector3.one;
		}
	}

	public IEnumerator HighlightCoroutine()
	{
		float startTime = Time.time;

		WaitForFixedUpdate waiter = new WaitForFixedUpdate();
		while(highlightActive)
		{
			Vector3 vec = new Vector3(Mathf.Sin((Time.time - startTime) * speed) * pulseRange + 1, Mathf.Sin((Time.time - startTime) * speed) * pulseRange + 1, Mathf.Sin((Time.time - startTime) * speed) * pulseRange + 1);

			transform.localScale = vec;

			yield return waiter;
		}

		highlightCoroutine = null;
	}
}
